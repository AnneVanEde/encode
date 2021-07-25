using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    class OODExpression : OODItem
    {
        public string parentName;
        public string namespaceName;
        public bool localDeclaration;
                
        public List<IndexTuple> oodReadVariables;
        public List<IndexTuple> oodWriteVariables;
        
        public List<IndexTuple> oodSubExpressions;
        public List<IndexTuple> oodSuperExpressions;

        public OODExpression(string _name, string _type, int _rootTreeNumber = -1) : base(_name, _rootTreeNumber)
        {
            type = _type;
            namespaceName = "Global";
            localDeclaration = false;

            oodReadVariables = new List<IndexTuple>();
            oodWriteVariables = new List<IndexTuple>();

            oodSubExpressions = new List<IndexTuple>();
            oodSuperExpressions = new List<IndexTuple>();
        }

        public override string GetKey()
        {
            return $"{type}_{parentName}_{name}";
        }
        
        public override string GetLabel()
        {
            return $"{parentName} : {name}";
        }

        public override List<IndexTuple> GetAllChildren(int parentIndex)
        {
            return oodSubExpressions.Concat(oodWriteVariables.Concat(oodReadVariables)).ToList();
        }
    }
}
