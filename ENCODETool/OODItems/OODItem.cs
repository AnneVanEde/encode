using System;
using System.Collections.Generic;
using System.Linq;

namespace ENCODE.Base
{
    abstract class OODItem
    {
        public List<IndexTuple> parentIndices;

        public string name;
        public string type;
        public int rootTreeNumber;

        public List<OODItem> oodRawItems;
        public List<IndexTuple> oodRawIndices;
        public OODItem(string _name, int _rootTreeNumber = -1)
        {
            name = _name;
            rootTreeNumber = _rootTreeNumber;
            oodRawItems = new List<OODItem>();

            parentIndices = new List<IndexTuple>();
            oodRawIndices = new List<IndexTuple>();
        }

        public abstract string GetKey();

        public virtual string GetLabel()
        {
            return $"{name}";
        }

        public virtual string GetParentLabel()
        {
            return $"{name}";
        }

        public abstract List<IndexTuple> GetAllChildren(int parentIndex);

        public virtual void AddDataFrom(OODItem oodAddItem, IndexTuple parentIndex)
        {
            parentIndices = parentIndices.Union(oodAddItem.parentIndices).ToList();

            if (rootTreeNumber == -1)
            {
                rootTreeNumber = oodAddItem.rootTreeNumber;
                oodRawItems.AddRange(oodAddItem.oodRawItems);
                oodRawIndices.AddRange(oodAddItem.oodRawIndices);
            }
        }
    }

    public class IndexTuple
    {
        public int arrayIndex;
        public int itemIndex;

        public IndexTuple(int _arrayIndex = -1, int _itemIndex = -1)
        {
            arrayIndex = _arrayIndex;
            itemIndex = _itemIndex;
        }

        public static bool operator ==(IndexTuple t1, IndexTuple t2)
        {
            return (t1.arrayIndex == t2.arrayIndex && t1.itemIndex == t2.itemIndex);
        }
        public static bool operator !=(IndexTuple t1, IndexTuple t2)
        {
            return !(t1.arrayIndex == t2.arrayIndex && t1.itemIndex == t2.itemIndex);
        }

        public bool IsValid()
        {
            if (arrayIndex != -1 && itemIndex != -1)
                return true;
            return false;
        }

        public override string ToString()
        {
            return $"({arrayIndex}, {itemIndex})";
        }

        public override int GetHashCode()
        {
            return arrayIndex.GetHashCode() ^ itemIndex.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            return this == obj as IndexTuple;
        }
    }
}
