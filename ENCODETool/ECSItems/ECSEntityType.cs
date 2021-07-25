using System;
using System.Collections.Generic;

namespace ENCODE.Base
{
    class ECSEntityType : ECSItem
    {
        public HashSet<IndexTuple> ecsComponents;
        public string namespaceName;

        public HashSet<IndexTuple> usedInSystems;

        public ECSEntityType(string _name = "", string _type = "", string _namespaceName = "") : base(_name, _type)
        {
            ecsComponents = new HashSet<IndexTuple>();
            namespaceName = _namespaceName;

            usedInSystems = new HashSet<IndexTuple>();
        }

        public override string GetLabel()
        {
            return variableName;
        }

    }
}
