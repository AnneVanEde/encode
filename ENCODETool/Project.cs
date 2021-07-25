using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;


namespace ENCODE.Base
{
    class Project
    {
        // OOD Raw List
        public List<OODItem> oodRawItems;
        public Dictionary<string, IndexTuple> oodRawDictionary;
        public Dictionary<string, IndexTuple> ecsRawDictionary;
        public Dictionary<IndexTuple, IndexTuple> coupleDictionary; 
        public List<SemanticModel> semanticModels;

        // DOD Lists
        public List<OODFile> oodFiles;
        public List<OODNamespace> oodNamespaces;
        public List<OODClass> oodClasses;
        public List<OODMethod> oodMethods;
        public List<OODMember> oodMembers;
        public List<OODExpression> oodExpressions;

        public List<List<IndexTuple>> classGroups;
        public List<List<IndexTuple>> methodGroups;

        public List<ECSComponentField> ecsComponentFields;
        public List<ECSComponent> ecsComponents;
        public List<ECSSystem> ecsSystems;
        public List<ECSEntityType> ecsEntityTypes;

        public int noOfErrors;

        public Project()
        {
            oodRawItems = new List<OODItem>();
            oodRawDictionary = new Dictionary<string, IndexTuple>();
            ecsRawDictionary = new Dictionary<string, IndexTuple>();
            coupleDictionary = new Dictionary<IndexTuple, IndexTuple>();

            semanticModels = new List<SemanticModel>();

            oodFiles = new List<OODFile>();
            oodNamespaces = new List<OODNamespace>();
            oodClasses = new List<OODClass>();
            oodMethods = new List<OODMethod>();
            oodMembers = new List<OODMember>();
            oodExpressions = new List<OODExpression>();

            classGroups = new List<List<IndexTuple>>();
            methodGroups = new List<List<IndexTuple>>();

            ecsComponentFields = new List<ECSComponentField>();
            ecsComponents = new List<ECSComponent>();
            ecsSystems = new List<ECSSystem>();
            ecsEntityTypes = new List<ECSEntityType>();

            noOfErrors = 0;
        }

        #region Add To Project List

        public IndexTuple ListAdd(OODFile oodItem, IndexTuple parentIndex)
        {
            //if (oodRawDictionary.TryGetValue(oodItem.GetKey(), out IndexTuple indexTuple))
            //    return indexTuple;

            int arrayIndex = (int)Types.File;
            int itemIndex = oodFiles.Count();
            IndexTuple indexTuple = new IndexTuple(arrayIndex, itemIndex);

            oodFiles.Add(oodItem);
            oodRawDictionary.Add(oodItem.GetKey(), indexTuple);
            return indexTuple;
        }

        public IndexTuple ListAdd(OODNamespace oodItem, IndexTuple parentIndex)
        {
            if (parentIndex.IsValid())
                oodItem.parentIndices.Add(parentIndex);


            if (oodRawDictionary.TryGetValue(oodItem.GetKey(), out IndexTuple indexTuple))
            {
                DOCListGetItem(indexTuple, out OODItem oodExistingItem);
                oodExistingItem.AddDataFrom(oodItem, parentIndex);
                return indexTuple;
            }

            int arrayIndex = (int)Types.Namespace;
            int itemIndex = oodNamespaces.Count();
            indexTuple = new IndexTuple(arrayIndex, itemIndex);

            oodNamespaces.Add(oodItem);
            oodRawDictionary.Add(oodItem.GetKey(), indexTuple);

            return indexTuple;
        }

        public IndexTuple ListAdd(OODClass oodItem, IndexTuple parentIndex)
        {
            if (parentIndex.IsValid())
                oodItem.parentIndices.Add(parentIndex);

            if (oodRawDictionary.TryGetValue(oodItem.GetKey(), out IndexTuple indexTuple))
            {
                DOCListGetItem(indexTuple, out OODItem oodExistingItem);
                ((OODClass)oodExistingItem).AddDataFrom(oodItem, parentIndex);
                return indexTuple;
            }

            int arrayIndex = (int)Types.Class;
            int itemIndex = oodClasses.Count();
            indexTuple = new IndexTuple(arrayIndex, itemIndex);

            oodClasses.Add(oodItem);
            oodRawDictionary.Add(oodItem.GetKey(), indexTuple);

            return indexTuple;
        }

