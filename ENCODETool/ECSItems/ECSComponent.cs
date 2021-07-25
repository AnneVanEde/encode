using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    class ECSComponent : ECSItem
    {
        public List<IndexTuple> ecsComponentFields;

        public List<IndexTuple> usedInDocClasses;
        public List<IndexTuple> readInECSSystems;
        public List<IndexTuple> writenInECSSystems;

        public int index;

        public string usedInClassLabel;
        public string usedInSystemLabel;

        public int blittable;
        public ECSComponent(string _name = "", string _type = "") : base(_name, _type)
        {
            usedInDocClasses = new List<IndexTuple>();
            readInECSSystems = new List<IndexTuple>();
            writenInECSSystems = new List<IndexTuple>();

            ecsComponentFields = new List<IndexTuple>();

            usedInClassLabel = "";
            usedInSystemLabel = "";
        }

        public override string GetKey()
        {
            return $"ecs_{index}_{variableType}_{variableName}";
        }

        public override string GetLabel()
        {
            
            return $"[{ecsComponentFields.Count} Variable(s)] [{usedInDocClasses.Count} Class(es)] {usedInClassLabel} [{readInECSSystems.Union(writenInECSSystems).Count()} System(s)] {usedInSystemLabel}";
        }

        public void AddDataFrom(ECSComponent ecsAddItem)
        {
            usedInDocClasses = usedInDocClasses.Union(ecsAddItem.usedInDocClasses).ToList();
            readInECSSystems = readInECSSystems.Union(ecsAddItem.readInECSSystems).ToList();
            writenInECSSystems = writenInECSSystems.Union(ecsAddItem.writenInECSSystems).ToList();
            ecsComponentFields = ecsComponentFields.Union(ecsAddItem.ecsComponentFields).ToList();
        }
    }
}
