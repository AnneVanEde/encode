using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ENCODE.Base
{
    class OODMethod : OODItem
    {
        public string parameters;
        public string parentName;
        public string namespaceName;
        public List<IndexTuple> groupIndex;

        public List<IndexTuple> oodExpressions;

        public List<IndexTuple> oodParameters;
        public List<IndexTuple> oodLocalVariables;
        public List<IndexTuple> oodReadGlobalVariables;
        public List<IndexTuple> oodWriteGlobalVariables;

        public ISymbol rawSymbol;
        public OODMethod(string _name, ISymbol _rawSymbol, int _rootTreeNumber = -1) : base(_name, _rootTreeNumber)
        {
            type = Types.Method.ToString();
            parameters = "";
            namespaceName = "Global";
            groupIndex = new List<IndexTuple>();
            rawSymbol = _rawSymbol;

            oodExpressions = new List<IndexTuple>();

            oodParameters = new List<IndexTuple>();
            oodLocalVariables = new List<IndexTuple>();
            oodReadGlobalVariables = new List<IndexTuple>();
            oodWriteGlobalVariables = new List<IndexTuple>();
        }

        public override string GetKey() 
        {
            return $"{type}_{namespaceName}_{parentName}_{name}_{parameters}";
        }

        public override string GetLabel()
        {
            return $"{namespaceName} {parentName} : {name} ({parameters})";
        }

        public override string GetParentLabel()
        {
            return $"{name} ({parameters})";
        }

        public override List<IndexTuple> GetAllChildren(int parentIndex)
        {
            return oodParameters.Concat(oodExpressions).ToList();
        }

        public List<IndexTuple> GetAllVariables()
        {
            return oodParameters.Union(oodLocalVariables)
                .Union(oodReadGlobalVariables)
                .Union(oodWriteGlobalVariables).ToList();
        }

        public void AddDataFrom(OODMethod oodAddItem, IndexTuple parentIndex)
        {
            if (rootTreeNumber == -1)
            {
                oodParameters = oodAddItem.oodParameters;
                parameters += oodAddItem.parameters;

                oodExpressions = oodAddItem.oodExpressions;
                oodReadGlobalVariables = oodAddItem.oodReadGlobalVariables;
                oodWriteGlobalVariables = oodAddItem.oodWriteGlobalVariables;

                groupIndex.AddRange(oodAddItem.groupIndex);
                parentName = oodAddItem.parentName;
            }

            base.AddDataFrom(oodAddItem, parentIndex);
        }
    }
}
