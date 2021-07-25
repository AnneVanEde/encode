using System;

namespace ENCODE.Base
{
    class ECSItem
    {
        public string variableName;
        public string variableType;

        public ECSItem(string _name = "", string _type = "")
        {
            variableName = _name;
            variableType = _type;
        }

        public virtual string GetKey()
        {
            return $"ecs_{variableType}_{variableName}";
        }

        public virtual string GetLabel()
        {
            return $"[{variableType}] {variableName}";
        }

    }
}
