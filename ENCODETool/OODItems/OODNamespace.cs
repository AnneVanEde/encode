using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    class OODNamespace : OODItem
    {
        public List<List<IndexTuple>> oodClasses;
        public OODNamespace(string _name, int _rootTreeNumber = -1) : base(_name, _rootTreeNumber)
        {
            type = Types.Namespace.ToString();
            oodClasses = new List<List<IndexTuple>>();
        }

        public override string GetKey()
        {
            return $"{type}_{name}";
        }

        public override List<IndexTuple> GetAllChildren(int parentIndex)
        {
            if (parentIndex == -1)
            {
                List<IndexTuple> output = new List<IndexTuple>();
                for (int i = 0; i < oodClasses.Count; i++)
                {
                    output.AddRange(oodClasses[i]);
                }
                return output;
            }
            return oodClasses[parentIndex];
        }

        public void AddDataFrom(OODNamespace oodAddItem, IndexTuple parentIndex)
        {
            if (rootTreeNumber == -1)
            {
                oodClasses.AddRange(oodAddItem.oodClasses);
            }

            base.AddDataFrom(oodAddItem, parentIndex);
        }
    }
}
