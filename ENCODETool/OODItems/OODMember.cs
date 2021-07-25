using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    class OODMember : OODItem
    {
        public string parentName;
        public string namespaceName;
        public string variableType;
        public IndexTuple oodLinkIndex;

        public IndexTuple componentMemberIndex;

        public OODMember(string _name, string _type, int _rootTreeNumber = -1) : base(_name, _rootTreeNumber)
        {
            type = _type;
            namespaceName = "Global";
            oodLinkIndex = new IndexTuple();
            componentMemberIndex = new IndexTuple();
            variableType = "";
        }
        public override string GetKey()
        {
            if (type == Constant.ENUM_DECLARATION_TYPE)
                return $"{type}_{name}";
            return $"{type}_{parentName}_{name}";
        }

        public override string GetLabel()
        {
            return $"{parentName} : [{variableType}] {name}";
        }

        public override List<IndexTuple> GetAllChildren(int parentIndex)
        {
            return oodRawIndices;
        }

        public void AddDataFrom(OODMember oodAddItem, IndexTuple parentIndex)
        {
            if (rootTreeNumber == -1)
            {
                parentName = oodAddItem.parentName;
                variableType = oodAddItem.variableType;
            }

            parentIndices = parentIndices.Union(oodAddItem.parentIndices).ToList();

            if (rootTreeNumber == -1)
            {
                //rootTreeNumber = oodAddItem.rootTreeNumber;
                oodRawItems.AddRange(oodAddItem.oodRawItems);
                oodRawIndices.AddRange(oodAddItem.oodRawIndices);
            }
        }
    }
}
