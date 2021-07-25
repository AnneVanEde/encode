using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ENCODE.Base
{
    class OODClass : OODItem
    {
        public IndexTuple baseClassIndex;
        public string namespaceName;

        public HashSet<IndexTuple> groupIndices;
        public HashSet<IndexTuple> componentIndices;

        public List<IndexTuple> oodVariables;
        public List<IndexTuple> oodMethods;

        public ISymbol rawSymbol;

        public OODClass(string _name, ISymbol _rawSymbol, int _rootTreeNumber = -1) : base(_name, _rootTreeNumber)
        {
            type = Types.Class.ToString();

            groupIndices = new HashSet<IndexTuple>();
            componentIndices = new HashSet<IndexTuple>();

            oodVariables = new List<IndexTuple>();
            oodMethods = new List<IndexTuple>();

            namespaceName = "Global";

            rawSymbol = _rawSymbol;
        }

        public override string GetKey()
        {
            return $"{type}_{namespaceName}_{name}";
        }
        
        public override string GetLabel()
        {
            string namespaceString = namespaceName != "" ? $"{namespaceName} : " : "";
            return $"{namespaceString}{name}";
        }
        public override string GetParentLabel()
        {
            return $"{name}";
        }

        public override List<IndexTuple> GetAllChildren(int parentIndex)
        {
            return oodVariables.Concat(oodMethods).ToList();
        }

        public void AddDataFrom(OODClass oodAddItem, IndexTuple parentIndex)
        {
            if (rootTreeNumber == -1)
            {
                baseClassIndex = oodAddItem.baseClassIndex;
                oodVariables.AddRange(oodAddItem.oodVariables);
                oodMethods.AddRange(oodAddItem.oodMethods);
                groupIndices.UnionWith(oodAddItem.groupIndices);
                componentIndices.UnionWith(oodAddItem.componentIndices);
            }

            base.AddDataFrom(oodAddItem, parentIndex);
        }
    }
}
