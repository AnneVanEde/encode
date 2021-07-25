using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ENCODE.Base
{
    static partial class TreeWalker
    {
        public static void LinkProject(Project project)
        {
            int totalChildren = project.oodRawItems.Count;
            int writeDevision = (totalChildren / 10) + 1;

            for (int childIndex = 0; childIndex < totalChildren; childIndex++)
            {
                LinkNode(project.oodRawItems[childIndex], project, new IndexTuple());

                // Update the progress bar in the window
                int percentage = (int)((childIndex * 33.3333f / totalChildren) + 33.3333f);
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (childIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Linking Tree {childIndex}/{totalChildren} - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Linking Tree {childIndex}/{totalChildren} - {percentage}% ");
                }
            }
            Logger.ReWrite($"Finished Linking {totalChildren}/{totalChildren} - 100%");
            if (!Constant.DEBUG_OUTPUT)
                Logger.WriteLineToConsoleOnly("");

            Logger.WriteLineDebug($"\n{project.oodFiles.Count()} Linked Files");
            Logger.WriteLineDebug($"{project.oodNamespaces.Count()} Linked Namespaces");
            Logger.WriteLineDebug($"{project.oodClasses.Count()} Linked Classes");
            Logger.WriteLineDebug($"{project.classGroups.Count()} Linked Class Groups");
            Logger.WriteLineDebug($"{project.oodMethods.Count()} Linked Methods");
            Logger.WriteLineDebug($"{project.methodGroups.Count()} Linked Method Groups");
            Logger.WriteLineDebug($"{project.oodMembers.Count()} Linked Fields");

        }

        private static IndexTuple LinkNode(OODItem oodItem, Project project, IndexTuple parentIndex)
        {
            if (oodItem.GetType() == typeof(OODFile))
                return LinkNode((OODFile)oodItem, project, parentIndex);
            else if (oodItem.GetType() == typeof(OODNamespace))
                return LinkNode((OODNamespace)oodItem, project, parentIndex);
            else if (oodItem.GetType() == typeof(OODClass))
                return LinkNode((OODClass)oodItem, project, parentIndex);
            else if (oodItem.GetType() == typeof(OODMethod))
                return LinkNode((OODMethod)oodItem, project, parentIndex);
            else if (oodItem.GetType() == typeof(OODExpression))
                return LinkNode((OODExpression)oodItem, project, parentIndex);
            else if (oodItem.GetType() == typeof(OODMember))
                return LinkNode((OODMember)oodItem, project, parentIndex);
            else
                return new IndexTuple(-1, -1);
        }

        private static IndexTuple LinkNode(OODFile oodItem, Project project, IndexTuple parentIndex)
        {
            IndexTuple index = project.ListAdd(oodItem, parentIndex);

            foreach (var childItem in oodItem.oodRawItems)
            {
                project.ListAddChild(index, LinkNode(childItem, project, index));
            }

            return index;
        }

        private static IndexTuple LinkNode(OODNamespace oodItem, Project project, IndexTuple parentIndex)
        {
            IndexTuple index = project.ListAdd(oodItem, parentIndex);

            List<IndexTuple> indexTuples = new List<IndexTuple>();

            foreach (var childItem in oodItem.oodRawItems)
            {
                IndexTuple indexTuple = LinkNode(childItem, project, index);
                project.ListAddChild(index, indexTuple);

                if (indexTuple.arrayIndex == (int)Types.Class)
                    indexTuples.Add(indexTuple);
            }
            project.ListAddChild(index, indexTuples);

            return index;
        }


        /// <summary>
        /// Link DOCItems together. 
        /// </summary>
        /// <param name="oodItem"></param>
        /// <param name="project"></param>
        /// <param name="parentIndex"></param>
        /// <returns></returns>
        private static IndexTuple LinkNode(OODClass oodItem, Project project, IndexTuple parentIndex)
        {
            if (oodItem.rootTreeNumber != -1 && !oodItem.rawSymbol.ContainingNamespace.IsGlobalNamespace)
                oodItem.namespaceName = oodItem.rawSymbol.ContainingNamespace.ToString();

            IndexTuple index = project.ListAdd(oodItem, parentIndex);

            INamedTypeSymbol symbolInfo = (INamedTypeSymbol)oodItem.rawSymbol;
            // If class extension, seen as same class
            if (oodItem.rootTreeNumber != -1 && symbolInfo.BaseType != null && symbolInfo.BaseType.Name != symbolInfo.Name)
            {
                INamedTypeSymbol baseClass = symbolInfo.BaseType;

                OODClass oodBaseClass = new OODClass(baseClass.Name, baseClass);
                if (!baseClass.ContainingNamespace.IsGlobalNamespace)
                    oodBaseClass.namespaceName = baseClass.ContainingNamespace.ToString();

                IndexTuple baseClassIndex = project.ListAdd(oodBaseClass, new IndexTuple());

                // Add this base class to class's list
                project.oodClasses[index.itemIndex].baseClassIndex = baseClassIndex;

                // Add to class group or create class group
                IndexTuple groupIndex = project.AddToClassGroup(baseClassIndex, index);
                project.oodClasses[index.itemIndex].groupIndices.Add(groupIndex);
                project.oodClasses[baseClassIndex.itemIndex].groupIndices.Add(groupIndex);
            }


            foreach (var childItem in oodItem.oodRawItems)
            {
                project.ListAddChild(index, LinkNode(childItem, project, index));
            }

            return index;
        }

        private static IndexTuple LinkNode(OODMethod oodItem, Project project, IndexTuple parentIndex)
        {
            IMethodSymbol symbolInfo = (IMethodSymbol)oodItem.rawSymbol;

            // Collect paramters for original method
            GetParameters(project, oodItem, symbolInfo);
            IndexTuple index = project.ListAdd(oodItem, parentIndex);

            // Check for overriden methods
            if (symbolInfo.IsOverride)
            {
                IMethodSymbol overridden = symbolInfo.OverriddenMethod;

                OODMethod oodOverridenItem = new OODMethod(overridden.Name, overridden);

                // Collect parameters for overriden method
                GetParameters(project, oodOverridenItem, overridden);

                IndexTuple baseClassIndex = new IndexTuple();

                // TODO: 'If' because otherwise it breaks on structs
                if (parentIndex.arrayIndex == (int)Types.Class)
                {
                    project.DOCListGetItem(parentIndex, out OODItem classItem);
                    baseClassIndex = !(((OODClass)classItem).baseClassIndex is null) ? ((OODClass)classItem).baseClassIndex : new IndexTuple();
                }

                IndexTuple overridenIndex = project.ListAdd(oodOverridenItem, baseClassIndex);

                // Add to method group or create method group
                IndexTuple groupIndex = project.AddToMethodGroup(overridenIndex, index);
                project.oodMethods[index.itemIndex].groupIndex.Add(groupIndex);
                project.oodMethods[overridenIndex.itemIndex].groupIndex.Add(groupIndex);
            }

            // Find all expressions in method
            GetExpressions(oodItem, project, index);

            return index;
        }

        private static void GetExpressions(OODItem oodItem, Project project, IndexTuple parentIndex)
        {
            foreach (var childItem in oodItem.oodRawItems)
            {
                if (childItem.GetType() == typeof(OODExpression))
                {
                    project.ListAddChild(parentIndex, LinkNode(childItem, project, parentIndex));
                }

                GetExpressions(childItem, project, parentIndex);
            }
        }

        private static void GetParameters(Project project, OODMethod oodItem, IMethodSymbol symbol)
        {
            foreach (var parameter in symbol.Parameters)
            {
                OODMember oodParameter = new OODMember(parameter.Name, Constant.IDENTIFIER_TYPE)
                {
                    variableType = parameter.Type.ToString()
                };
                IndexTuple parameterIndex = project.ListAdd(oodParameter, new IndexTuple());

                oodItem.oodParameters.Add(parameterIndex);
                oodItem.parameters += oodItem.parameters == "" ? parameter.Type.ToString() : "," + parameter.Type.ToString();
            }

            foreach (var parameterIndex in oodItem.oodParameters)
            {
                project.oodMembers[parameterIndex.itemIndex].parentName = oodItem.GetParentLabel();
            }
        }

        private static IndexTuple LinkNode(OODMember oodItem, Project project, IndexTuple parentIndex)
        {
            IndexTuple index = new IndexTuple();

            var childItem = oodItem.oodRawItems.LastOrDefault();

            if (oodItem.type == Constant.USING_TYPE || oodItem.type == Constant.ENUM_DECLARATION_TYPE)
            {
                index = project.ListAdd((OODMember)oodItem, parentIndex);
            }
            else if (oodItem.type == Constant.GLOBAL_DECLARATION_TYPE && childItem == null)
            {
                index = project.ListAdd((OODMember)oodItem, parentIndex);
            }
            else if (childItem != null && childItem.type == "VariableDeclaration" && childItem.oodRawItems.Count >= 2)
            {
                List<IndexTuple> indices = new List<IndexTuple>();

                for (int child = 0; child < childItem.oodRawItems.Count; child++)
                {
                    var oodChildItem = childItem.oodRawItems[child];

                    // End of the Line
                    if (oodChildItem.type == "VariableDeclarator")
                    {
                        // A = ??
                        OODMember newOodMember = new OODMember(oodChildItem.name, childItem.type, childItem.rootTreeNumber)
                        {
                            variableType = ((OODMember)childItem).variableType
                        };

                        //((OODMember)oodChildItem).variableType = ((OodMember)childItem).variableType;
                        indices.Add(project.ListAdd(newOodMember, parentIndex));

                    }
                }

                // Workaround if multiple children, add extras manually
                if (indices.Count >= 1)
                {
                    index = indices[0];
                    for (int i = 1; i < indices.Count; i++)
                    {
                        project.ListAddChild(parentIndex, indices[i]);

                    }
                }
            }




            return index;
        }


        #region Expressions


        private static IndexTuple LinkNode(OODExpression oodItem, Project project, IndexTuple parentIndex)
        {
            IndexTuple index = project.ListAdd(oodItem, parentIndex);

            if (oodItem.type == Constant.RETURN_EXPRESSION_TYPE)
            {
                bool writing = false;

                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, index, ref writing);
                }
            }
            else if (oodItem.type == Constant.EXPRESSION_TYPE)
            {
                bool writing = true;
                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, index, ref writing);
                }
            }
            else if (oodItem.type == Constant.IF_EXPRESSION_TYPE)
            {
                bool writing = false;

                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, index, ref writing);
                }
            }
            else if (oodItem.type == Constant.LOCAL_DECLARATION_TYPE)
            {
                bool writing = true;
                project.oodExpressions[index.itemIndex].localDeclaration = true;

                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, index, ref writing);
                }
            }
            else if (oodItem.type == Constant.FOR_STATEMENT_TYPE)
            {
                bool writing = true;
                project.oodExpressions[index.itemIndex].localDeclaration = true;

                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, index, ref writing);
                }
            }
            else if (oodItem.type == Constant.FOREACH_STATEMENT_TYPE)
            {
                bool writing = true;
                project.oodExpressions[index.itemIndex].localDeclaration = true;

                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, index, ref writing);
                }
            }

            return index;
        }


        private static void LinkExpressionVariables(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            if (oodItem.GetType() != typeof(OODMember)) return;

            if (oodItem.type == "SimpleMemberAccessExpression")
            {
                // This is a array or composite variable
                UnravelSimpleMemberAccessExpression(oodItem, project, parentIndex, ref writing);
            }
            else if (oodItem.type == "InvocationExpression")
            {
                UnravelInvocationExpression(oodItem, project, parentIndex, ref writing);
            }
            else if (oodItem.type == "ElementAccessExpression")
            {
                UnravelElementAccessExpression(oodItem, project, parentIndex, ref writing);
            }
            else if (oodItem.type == "VariableDeclaration")
            {
                UnravelVariableDeclaration(oodItem, project, parentIndex, ref writing);
            }
            else if (oodItem.type == Constant.IDENTIFIER_TYPE)
            {
                AddIdentifierToExpression(oodItem, project, parentIndex, ref writing);
            }
            else if (oodItem.type == "ObjectCreationExpression" || oodItem.type == "ArrayType" || oodItem.type == "CastExpression")
            {
                for (int i = 1; i < oodItem.oodRawItems.Count; i++)
                {
                    var childItem = oodItem.oodRawItems[i];
                    LinkExpressionVariables(childItem, project, parentIndex, ref writing);
                }
            }
            else if (oodItem.type == "ObjectInitializerExpression")
            {
                for (int i = 1; i < oodItem.oodRawItems.Count; i++)
                {
                    var childItem = oodItem.oodRawItems[i];
                    if (childItem.type == "SimpleAssignmentExpression")
                        childItem.type = "SimpleObjectAssignmentExpression";
                    LinkExpressionVariables(childItem, project, parentIndex, ref writing);
                }
            }
            else if (oodItem.type == "SimpleObjectAssignmentExpression")
            {
                for (int i = 1; i < oodItem.oodRawItems.Count; i++)
                {
                    var childItem = oodItem.oodRawItems[i];
                    LinkExpressionVariables(childItem, project, parentIndex, ref writing);
                }
            }
            else if (oodItem.type == "AsExpression" || oodItem.type == "IsExpression")
            {
                for (int i = 0; i < oodItem.oodRawItems.Count - 1; i++)
                {
                    var childItem = oodItem.oodRawItems[i];
                    LinkExpressionVariables(childItem, project, parentIndex, ref writing);
                }
            }
            else
            {
                foreach (var childItem in oodItem.oodRawItems)
                {
                    LinkExpressionVariables(childItem, project, parentIndex, ref writing);
                }
            }

        }

        private static void UnravelSimpleMemberAccessExpression(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            if (oodItem.oodRawItems.Count != 2) return;

            var oodFirstChildItem = oodItem.oodRawItems[0];
            var oodSecondChildItem = oodItem.oodRawItems[1];



            // End of the Line
            if (oodFirstChildItem.type == Constant.IDENTIFIER_TYPE && oodSecondChildItem.type == Constant.IDENTIFIER_TYPE)
            {
                // A.B
                string oldName = (oodItem.name == "" || oodItem.name == "()" || oodItem.name == "[]") ? oodItem.name : $".{oodItem.name}";
                oodItem.name = $"{oodFirstChildItem.name}.{oodSecondChildItem.name}{oldName}";

                int rootnumber = Math.Min(Math.Min(oodFirstChildItem.rootTreeNumber, oodSecondChildItem.rootTreeNumber), oodItem.rootTreeNumber);
                oodItem.rootTreeNumber = rootnumber;

                IndexTuple index = project.ListAdd((OODMember)oodItem, parentIndex);

                // Store Index
                project.ListAddChildExpression(parentIndex, index, writing);

                writing = false;


            }
            else if (oodItem.parentIndices.Count >= 1 && project.oodMembers[oodItem.parentIndices.FirstOrDefault().itemIndex].type == "ObjectInitializerExpression")
            {
                LinkExpressionVariables(oodSecondChildItem, project, parentIndex, ref writing);
            }
            else if (oodSecondChildItem.type == Constant.IDENTIFIER_TYPE)
            {
                // Recurse deeper into structure
                // ?.C
                oodFirstChildItem.name = oodSecondChildItem.name + oodFirstChildItem.name;

                LinkExpressionVariables(oodFirstChildItem, project, parentIndex, ref writing);
            }
        }

        private static void UnravelInvocationExpression(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            if (oodItem.oodRawItems.Count != 2) return;

            var oodFirstChildItem = oodItem.oodRawItems[0];
            var oodSecondChildItem = oodItem.oodRawItems[1];

            OODExpression oodExpression = new OODExpression(oodItem.name, oodItem.type, oodItem.rootTreeNumber);
            oodExpression.oodRawItems = oodItem.oodRawItems;

            // End of the Line
            if (oodFirstChildItem.type == Constant.IDENTIFIER_TYPE && oodSecondChildItem.type == "ArgumentList") // <-- Hanlde Method calls differently
            {
                // A(?)
                string oldName = oodItem.name != "" ? $".{oodItem.name}" : "";
                oodExpression.name = $"{oodFirstChildItem.name}(){oldName}";

                int rootnumber = Math.Min(Math.Min(oodFirstChildItem.rootTreeNumber, oodSecondChildItem.rootTreeNumber), oodItem.rootTreeNumber);
                oodItem.rootTreeNumber = rootnumber;

                oodExpression.oodSuperExpressions.Add(parentIndex);

                IndexTuple index = project.ListAdd(oodExpression, parentIndex);

                // Store Index
                project.ListAddChildExpression(parentIndex, index, true); // add child to super
                project.ListAddChildExpression(index, parentIndex, false); // add parent to child

                // Add Arguments as well
                writing = false;
                LinkExpressionVariables(oodSecondChildItem, project, index, ref writing);

            }
            else if (oodFirstChildItem.type == "SimpleMemberAccessExpression" && oodSecondChildItem.type == "ArgumentList")
            {
                // ?.?(?)
                oodFirstChildItem.name += "()";

                int rootnumber = Math.Min(Math.Min(oodFirstChildItem.rootTreeNumber, oodSecondChildItem.rootTreeNumber), oodItem.rootTreeNumber);
                oodItem.rootTreeNumber = rootnumber;

                IndexTuple index = project.ListAdd(oodExpression, parentIndex);

                project.ListAddChildExpression(parentIndex, index, true); // add child to super
                project.ListAddChildExpression(index, parentIndex, false); // add parent to child

                // Find the main Variable -> ?.?
                LinkExpressionVariables(oodFirstChildItem, project, index, ref writing);

                // put results from child on current item
                if (oodExpression.oodReadVariables.Count == 1)
                {
                    project.oodExpressions[index.itemIndex].name = project.oodMembers[oodExpression.oodReadVariables[0].itemIndex].name;
                    oodExpression.oodReadVariables.Clear();
                }
                else if (oodExpression.oodWriteVariables.Count == 1)
                {
                    project.oodExpressions[index.itemIndex].name = project.oodMembers[oodExpression.oodWriteVariables[0].itemIndex].name;
                    oodExpression.oodWriteVariables.Clear();
                }

                // Add Arguments as well -> (?)
                writing = false;
                LinkExpressionVariables(oodSecondChildItem, project, index, ref writing);
            }

        }

        private static void UnravelElementAccessExpression(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            if (oodItem.oodRawItems.Count != 2) return;

            var oodFirstChildItem = oodItem.oodRawItems[0];
            var oodSecondChildItem = oodItem.oodRawItems[1];

            // End of the Line
            if (oodFirstChildItem.type == Constant.IDENTIFIER_TYPE && oodSecondChildItem.type == "BracketedArgumentList")
            {
                // A[?]
                OODMember oodMember = (OODMember)oodItem;
                string oldName = oodMember.name != "" ? $".{oodMember.name}" : "";
                oodMember.name = $"{oodFirstChildItem.name}{oldName}";
                oodMember.variableType = "[]";

                int rootnumber = Math.Min(Math.Min(oodFirstChildItem.rootTreeNumber, oodSecondChildItem.rootTreeNumber), oodItem.rootTreeNumber);
                oodItem.rootTreeNumber = rootnumber;

                IndexTuple index = project.ListAdd(oodMember, parentIndex);

                // Store Index
                project.ListAddChildExpression(parentIndex, index, writing);

                // Add Arguments as well
                writing = false;
                LinkExpressionVariables(oodSecondChildItem, project, parentIndex, ref writing);

            }
            else if (oodFirstChildItem.type == "ElementAccessExpression" && oodSecondChildItem.type == "BracketedArgumentList")
            {
                // Recurse deeper into structure
                // ?[?][?]

                // Find the main Variable
                LinkExpressionVariables(oodFirstChildItem, project, parentIndex, ref writing);

                // Add Arguments as well
                writing = false;
                LinkExpressionVariables(oodSecondChildItem, project, parentIndex, ref writing);
            }
        }

        private static void UnravelVariableDeclaration(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            for (int child = 0; child < oodItem.oodRawItems.Count; child++)
            {
                var oodChildItem = oodItem.oodRawItems[child];

                if (oodChildItem.type == Constant.IDENTIFIER_TYPE && oodChildItem.rootTreeNumber < 0)
                    oodItem.rootTreeNumber = oodChildItem.rootTreeNumber;

                // End of the Line
                if (oodChildItem.type == "VariableDeclarator")
                {
                    // A = ??
                    oodChildItem.rootTreeNumber = oodItem.rootTreeNumber;
                    ((OODMember)oodChildItem).variableType = ((OODMember)oodItem).variableType;
                    IndexTuple index = project.ListAdd((OODMember)oodChildItem, parentIndex);

                    // Store Index
                    project.ListAddChildExpression(parentIndex, index, writing);

                    // Add Arguments as well
                    bool tempWriting = false;
                    LinkExpressionVariables(oodChildItem, project, parentIndex, ref tempWriting);

                }
            }
            writing = false;

        }

        private static void AddArgumentToExpression(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            if (oodItem.oodRawItems.Count != 1) return;

            var oodFirstChildItem = oodItem.oodRawItems[0]; // <-- Group arguments for methods


            if (oodFirstChildItem.type == Constant.IDENTIFIER_TYPE || oodItem.type == "BracketedArgumentList")//|| oodFirstChildItem.type == "ObjectCreationExpression" || oodFirstChildItem.type == "ArrayType" || oodFirstChildItem.type == "CastExpression")
            {
                LinkExpressionVariables(oodFirstChildItem, project, parentIndex, ref writing);
            }
            else
            {
                IndexTuple index = project.ListAdd((OODMember)oodItem, parentIndex);

                // Store Index
                project.ListAddChildExpression(parentIndex, index, writing);
                writing = false;

                LinkExpressionVariables(oodFirstChildItem, project, index, ref writing);
            }
        }
        private static void AddIdentifierToExpression(OODItem oodItem, Project project, IndexTuple parentIndex, ref bool writing)
        {
            IndexTuple index = project.ListAdd((OODMember)oodItem, parentIndex);

            // Store Index
            project.ListAddChildExpression(parentIndex, index, writing);
            writing = false;
        }
        #endregion


    }
}
