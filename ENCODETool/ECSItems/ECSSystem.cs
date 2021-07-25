using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    class ECSSystem : ECSItem
    {
        public List<IndexTuple> usedInDocClasses;
        public List<IndexTuple> ecsComponentReadMembers;
        public List<IndexTuple> ecsComponentWriteMembers;

        public HashSet<IndexTuple> runOnEntityTypes;
        public HashSet<IndexTuple> ecsReadComponents;
        public HashSet<IndexTuple> ecsWriteComponents;

        public string parentName;
        public ECSSystem(string _name = "", string _type = "", string _parentName = "") : base(_name, _type)
        {
            usedInDocClasses = new List<IndexTuple>();
            ecsComponentReadMembers = new List<IndexTuple>();
            ecsComponentWriteMembers = new List<IndexTuple>();

            runOnEntityTypes = new HashSet<IndexTuple>();
            ecsReadComponents = new HashSet<IndexTuple>();
            ecsWriteComponents = new HashSet<IndexTuple>();

            parentName = _parentName;
        }

        public override string GetLabel()
        {
            return $"[{parentName}] {variableName} ({variableType})";
        }

        public void AddDataFrom(ECSSystem ecsAddItem)
        {
            ecsComponentReadMembers.AddRange(ecsAddItem.ecsComponentReadMembers);
            ecsComponentWriteMembers.AddRange(ecsAddItem.ecsComponentWriteMembers);
            ecsReadComponents.UnionWith(ecsAddItem.ecsReadComponents);
            ecsWriteComponents.UnionWith(ecsAddItem.ecsWriteComponents);
        }
    }
}