        public IndexTuple ListAdd(OODMethod oodItem, IndexTuple parentIndex)
        {
            if (parentIndex.IsValid())
            {
                oodItem.parentIndices.Add(parentIndex);
                oodItem.parentName = GetName(parentIndex);
                oodItem.namespaceName = GetNamespace(parentIndex);
            }

            if (oodRawDictionary.TryGetValue(oodItem.GetKey(), out IndexTuple indexTuple))
            {
                DOCListGetItem(indexTuple, out OODItem oodExistingItem);
                ((OODMethod)oodExistingItem).AddDataFrom(oodItem, parentIndex);
                return indexTuple;
            }

            int arrayIndex = (int)Types.Method;
            int itemIndex = oodMethods.Count();
            indexTuple = new IndexTuple(arrayIndex, itemIndex);

            oodMethods.Add(oodItem);

            // TODO: Keys not yet unique
            oodRawDictionary.Add(oodItem.GetKey(), indexTuple);

            return indexTuple;
        }

        public IndexTuple ListAdd(OODExpression oodItem, IndexTuple parentIndex)
        {
            if (parentIndex.IsValid())
            {
                oodItem.parentIndices.Add(parentIndex);
                oodItem.parentName = GetName(parentIndex);
                oodItem.namespaceName = GetNamespace(parentIndex);
            }

            int arrayIndex = (int)Types.Expression;
            int itemIndex = oodExpressions.Count();
            IndexTuple indexTuple = new IndexTuple(arrayIndex, itemIndex);

            oodExpressions.Add(oodItem);

            return indexTuple;
        }

        public IndexTuple ListAdd(OODMember oodItem, IndexTuple parentIndex)
        {
            if (parentIndex.IsValid())
            {
                oodItem.parentIndices.Add(parentIndex);
                oodItem.parentName = GetName(parentIndex);
                oodItem.namespaceName = GetNamespace(parentIndex);
            }

            if (oodRawDictionary.TryGetValue(oodItem.GetKey(), out IndexTuple indexTuple))
            {
                DOCListGetItem(indexTuple, out OODItem oodExistingItem);
                oodExistingItem.AddDataFrom(oodItem, parentIndex);
                return indexTuple;
            }


            int arrayIndex = (int)Types.Member;
            int itemIndex = oodMembers.Count();
            indexTuple = new IndexTuple(arrayIndex, itemIndex);

            oodMembers.Add(oodItem);
            oodRawDictionary.Add(oodItem.GetKey(), indexTuple);

            return indexTuple;
        }

        public IndexTuple ListAdd(ECSComponentField ecsComponentField)
        {
            if (ecsRawDictionary.TryGetValue(ecsComponentField.GetKey(), out IndexTuple indexTuple))
            {
                DOCListGetItem(indexTuple, out ECSItem ecsExistingItem);
                ((ECSComponentField)ecsExistingItem).AddDataFrom(ecsComponentField);
                return indexTuple;
            }

            int arrayIndex = (int)Types.ComponentMember;
            int itemIndex = ecsComponentFields.Count();
            indexTuple = new IndexTuple(arrayIndex, itemIndex);

            ecsComponentFields.Add(ecsComponentField);
            ecsRawDictionary.Add(ecsComponentField.GetKey(), indexTuple);

            return indexTuple;
        }

