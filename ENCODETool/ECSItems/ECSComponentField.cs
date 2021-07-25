using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    class ECSComponentField : ECSItem
    {

        public List<IndexTuple> usedInDocClasses;
        public List<IndexTuple> readInECSSystems;
        public List<IndexTuple> writenInECSSystems;
        public int blittable;

        public ECSComponentField(string _name, string _type) : base(_name, _type)
        {
            usedInDocClasses = new List<IndexTuple>();
            readInECSSystems = new List<IndexTuple>();
            writenInECSSystems = new List<IndexTuple>();
        }

        public void AddDataFrom(ECSComponentField ecsAddItem)
        {
            usedInDocClasses.AddRange(ecsAddItem.usedInDocClasses);
            readInECSSystems.AddRange(ecsAddItem.readInECSSystems);
            writenInECSSystems.AddRange(ecsAddItem.writenInECSSystems);
        }

    }
}
