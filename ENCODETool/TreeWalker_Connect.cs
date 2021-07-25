using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    partial class TreeWalker
    {
        public static void ConnectProject(Project project)
        {
            int totalFields = project.oodMembers.Count;
            int writeDevision = (totalFields / 10) + 1;

            for (int fieldIndex = 0; fieldIndex < totalFields; fieldIndex++)
            {
                ConnectObjectFields(new IndexTuple((int)Types.Member, fieldIndex), project);

                // Update the progress bar in the window
                int percentage = (int)((fieldIndex * 16.6667f / totalFields) + 66.6667f);
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (fieldIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Connecting Field {fieldIndex}/{totalFields} - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Connecting Field {fieldIndex}/{totalFields} - {percentage}% ");
                }
            }
            Logger.ReWrite($"Finished Connecting Fields {totalFields}/{totalFields} - 100%\n");

            int totalMethods = project.oodMethods.Count;
            writeDevision = (totalMethods / 10) + 1;

            for (int methodIndex = 0; methodIndex < totalMethods; methodIndex++)
            {
                foreach (var expressionIndex in project.oodMethods[methodIndex].oodExpressions)
                {
                    ConnectMethodFields(new IndexTuple((int)Types.Method, methodIndex), expressionIndex, project);
                }
                // Update the progress bar in the window
                int percentage = (int)((methodIndex * 16.6667f / totalMethods) + 83.3334f);
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (methodIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Connecting Methods {methodIndex}/{totalMethods} - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Connecting Methods {methodIndex}/{totalMethods} - {percentage}% ");
                }
            }
            Logger.ReWrite($"Finished Connecting Methods {totalMethods}/{totalMethods} - 100%");
            Logger.WriteLine($"\n{project.noOfErrors} Fields detected that can't be connected.");

        }

        private static void ConnectMethodFields(IndexTuple oodMethodIndex, IndexTuple expressionIndex, Project project)
        {
            OODExpression oodExpression = project.oodExpressions[expressionIndex.itemIndex];

            // Handle all write variables
            for (int i = 0; i < oodExpression.oodWriteVariables.Count; i++)
            {
                IndexTuple writeVariable = oodExpression.oodWriteVariables[i];
                OODMember oodMember = project.oodMembers[writeVariable.itemIndex];
                string[] oodMemberNameParts = oodMember.name.Split('.');

                Connect(oodMemberNameParts, oodMethodIndex, expressionIndex, writeVariable, i, project, true);
            }

            // Handle all read variables
            for (int i = 0; i < oodExpression.oodReadVariables.Count; i++)
            {
                IndexTuple readVariable = oodExpression.oodReadVariables[i];
                OODMember oodMember = project.oodMembers[readVariable.itemIndex];
                string[] oodMemberNameParts = oodMember.name.Split('.');

                Connect(oodMemberNameParts, oodMethodIndex, expressionIndex, readVariable, i, project, false);
            }

            // Handle all Method Calls
            for (int i = 0; i < project.oodExpressions[expressionIndex.itemIndex].oodSubExpressions.Count; i++)
            {
                IndexTuple subExpressionIndex = project.oodExpressions[expressionIndex.itemIndex].oodSubExpressions[i];
                ConnectMethodFields(oodMethodIndex, subExpressionIndex, project);
            }

        }

        private static void Connect(string[] oodMemberNameParts, IndexTuple oodMethodIndex, IndexTuple oodExpressionIndex, IndexTuple variableIndex, int variableNumber, Project project, bool write)
        {
            string currName = oodMemberNameParts[0];
            IndexTuple matchIndex;

            OODMethod oodMethod = project.oodMethods[oodMethodIndex.itemIndex];
            OODExpression oodExpression = project.oodExpressions[oodExpressionIndex.itemIndex];


            if (write && NewToLocalVariables(oodExpression, oodMethod, currName, variableIndex, project, out matchIndex))
            {
                // A new local declaration variable, add to local variables
                project.oodMethods[oodMethodIndex.itemIndex].oodLocalVariables.Add(variableIndex);
                return;
            }
            else if (LocalOrParameterVariable(oodExpression, oodMethod, currName, project, out matchIndex) &&
                StopRecursing(oodMemberNameParts, project, matchIndex))
            {
                // Variable is already known in method, but is ignored in planning, only update expression 

                UpdateExpression(oodExpressionIndex, variableNumber, project, matchIndex, write);
                return;
            }
            else if (LocalOrParameterVariable(oodExpression, oodMethod, currName, project, out matchIndex) ||
                GlobalReadOrWriteVariable(oodExpression, oodMethod, currName, project, out matchIndex) ||
                SearchClassHierarchy(oodMethod.parentIndices.FirstOrDefault(), currName, project, out matchIndex) ||
                IsStaticType(oodMethod.namespaceName, currName, variableIndex, project, out matchIndex))
            {
                // Variable is already known in method, update expression and update method if necessary
                // Variable is a global one from a class or parent class

                // Look at next part of the compound name
                if (Recurse(oodMemberNameParts.Skip(1).ToArray(), project, ref matchIndex))
                {
                    UpdateExpression(oodExpressionIndex, variableNumber, project, matchIndex, write);
                    UpdateMethod(oodMethodIndex, oodMethod, project, matchIndex, write);
                    return;
                }
            }

            // If there was ultimately no match
            if (!Ignore(currName, variableIndex, project) && !IsEnum(currName, project) && !matchIndex.IsValid())
            {
                // can't proccess
                PrintError(variableIndex, project, write);
                return;
            }


        }

        private static bool NewToLocalVariables(OODExpression oodExpression, OODMethod oodMethod, string currName, IndexTuple variableIndex, Project project, out IndexTuple matchIndex)
        {
            OODMember oodMember = project.oodMembers[variableIndex.itemIndex];
            return !CompareFieldAndType(oodMethod.oodLocalVariables, currName, oodMember.variableType, project, out matchIndex) && oodExpression.localDeclaration;
        }

        private static bool LocalOrParameterVariable(OODExpression oodExpression, OODMethod oodMethod, string currName, Project project, out IndexTuple matchIndex)
        {
            return CompareField(oodMethod.oodLocalVariables, currName, project, out matchIndex) ||
                CompareField(oodMethod.oodParameters, currName, project, out matchIndex);

        }
        private static bool GlobalReadOrWriteVariable(OODExpression oodExpression, OODMethod oodMethod, string currName, Project project, out IndexTuple matchIndex)
        {
            return CompareField(oodMethod.oodReadGlobalVariables, currName, project, out matchIndex) ||
                CompareField(oodMethod.oodWriteGlobalVariables, currName, project, out matchIndex);

        }

        private static bool SearchClassHierarchy(IndexTuple oodClassIndex, string currName, Project project, out IndexTuple matchIndex)
        {
            OODClass oodClass = project.oodClasses[oodClassIndex.itemIndex];

            // If present in this class, return matchIndex
            if (CompareField(oodClass.oodVariables, currName, project, out matchIndex))
                return true;

            // Else recurse over parent classes
            foreach (var groupIndex in oodClass.groupIndices)
            {
                // If the group exists and this class is not the parent class
                if (project.GroupListGetParentIndex(groupIndex, out IndexTuple groupParentIndex) == 1 && groupParentIndex != oodClassIndex)
                {
                    //// check parent variables and recurse
                    if (SearchClassHierarchy(groupParentIndex, currName, project, out matchIndex))
                        return true;
                }
            }

            return false;
        }

        private static void UpdateMethod(IndexTuple oodMethodIndex, OODMethod oodMethod, Project project, IndexTuple matchIndex, bool write)
        {
            if (write && !oodMethod.oodWriteGlobalVariables.Contains(matchIndex))
                project.oodMethods[oodMethodIndex.itemIndex].oodWriteGlobalVariables.Add(matchIndex);
            else if (!oodMethod.oodReadGlobalVariables.Contains(matchIndex))
                project.oodMethods[oodMethodIndex.itemIndex].oodReadGlobalVariables.Add(matchIndex);
        }

        private static void UpdateExpression(IndexTuple oodExpressionIndex, int variableNumber, Project project, IndexTuple matchIndex, bool write)
        {
            if (write)
                project.oodExpressions[oodExpressionIndex.itemIndex].oodWriteVariables[variableNumber] = matchIndex;
            else
                project.oodExpressions[oodExpressionIndex.itemIndex].oodReadVariables[variableNumber] = matchIndex;
        }

        private static bool CompareField(List<IndexTuple> compareIndexList, string oodCompName, Project project, out IndexTuple matchIndex)
        {
            int matches = 0;
            matchIndex = new IndexTuple();

            for (int i = 0; i < compareIndexList.Count; i++)
            {
                OODMember matchField = project.oodMembers[compareIndexList[i].itemIndex];

                if (oodCompName == matchField.name)
                {
                    matchIndex = compareIndexList[i];
                    matches++;
                }
            }
            return matches == 1;
        }

        private static bool CompareFieldAndType(List<IndexTuple> compareIndexList, string compareName, string compareType, Project project, out IndexTuple matchIndex)
        {
            int matches = 0;
            matchIndex = new IndexTuple();

            for (int i = 0; i < compareIndexList.Count; i++)
            {
                OODMember matchField = project.oodMembers[compareIndexList[i].itemIndex];

                if (compareName == matchField.name && compareType == matchField.variableType)
                {
                    matchIndex = compareIndexList[i];
                    matches++;
                }
            }
            return matches == 1;
        }

        /// <summary>
        /// Check if a static class can be matched to the variable
        /// </summary>
        /// <param name="oodMemberNamespace"></param>
        /// <param name="oodMemberName"></param>
        /// <param name="variableIndex"></param>
        /// <param name="project"></param>
        /// <param name="matchIndex"></param>
        /// <returns></returns>
        private static bool IsStaticType(string oodMemberNamespace, string oodMemberName, IndexTuple variableIndex, Project project, out IndexTuple matchIndex)
        {
            // Static class in this or a parent namespace
            if (CheckParentNamespacesForClass(oodMemberNamespace, oodMemberName, project, out matchIndex))
                return true;


            // Check using namespaces for static methods
            OODMember oodMember = project.oodMembers[variableIndex.itemIndex];
            if (oodMember.rootTreeNumber < 0) return false;
            List<IndexTuple> usingIndices = project.oodFiles[oodMember.rootTreeNumber].oodUsing;

            // Only the using namespaces (not its parents)
            for (int i = 0; i < usingIndices.Count; i++)
            {
                IndexTuple usingIndex = usingIndices[i];
                string usingNamespace = project.oodMembers[usingIndex.itemIndex].name;

                // Static type
                if (CheckNamespaceForClass(usingNamespace, oodMemberName, project, out matchIndex))
                    return true;
            }

            return false;
        }

        private static bool Recurse(string[] oodMemberNameParts, Project project, ref IndexTuple matchIndex)
        {
            // If there is no name part left or the last part is irrelevant or it is from a manual base class
            OODMember matchField = project.oodMembers[matchIndex.itemIndex];

            if (StopRecursing(oodMemberNameParts, project, matchIndex))
                return true;

            string currName = oodMemberNameParts[0];

            if (SearchClassHierarchy(matchField.oodLinkIndex, currName, project, out matchIndex))
                return Recurse(oodMemberNameParts.Skip(1).ToArray(), project, ref matchIndex);

            return false;
        }

        private static bool StopRecursing(string[] oodMemberNameParts, Project project, IndexTuple matchIndex)
        {
            OODMember matchField = project.oodMembers[matchIndex.itemIndex];
            return oodMemberNameParts.Length < 1 || !matchField.oodLinkIndex.IsValid() || matchField.rootTreeNumber == -1;
        }

        private static void PrintError(IndexTuple variableIndex, Project project, bool write)
        {
            string label = project.oodMembers[variableIndex.itemIndex].GetLabel();
            project.noOfErrors++;

            if (write)
                Logger.WriteLineDebug($"[{label}] Write variable can't be connected!");
            else
                Logger.WriteLineDebug($"[{label}] Read variable can't be connected!");

        }


        /// <summary>
        /// Ignore variables that are used from a using directive
        /// </summary>
        /// <param name="oodMemberIndex"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        private static bool Ignore(string currName, IndexTuple oodMemberIndex, Project project)
        {
            OODMember oodMember = project.oodMembers[oodMemberIndex.itemIndex];
            if (oodMember.rootTreeNumber < 0)
                return true;
            HashSet<string> ignoreList = project.oodFiles[oodMember.rootTreeNumber].ignoreList;

            if (ignoreList.Contains(currName)) return true;
            if (ignoreList.Contains(oodMember.variableType.Split('.')[0])) return true;

            return false;
        }

        private static bool IsEnum(string currName, Project project)
        {
            var key = $"{Constant.ENUM_DECLARATION_TYPE}_{currName}"; // TODO variables from other namespaces

            if (project.oodRawDictionary.ContainsKey(key))
                return true;

            return false;
        }

        private static void ConnectObjectFields(IndexTuple oodMemberIndex, Project project)
        {
            OODMember oodMember = project.oodMembers[oodMemberIndex.itemIndex];
            if (oodMember.rootTreeNumber < 0)
                return;

            List<IndexTuple> usingIndices = project.oodFiles[oodMember.rootTreeNumber].oodUsing;

            // -- This namespace and its parent namespaces --
            // Variable type
            if (CheckParentNamespacesForClass(oodMember.namespaceName, oodMember.variableType, project, out IndexTuple matchIndex))
                project.oodMembers[oodMemberIndex.itemIndex].oodLinkIndex = matchIndex;

            // Static type
            if (CheckParentNamespacesForClass(oodMember.namespaceName, oodMember.name.Split('.')[0], project, out matchIndex))
                project.oodMembers[oodMemberIndex.itemIndex].oodLinkIndex = matchIndex;


            // Only the using namespaces (not its parents)
            for (int i = 0; i < usingIndices.Count; i++)
            {
                IndexTuple usingIndex = usingIndices[i];
                string usingNamespace = project.oodMembers[usingIndex.itemIndex].name;

                // -- This namespace --
                // Variable type
                if (CheckNamespaceForClass(usingNamespace, oodMember.variableType, project, out matchIndex))
                    project.oodMembers[oodMemberIndex.itemIndex].oodLinkIndex = matchIndex;

                // Static type
                if (CheckNamespaceForClass(usingNamespace, oodMember.name.Split('.')[0], project, out matchIndex))
                    project.oodMembers[oodMemberIndex.itemIndex].oodLinkIndex = matchIndex;
            }
        }


        private static bool CheckNamespaceForClass(string checkNamespace, string checkName, Project project, out IndexTuple matchIndex)
        {
            string key = $"{Types.Class}_{checkNamespace}_{checkName}"; // TODO variables from other namespaces

            if (project.oodRawDictionary.ContainsKey(key))
            {
                matchIndex = project.oodRawDictionary[key];
                return true;
            }

            matchIndex = new IndexTuple();
            return false;
        }

        private static bool CheckParentNamespacesForClass(string oodMemberNamespace, string oodMemberName, Project project, out IndexTuple matchIndex)
        {
            // Static class in this or a parent namespace
            string[] namespaceParts = oodMemberNamespace.Split('.');
            string parentNamespace = "";

            for (int i = 0; i < namespaceParts.Length; i++)
            {
                string dot = parentNamespace == "" ? "" : ".";
                parentNamespace += dot + namespaceParts[i];
                if (CheckNamespaceForClass(parentNamespace, oodMemberName, project, out matchIndex))
                    return true;
            }

            matchIndex = new IndexTuple();
            return false;
        }
    }
}