        public IndexTuple ListAdd(ECSComponent ecsNewComponent)
        {
            for (int i = 0; i < ecsComponents.Count; i++)
            {
                ECSComponent ecsExistingComponent = ecsComponents[i];

                // -- Checks to see if they are similar enough in classes to merge --
                if (!MergeBasedOnClasses(ecsExistingComponent, ecsNewComponent))
                    continue;

                // -- Checks to see if they are similar enough in systems to merge --
                if (!MergeBasedOnSystems(ecsExistingComponent, ecsNewComponent))
                    continue;

                if(SplitBasedOnBlittable(ecsExistingComponent, ecsNewComponent))
                    continue;

                // Merge to single component
                ecsExistingComponent.AddDataFrom(ecsNewComponent);
                return new IndexTuple((int)Types.Component, i);

            }

            // If there was no match, add as new
            int arrayIndex = (int)Types.Component;
            int itemIndex = ecsComponents.Count();
            IndexTuple indexTuple = new IndexTuple(arrayIndex, itemIndex);

            ecsNewComponent.index = indexTuple.itemIndex;

            foreach (var usedIn in ecsNewComponent.usedInDocClasses)
            {
                ecsNewComponent.usedInClassLabel += $"[{GetName(usedIn)}] ";
                oodClasses[usedIn.itemIndex].componentIndices.Add(indexTuple);
            }

            foreach (var usedIn in ecsNewComponent.readInECSSystems)
            {
                ecsNewComponent.usedInSystemLabel += $"[{GetName(usedIn)}] ";
                ecsSystems[usedIn.itemIndex].ecsReadComponents.Add(indexTuple);
            }

            foreach (var usedIn in ecsNewComponent.writenInECSSystems)
            {
                ecsNewComponent.usedInSystemLabel += $"[{GetName(usedIn)}] ";
                ecsSystems[usedIn.itemIndex].ecsWriteComponents.Add(indexTuple);
            }

            ecsComponents.Add(ecsNewComponent);

            return indexTuple;
        }

        private bool IsNormalized(float param)
        {
            return param >= 0f && param <= 1f;
        }

        public IndexTuple ListAdd(ECSSystem ecsSytem)
        {
            int arrayIndex = (int)Types.System;
            int itemIndex = ecsSystems.Count();
            IndexTuple indexTuple = new IndexTuple(arrayIndex, itemIndex);

            ecsSystems.Add(ecsSytem);
            return indexTuple;
        }

        public IndexTuple ListAdd(ECSEntityType ecsEntityType)
        {
            int arrayIndex = (int)Types.Entity;
            int itemIndex = ecsEntityTypes.Count();
            IndexTuple indexTuple = new IndexTuple(arrayIndex, itemIndex);

            ecsEntityTypes.Add(ecsEntityType);
            return indexTuple;
        }

        private bool MergeBasedOnClasses(ECSComponent ecsExistingComponent, ECSComponent ecsNewComponent)
        {
            // If used by the same classes
            float classEquality = EqualityDegree(ecsExistingComponent.usedInDocClasses, ecsNewComponent.usedInDocClasses);

            // If they are in range (-1 is single field components) and have enough similarities
            bool sameClasses = IsNormalized(classEquality) && IsNormalized(Program.planProfile.MergeOnClassEqualityLevel) &&
                classEquality >= Program.planProfile.MergeOnClassEqualityLevel;
            
            return sameClasses;
        }


        private bool MergeBasedOnSystems(ECSComponent ecsExistingComponent, ECSComponent ecsNewComponent)
        {
            // If they are in range (-1 is single field components) and have enough similarities
            bool sameSystems = IsNormalized(Program.planProfile.MergeOnSystemEqualityLevel);

            // Early out
            if (!sameSystems)
                return false;

            // If read access and write access should be split into different components or not
            if (Program.planProfile.SplitOnReadWriteAccess)
            {
                float systemWriteEquality = EqualityDegree(ecsExistingComponent.writenInECSSystems, ecsNewComponent.writenInECSSystems);
                float systemReadEquality = EqualityDegree(ecsExistingComponent.readInECSSystems, ecsNewComponent.readInECSSystems);

                sameSystems &= IsNormalized(systemWriteEquality) && IsNormalized(systemReadEquality) &&
                systemWriteEquality >= Program.planProfile.MergeOnSystemEqualityLevel && systemReadEquality >= Program.planProfile.MergeOnSystemEqualityLevel;

            }
            else
            {
                float systemEquality = EqualityDegree(ecsExistingComponent.readInECSSystems.Union(ecsExistingComponent.writenInECSSystems).ToList(),
                    ecsNewComponent.readInECSSystems.Union(ecsNewComponent.writenInECSSystems).ToList());

                sameSystems &= IsNormalized(systemEquality) && systemEquality >= Program.planProfile.MergeOnSystemEqualityLevel;
            }

            return sameSystems;
        }


