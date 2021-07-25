using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    class OODFile : OODItem
    {
        public List<IndexTuple> oodNamespaces;
        public List<IndexTuple> oodUsing;
        public List<IndexTuple> oodClasses;
        public HashSet<string> ignoreList;

        public OODFile(string _name, int _rootTreeNumber = -1) : base(_name, _rootTreeNumber)
        {
            type = Types.File.ToString();

            oodUsing = new List<IndexTuple>();
            oodNamespaces = new List<IndexTuple>();
            oodClasses = new List<IndexTuple>();

            ignoreList = new HashSet<string>();
        }

        public override string GetKey()
        {
            return $"{type}_{name}";
        }

        public override List<IndexTuple> GetAllChildren(int parentIndex)
        {
            return oodUsing.Concat(oodNamespaces)
                           .Concat(oodClasses)
                           .ToList();
        }

        public void AddDataFrom(OODFile oodAddItem, IndexTuple parentIndex)
        {
            if (rootTreeNumber == -1)
            {
                oodNamespaces .AddRange( oodAddItem.oodNamespaces);
                oodUsing.AddRange(oodAddItem.oodUsing);
                oodClasses.AddRange(oodAddItem.oodClasses);
            }

            base.AddDataFrom(oodAddItem, parentIndex);
        }
    }
}
