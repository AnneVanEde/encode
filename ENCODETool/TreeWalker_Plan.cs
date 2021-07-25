using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    static partial class TreeWalker
    {

        public static void GatherComponentFields(Project project)
        {
            int totalClasses = project.oodClasses.Count;
            int writeDevision = (totalClasses / 10) + 1;

            // Go over all classes
            for (int classIndex = 0; classIndex < project.oodClasses.Count; classIndex++)
            {
                GatherComponentFields(project, new IndexTuple((int)Types.Class, classIndex));

                // Update the progress bar in the window
                int percentage = classIndex * 50 / totalClasses;
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (classIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Gathering Component Fields - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Gathering Component Fields - {percentage}% ");
                }
            }
            Logger.ReWrite($"Gathering Component Fields - 100%");
            Logger.WriteLine("");
            Logger.WriteLineDebug($"{project.ecsComponentFields.Count()} Component Fields");
        }

        private static void GatherComponentFields(Project project, IndexTuple classTuple)
        {
            OODClass oodClass = project.oodClasses[classTuple.itemIndex];

            // If it is a manually added class
            if (oodClass.rootTreeNumber < 0)
                return;

            foreach (IndexTuple variableIndex in oodClass.oodVariables)
            {
                // Add containing class to field
                OODMember oodVariable = project.oodMembers[variableIndex.itemIndex];

                // If it is a enum class
                if (oodVariable.rootTreeNumber < 0)
                    return;

                int blittable = IsBlittable(oodVariable.variableType);
                ECSComponentField ecsComponentField = new ECSComponentField(oodVariable.name, oodVariable.variableType) { blittable = blittable };
                ecsComponentField.usedInDocClasses.Add(classTuple);

                // If necessarry add variable to child classes of containing class
                AddChildClassesToVariable(project, classTuple, ecsComponentField);

                IndexTuple ecsIndexTuple = project.ListAdd(ecsComponentField);
                project.oodMembers[variableIndex.itemIndex].componentMemberIndex = ecsIndexTuple;
            }

        }

        private static int IsBlittable(string type)
        {
            string[] blittable = Constant.BLITTABLE_TYPES.Union(Program.planProfile.BlittableTypes).ToArray();
            string[] nonBlittable = Constant.NON_BLITTABLE_TYPES.Union(Program.planProfile.NonBlittableTypes).ToArray();

            // If there is a known non-blittable type present, return it is not blittable
            for (int i = 0; i < nonBlittable.Length; i++)
            {
                if (type.Contains(nonBlittable[i]))
                    return 0;
            }

            // Remove Array
            char[] splitChars = new char[] { '<', '>', '[', ']', ',' };
            string[] typeExploded = type.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

            // If there is a type that is not known in blittable or non-blittable
            for (int i = 0; i < typeExploded.Length; i++)
            {
                // return that it is unsure
                if (!blittable.Contains(typeExploded[i]))
                    return -1;
            }

            // If there is only a blittable type, return it is blittable
            return 1;
        }

        private static void AddChildClassesToVariable(Project project, IndexTuple classTuple, ECSComponentField ecsComponentField)
        {
            OODClass oodClass = project.oodClasses[classTuple.itemIndex];

            foreach (var groupIndex in oodClass.groupIndices)
            {
                if (project.GroupListGetParentIndex(groupIndex, out IndexTuple groupParentIndex) == 1 && groupParentIndex == classTuple)
                {
                    project.GroupListGetItems(groupIndex, out List<IndexTuple> groupMembers);

                    for (int index = 1; index < groupMembers.Count; index++)
                    {
                        IndexTuple groupMemberIndex = groupMembers[index];
                        ecsComponentField.usedInDocClasses.Add(groupMemberIndex);

                        AddChildClassesToVariable(project, groupMemberIndex, ecsComponentField);
                    }
                }
            }

        }


        public static void GatherSystems(Project project)
        {
            int totalMethods = project.oodMethods.Count;
            int writeDevision = (totalMethods / 10) + 1;

            // Go over all classes
            for (int methodIndex = 0; methodIndex < project.oodMethods.Count; methodIndex++)
            {
                GatherSystems(project, new IndexTuple((int)Types.Method, methodIndex));

                // Update the progress bar in the window
                int percentage = (methodIndex * 50 / totalMethods) + 50;
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (methodIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Gathering Systems - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Gathering Systems - {percentage}% ");
                }
            }
            Logger.ReWrite($"Gathering Systems - 100%");
            Logger.WriteLine("");
            Logger.WriteLineDebug($"{project.ecsSystems.Count()} Systems");
        }

        private static void GatherSystems(Project project, IndexTuple methodIndex)
        {
            OODMethod oodMethod = project.oodMethods[methodIndex.itemIndex];
            ECSSystem ecsSystem = new ECSSystem(oodMethod.name, oodMethod.parameters, oodMethod.parentName);

            IndexTuple ecsSystemIndexTuple = project.ListAdd(ecsSystem);
            project.AddCoupleToDictionary(methodIndex, ecsSystemIndexTuple);

            foreach (IndexTuple writeIndex in oodMethod.oodWriteGlobalVariables)
            {
                project.ListAddSystemToComponentField(writeIndex, ecsSystemIndexTuple, true);
            }

            foreach (IndexTuple readIndex in oodMethod.oodReadGlobalVariables)
            {
                project.ListAddSystemToComponentField(readIndex, ecsSystemIndexTuple, false);
            }
        }


        public static void CombiningComponentMembers(Project project)
        {
            int totalComponentMembers = project.ecsComponentFields.Count;
            int writeDevision = (totalComponentMembers / 10) + 1;

            for (int componentMemberIndex = 0; componentMemberIndex < totalComponentMembers; componentMemberIndex++)
            {
                ECSComponentField ecsComponentField = project.ecsComponentFields[componentMemberIndex];

                ECSComponent ecsComponent = new ECSComponent()
                {
                    usedInDocClasses = ecsComponentField.usedInDocClasses,
                    readInECSSystems = ecsComponentField.readInECSSystems,
                    writenInECSSystems = ecsComponentField.writenInECSSystems,
                    blittable = ecsComponentField.blittable
                };

                ecsComponent.ecsComponentFields.Add(new IndexTuple((int)Types.ComponentMember, componentMemberIndex));
                project.ListAdd(ecsComponent);

                // Update the progress bar in the window
                int percentage = componentMemberIndex * 50 / totalComponentMembers;
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (componentMemberIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Combining Component Fields - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Combining Component Fields - {percentage}% ");
                }
            }
            Logger.ReWrite($"Combining Component Fields - 100%");
            Logger.WriteLine("");
            Logger.WriteLineDebug($"{project.ecsComponents.Count()} Components");
        }


        public static void ConnectEntityTypes(Project project)
        {
            int totalClasses = project.oodClasses.Count;
            int writeDevision = (totalClasses / 10) + 1;

            for (int classIndex = 0; classIndex < totalClasses; classIndex++)
            {
                OODClass oodClass = project.oodClasses[classIndex];

                if (oodClass.componentIndices.Count == 0 || oodClass.rootTreeNumber == -1)
                    continue;

                ECSEntityType ecsEntityType = new ECSEntityType(oodClass.name, "", oodClass.namespaceName)
                {
                    ecsComponents = oodClass.componentIndices
                };

                IndexTuple ecsIndexTuple = project.ListAdd(ecsEntityType);

                project.AddCoupleToDictionary(new IndexTuple((int)Types.Class, classIndex), ecsIndexTuple);
                AddEntityTypeToSystems(ecsEntityType, ecsIndexTuple, project);

                // Update the progress bar in the window
                int percentage = (classIndex * 50 / totalClasses) + 50;
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (classIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Collecting Entity Types - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Collecting Entity Types - {percentage}% ");
                }
            }
            Logger.ReWrite($"Collecting Entity Types - 100%");
            Logger.WriteLine("");
            Logger.WriteLineDebug($"{project.ecsEntityTypes.Count()} Entity Types");

        }

        public static void GatherEntityTypes(Project project)
        {
            int totalClasses = project.oodClasses.Count;
            int writeDevision = (totalClasses / 10) + 1;

            for (int classIndex = 0; classIndex < totalClasses; classIndex++)
            {
                OODClass oodClass = project.oodClasses[classIndex];

                if (oodClass.componentIndices.Count == 0 || oodClass.rootTreeNumber == -1)
                    continue;

                ECSEntityType ecsEntityType = new ECSEntityType(oodClass.name, "", oodClass.namespaceName)
                {
                    ecsComponents = oodClass.componentIndices
                };

                IndexTuple ecsIndexTuple = project.ListAdd(ecsEntityType);

                project.AddCoupleToDictionary(new IndexTuple((int)Types.Class, classIndex), ecsIndexTuple);
                AddEntityTypeToSystems(ecsEntityType, ecsIndexTuple, project);

                // Update the progress bar in the window
                int percentage = (classIndex * 50 / totalClasses) + 50;
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (classIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Collecting Entity Types - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Collecting Entity Types - {percentage}% ");
                }
            }
            Logger.ReWrite($"Collecting Entity Types - 100%");
            Logger.WriteLine("");
            Logger.WriteLineDebug($"{project.ecsEntityTypes.Count()} Entity Types");

        }

        private static void AddEntityTypeToSystems(ECSEntityType ecsEntityType, IndexTuple entityIndex, Project project) // <-- seems opposite?
        {
            foreach (var componentIndex in ecsEntityType.ecsComponents)
            {
                List<IndexTuple> usedInSystemsList = Program.projectStructure.ecsComponents[componentIndex.itemIndex].readInECSSystems.Union(Program.projectStructure.ecsComponents[componentIndex.itemIndex].writenInECSSystems).ToList();

                // Entity is processed by the system
                project.ecsEntityTypes[entityIndex.itemIndex].usedInSystems.UnionWith(usedInSystemsList);

                foreach (var systemIndex in usedInSystemsList)
                {
                    project.ecsSystems[systemIndex.itemIndex].runOnEntityTypes.Add(entityIndex);
                }
            }

            //for (int systemIndex = 0; systemIndex < project.ecsSystems.Count; systemIndex++)
            //{
            //    ECSSystem ecsSystem = project.ecsSystems[systemIndex];
            //
            //    // If the system uses more components than the entity has, it cant fit
            //    if (ecsSystem.ecsReadComponents.Count > ecsEntityType.ecsComponents.Count || ecsSystem.ecsWriteComponents.Count > ecsEntityType.ecsComponents.Count)
            //        continue;
            //
            //    HashSet<IndexTuple> systemComponents = ecsSystem.ecsReadComponents.Union(ecsSystem.ecsWriteComponents).ToHashSet();
            //    HashSet<IndexTuple> entityComponents = ecsEntityType.ecsComponents;
            //
            //    // false if any component in system components is not present in entity components
            //    if (systemComponents.Count == 0)
            //        continue;
            //
            //    //bool allComponentsPresent = !systemComponents.Except(entityComponents).Any();
            //    //
            //    //if (allComponentsPresent)
            //    //{
            //    //    // Entity is processed by the system
            //    //    project.ecsSystems[systemIndex].runOnEntityTypes.Add(entityIndex);
            //    //    project.ecsEntityTypes[entityIndex.itemIndex].usedInSystems.Add(new IndexTuple((int)Types.Entity, systemIndex));
            //    //}
            //
            //    foreach (var componentIndex in systemComponents)
            //    {
            //        if (entityComponents.Contains(componentIndex))
            //        {
            //            // Entity is processed by the system
            //            project.ecsSystems[systemIndex].runOnEntityTypes.Add(entityIndex);
            //            project.ecsEntityTypes[entityIndex.itemIndex].usedInSystems.Add(new IndexTuple((int)Types.Entity, systemIndex));
            //            continue;
            //        }
            //    }
            //}
        }


        public static void SortComponentMembersInComponents(Project project)
        {
            int totalComponents = project.ecsComponents.Count;
            int writeDevision = (totalComponents / 10) + 1;

            for (int componentIndex = 0; componentIndex < totalComponents; componentIndex++)
            {
                ECSComponent ecsComponent = project.ecsComponents[componentIndex];
                SortedDictionary<string, IndexTuple> typeIndexDictionary = new SortedDictionary<string, IndexTuple>();

                for (int i = 0; i < ecsComponent.ecsComponentFields.Count; i++)
                {
                    IndexTuple componentMemberIndex = ecsComponent.ecsComponentFields[i];
                    ECSComponentField ecsComponentField = project.ecsComponentFields[componentMemberIndex.itemIndex];
                    typeIndexDictionary.Add($"{ecsComponentField.variableType}_{i}", componentMemberIndex);
                }

                project.ecsComponents[componentIndex].ecsComponentFields = typeIndexDictionary.Values.ToList();

                // Update the progress bar in the window
                int percentage = componentIndex * 100 / totalComponents;
                Program.menuForm.UpdateStepProgressBar(percentage);

                if (componentIndex % writeDevision == 0)
                {
                    Logger.ReWrite($"Sorting Components based on Field Types - {percentage}% ");
                    Logger.WriteLineToFileOnly($"Sorting Components based on Field Types - {percentage}% ");
                }
            }
            Logger.ReWrite($"Sorting Components based on Field Types - 100%");


        }

    }
}