        private bool SplitBasedOnBlittable(ECSComponent ecsExistingComponent, ECSComponent ecsNewComponent)
        {
            // If don't care, merge
            if (!Program.planProfile.SplitOnBlittable)
                return false;

            // One is unknown, merge
            if (ecsExistingComponent.blittable == -1 || ecsNewComponent.blittable == -1)
                return false;

            // Merge if both are blittable or both nonblittable;
            return ecsExistingComponent.blittable != ecsNewComponent.blittable;
        }


        public float EqualityDegree(List<IndexTuple> list1, List<IndexTuple> list2)
        {
            int pairs = 0;
            int nonPairs = list1.Count + list2.Count;

            if (nonPairs == 0)
                return 1.0f;

            foreach (IndexTuple list1Tuple in list1)
            {
                if (list2.Contains(list1Tuple))
                {
                    nonPairs -= 2;
                    pairs++;
                }
            }

            return ((float)pairs / (float)(pairs + nonPairs));
        }

        #endregion

        #region Add Child to Parent List

        public int ListAddChild(IndexTuple indexTuple, List<IndexTuple> childIndexTuples)
        {

            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodNamespaces.Count())
            {
                oodNamespaces[indexTuple.itemIndex].oodClasses.Add(childIndexTuples);
                return 1;
            }
            return 0;
        }

        public int ListAddChild(IndexTuple indexTuple, IndexTuple childIndexTuple)
        {
            switch (indexTuple.arrayIndex)
            {
                case (int)Types.File:
                    return ListAddChildFile(indexTuple, childIndexTuple);
                case (int)Types.Namespace:
                    return ListAddChildNamespace(indexTuple, childIndexTuple);
                case (int)Types.Class:
                    return ListAddChildClass(indexTuple, childIndexTuple);
                case (int)Types.Method:
                    return ListAddChildMethod(indexTuple, childIndexTuple);
                case (int)Types.Expression:
                    return ListAddChildMethod(indexTuple, childIndexTuple);
                case (int)Types.Member:
                    return ListAddChildField(indexTuple, childIndexTuple);
                default:
                    return 0;
            };
        }

        private int ListAddChildFile(IndexTuple indexTuple, IndexTuple childIndexTuple)
        {
            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodFiles.Count())
            {
                oodFiles[indexTuple.itemIndex].oodRawIndices.Add(childIndexTuple);

                switch (childIndexTuple.arrayIndex)
                {
                    case (int)Types.Namespace:
                        oodFiles[indexTuple.itemIndex].oodNamespaces.Add(childIndexTuple);
                        return 1;
                    case (int)Types.Class:
                        oodFiles[indexTuple.itemIndex].oodClasses.Add(childIndexTuple);
                        return 1;
                    case (int)Types.Member:
                        oodFiles[indexTuple.itemIndex].oodUsing.Add(childIndexTuple);
                        string[] ignoreList = oodMembers[childIndexTuple.itemIndex].variableType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var ignoreItem in ignoreList)
                            oodFiles[indexTuple.itemIndex].ignoreList.Add(ignoreItem);
                        return 1;
                    default:
                        return 0;
                };
            }

            return 0;
        }

        private int ListAddChildNamespace(IndexTuple indexTuple, IndexTuple childIndexTuple)
        {
            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodNamespaces.Count())
            {
                if (childIndexTuple.arrayIndex == (int)Types.Member && oodMembers[childIndexTuple.itemIndex].type == Constant.USING_TYPE)
                {
                    IndexTuple parentIndex = oodNamespaces[indexTuple.itemIndex].parentIndices.LastOrDefault();
                    oodFiles[parentIndex.itemIndex].oodUsing.Add(childIndexTuple);

                    string[] ignoreList = oodMembers[childIndexTuple.itemIndex].variableType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ignoreItem in ignoreList)
                        oodFiles[parentIndex.itemIndex].ignoreList.Add(ignoreItem);
                    return 1;
                }
                oodNamespaces[indexTuple.itemIndex].oodRawIndices.Add(childIndexTuple);
            }
            return 0;
        }

        private int ListAddChildClass(IndexTuple indexTuple, IndexTuple childIndexTuple)
        {
            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodClasses.Count())
            {
                oodClasses[indexTuple.itemIndex].oodRawIndices.Add(childIndexTuple);

                switch (childIndexTuple.arrayIndex)
                {
                    case (int)Types.Method:
                        oodClasses[indexTuple.itemIndex].oodMethods.Add(childIndexTuple);
                        return 1;
                    case (int)Types.Member:
                        oodClasses[indexTuple.itemIndex].oodVariables.Add(childIndexTuple);
                        return 1;
                    default:
                        return 0;
                };
            }
            return 0;
        }

        private int ListAddChildMethod(IndexTuple indexTuple, IndexTuple childIndexTuple)
        {
            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMethods.Count())
            {
                oodMethods[indexTuple.itemIndex].oodRawIndices.Add(childIndexTuple);

                switch (childIndexTuple.arrayIndex)
                {
                    case (int)Types.Member:
                        //oodMethods[indexTuple.itemIndex].oodParameters.Add(childIndexTuple);
                        return 1;
                    case (int)Types.Expression:
                        oodMethods[indexTuple.itemIndex].oodExpressions.Add(childIndexTuple);
                        return 1;
                    default:
                        return 0;
                };
            }
            return 0;
        }

        public int ListAddChildExpression(IndexTuple indexTuple, IndexTuple childIndexTuple, bool writeOrSuper)
        {
            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodExpressions.Count())
            {
                oodExpressions[indexTuple.itemIndex].oodRawIndices.Add(childIndexTuple);

                switch (childIndexTuple.arrayIndex)
                {
                    case (int)Types.Member:
                        if (writeOrSuper)
                            // Add the write variable reference
                            oodExpressions[indexTuple.itemIndex].oodWriteVariables.Add(childIndexTuple);
                        else
                            // Add the read variable reference
                            oodExpressions[indexTuple.itemIndex].oodReadVariables.Add(childIndexTuple);

                        return 1;
                    case (int)Types.Expression:
                        if (writeOrSuper)
                            // Add the write variable reference
                            oodExpressions[indexTuple.itemIndex].oodSubExpressions.Add(childIndexTuple);
                        else
                            // Add the read variable reference
                            oodExpressions[indexTuple.itemIndex].oodSuperExpressions.Add(childIndexTuple);
                        return 1;
                    default:
                        return 0;
                };
            }
            return 0;
        }

        public int ListAddSystemToComponentField(IndexTuple fieldIndex, IndexTuple systemIndex, bool write)
        {
            if (!(fieldIndex.itemIndex >= 0 && fieldIndex.itemIndex < oodMembers.Count() &&
                systemIndex.itemIndex >= 0 && systemIndex.itemIndex < ecsSystems.Count()))
                return 0;

            IndexTuple componentMemberIndex = oodMembers[fieldIndex.itemIndex].componentMemberIndex;

            if (componentMemberIndex.IsValid() && componentMemberIndex.itemIndex >= 0 && componentMemberIndex.itemIndex < ecsComponentFields.Count())
            {
                if (write)
                {
                    if (!ecsComponentFields[componentMemberIndex.itemIndex].writenInECSSystems.Contains(systemIndex))
                        ecsComponentFields[componentMemberIndex.itemIndex].writenInECSSystems.Add(systemIndex);

                    if (!ecsSystems[systemIndex.itemIndex].ecsComponentWriteMembers.Contains(componentMemberIndex))
                        ecsSystems[systemIndex.itemIndex].ecsComponentWriteMembers.Add(componentMemberIndex);
                    return 1;
                }
                else
                {
                    if (!ecsComponentFields[componentMemberIndex.itemIndex].readInECSSystems.Contains(systemIndex))
                        ecsComponentFields[componentMemberIndex.itemIndex].readInECSSystems.Add(systemIndex);

                    if (!ecsSystems[systemIndex.itemIndex].ecsComponentReadMembers.Contains(componentMemberIndex))
                        ecsSystems[systemIndex.itemIndex].ecsComponentReadMembers.Add(componentMemberIndex);
                    return 1;
                }

            }
            return 0;
        }

        private int ListAddChildField(IndexTuple indexTuple, IndexTuple childIndexTuple)
        {
            if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMembers.Count())
            {
                oodMembers[indexTuple.itemIndex].oodRawIndices.Add(childIndexTuple);
                return 1;
            }
            return 0;
        }

        #endregion

        #region Add Groups to Project List

        public IndexTuple AddToClassGroup(IndexTuple parentClass, IndexTuple childClass)
        {
            string groupKey = $"{Types.ClassGroup}_{oodClasses[parentClass.itemIndex].GetKey()}";

            // If group exists
            if (oodRawDictionary.TryGetValue(groupKey, out IndexTuple indexTuple))
            {
                if (!classGroups[indexTuple.itemIndex].Contains(childClass))
                {
                    classGroups[indexTuple.itemIndex].Add(childClass);
                    return indexTuple;
                }
                else
                {
                    return indexTuple;
                }
            }

            // Group does not exist: create
            int groupNumber = classGroups.Count;
            indexTuple = new IndexTuple((int)Types.ClassGroup, groupNumber);

            classGroups.Add(new List<IndexTuple> { parentClass, childClass });
            oodRawDictionary.Add(groupKey, indexTuple);
            return indexTuple;
        }

        public IndexTuple AddToMethodGroup(IndexTuple parentMethod, IndexTuple childMethod)
        {
            string groupKey = $"{Types.MethodGroup}_{oodMethods[parentMethod.itemIndex].name}";

            // If group exists
            if (oodRawDictionary.TryGetValue(groupKey, out IndexTuple indexTuple))
            {
                methodGroups[indexTuple.itemIndex].Add(childMethod);
                return indexTuple;
            }

            // Group does not exist: create
            int groupNumber = methodGroups.Count;
            indexTuple = new IndexTuple((int)Types.MethodGroup, groupNumber);

            methodGroups.Add(new List<IndexTuple> { parentMethod, childMethod });
            oodRawDictionary.Add(groupKey, indexTuple);

            return indexTuple;
        }

        #endregion

        #region Getters
        public int DOCListGetItem(IndexTuple indexTuple, out OODItem oodItem)
        {
            oodItem = null;

            switch (indexTuple.arrayIndex)
            {
                case (int)Types.File:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodFiles.Count())
                    {
                        oodItem = oodFiles[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.Namespace:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodNamespaces.Count())
                    {
                        oodItem = oodNamespaces[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.Class:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodClasses.Count())
                    {
                        oodItem = oodClasses[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.Method:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMethods.Count())
                    {
                        oodItem = oodMethods[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.Member:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMembers.Count())
                    {
                        oodItem = oodMembers[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.ClassGroup:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < classGroups.Count())
                    {
                        oodItem = oodClasses[classGroups[indexTuple.itemIndex][0].itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.MethodGroup:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < methodGroups.Count())
                    {
                        oodItem = oodMethods[methodGroups[indexTuple.itemIndex][0].itemIndex];
                        return 1;
                    }
                    break;
                default:
                    return -1;
            }

            return -1;
        }

        public int DOCListGetItem(IndexTuple indexTuple, out ECSItem ecsItem)
        {
            ecsItem = null;

            switch (indexTuple.arrayIndex)
            {
                case (int)Types.Component:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < ecsComponents.Count())
                    {
                        ecsItem = ecsComponents[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.ComponentMember:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < ecsComponentFields.Count())
                    {
                        ecsItem = ecsComponentFields[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.System:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < ecsSystems.Count())
                    {
                        ecsItem = ecsSystems[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                case (int)Types.Entity:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < ecsEntityTypes.Count())
                    {
                        ecsItem = ecsEntityTypes[indexTuple.itemIndex];
                        return 1;
                    }
                    break;
                default:
                    return -1;
            }

            return -1;
        }

        public int DOCListGetItem(IndexTuple indexTuple, IndexTuple parentIndexTuple, out OODItem oodItem)
        {
            oodItem = null;

            switch (indexTuple.arrayIndex)
            {
                case (int)Types.File:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodFiles.Count())
                    {
                        oodItem = oodFiles[indexTuple.itemIndex];
                        return GetParentIndex(oodItem, parentIndexTuple);
                    }
                    break;
                case (int)Types.Namespace:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodNamespaces.Count())
                    {
                        oodItem = oodNamespaces[indexTuple.itemIndex];
                        return GetParentIndex(oodItem, parentIndexTuple);
                    }
                    break;
                case (int)Types.Class:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodClasses.Count())
                    {
                        oodItem = oodClasses[indexTuple.itemIndex];
                        return GetParentIndex(oodItem, parentIndexTuple);
                    }
                    break;
                case (int)Types.Method:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMethods.Count())
                    {
                        oodItem = oodMethods[indexTuple.itemIndex];
                        return GetParentIndex(oodItem, parentIndexTuple);
                    }
                    break;
                case (int)Types.Expression:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodExpressions.Count())
                    {
                        oodItem = oodExpressions[indexTuple.itemIndex];
                        return GetParentIndex(oodItem, parentIndexTuple);
                    }
                    break;
                case (int)Types.Member:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMembers.Count())
                    {
                        oodItem = oodMembers[indexTuple.itemIndex];
                        return GetParentIndex(oodItem, parentIndexTuple);
                    }
                    break;
                default:
                    return -1;
            }

            return -1;
        }

        public int GroupListGetParentIndex(IndexTuple groupIndexTuple, out IndexTuple parentIndexTuple)
        {
            if (groupIndexTuple.arrayIndex == (int)Types.ClassGroup && (groupIndexTuple.itemIndex >= 0 && groupIndexTuple.itemIndex < classGroups.Count()))
            {
                parentIndexTuple = classGroups[groupIndexTuple.itemIndex][0];
                return 1;
            }
            else if (groupIndexTuple.arrayIndex == (int)Types.MethodGroup && (groupIndexTuple.itemIndex >= 0 && groupIndexTuple.itemIndex < methodGroups.Count()))
            {
                parentIndexTuple = methodGroups[groupIndexTuple.itemIndex][0];
                return 1;
            }
            parentIndexTuple = new IndexTuple();
            return -1;

        }

        public int GroupListGetItems(IndexTuple groupIndexTuple, out List<IndexTuple> groupMembers)
        {
            if (groupIndexTuple.arrayIndex == (int)Types.ClassGroup && (groupIndexTuple.itemIndex >= 0 && groupIndexTuple.itemIndex < classGroups.Count()))
            {
                groupMembers = classGroups[groupIndexTuple.itemIndex];
                return 1;
            }
            else if (groupIndexTuple.arrayIndex == (int)Types.MethodGroup && (groupIndexTuple.itemIndex >= 0 && groupIndexTuple.itemIndex < methodGroups.Count()))
            {
                groupMembers = methodGroups[groupIndexTuple.itemIndex];
                return 1;
            }
            groupMembers = new List<IndexTuple>();
            return -1;
        }



        private int GetParentIndex(OODItem oodItem, IndexTuple parentIndexTuple)
        {
            for (int i = 0; i < oodItem.parentIndices.Count(); i++)
            {
                if (oodItem.parentIndices[i] == parentIndexTuple)
                    return i;
            }
            return -1;
        }

        public IndexTuple GetIndex(OODItem oodItem)
        {
            if (oodRawDictionary.TryGetValue(oodItem.GetKey(), out IndexTuple indexTuple))
                return indexTuple;
            return new IndexTuple();
        }

        private string GetName(IndexTuple indexTuple)
        {
            switch (indexTuple.arrayIndex)
            {
                case (int)Types.File:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodFiles.Count())
                    {
                        return oodFiles[indexTuple.itemIndex].GetParentLabel();
                    }
                    break;
                case (int)Types.Namespace:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodNamespaces.Count())
                    {
                        return oodNamespaces[indexTuple.itemIndex].GetParentLabel();
                    }
                    break;
                case (int)Types.Class:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodClasses.Count())
                    {
                        return oodClasses[indexTuple.itemIndex].GetParentLabel();
                    }
                    break;
                case (int)Types.Method:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMethods.Count())
                    {
                        return oodMethods[indexTuple.itemIndex].GetParentLabel();
                    }
                    break;
                case (int)Types.Expression:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodExpressions.Count())
                    {
                        return oodExpressions[indexTuple.itemIndex].GetParentLabel();
                    }
                    break;
                case (int)Types.Member:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMembers.Count())
                    {
                        return oodMembers[indexTuple.itemIndex].GetParentLabel();
                    }
                    break;
                case (int)Types.System:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < ecsSystems.Count())
                    {
                        return ecsSystems[indexTuple.itemIndex].GetLabel();
                    }
                    break;
            }

            return "";
        }


        private string GetNamespace(IndexTuple indexTuple)
        {
            switch (indexTuple.arrayIndex)
            {
                //case (int)ArrayIndex.DOCFile:
                //    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodFiles.Count())
                //    {
                //        return oodFiles[indexTuple.itemIndex].GetParentLabel();
                //    }
                //    break;
                //case (int)ArrayIndex.DOCNamespace:
                //    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodNamespaces.Count())
                //    {
                //        return oodNamespaces[indexTuple.itemIndex].name;
                //    }
                //    break;
                case (int)Types.Class:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodClasses.Count())
                    {
                        return oodClasses[indexTuple.itemIndex].namespaceName;
                    }
                    break;
                case (int)Types.Method:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMethods.Count())
                    {
                        return oodMethods[indexTuple.itemIndex].namespaceName;
                    }
                    break;
                case (int)Types.Expression:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodExpressions.Count())
                    {
                        return oodExpressions[indexTuple.itemIndex].namespaceName;
                    }
                    break;
                case (int)Types.Member:
                    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < oodMembers.Count())
                    {
                        return oodMembers[indexTuple.itemIndex].namespaceName;
                    }
                    break;
                    //case (int)ArrayIndex.ECSSystem:
                    //    if (indexTuple.itemIndex >= 0 && indexTuple.itemIndex < ecsSystems.Count())
                    //    {
                    //        return ecsSystems[indexTuple.itemIndex].GetLabel();
                    //    }
                    //    break;
            }

            return "";
        }

        #endregion

        public void AddCoupleToDictionary(IndexTuple indexOne, IndexTuple indexTwo)
        {
            coupleDictionary.Add(indexOne, indexTwo);
            coupleDictionary.Add(indexTwo, indexOne);
        }

        public IndexTuple GetCoupleFromDictionary(IndexTuple index)
        {
            if (coupleDictionary.ContainsKey(index))
                return coupleDictionary[index];

            return new IndexTuple();
        }

        public bool ResetPlanningData()
        {
            coupleDictionary.Clear();
            ecsRawDictionary.Clear();
            ecsSystems.Clear();
            ecsEntityTypes.Clear();
            ecsComponents.Clear();
            ecsComponentFields.Clear();

            foreach (var oodClass in oodClasses)
            {
                oodClass.componentIndices.Clear();
            }

            return true;
        }
    }


}
