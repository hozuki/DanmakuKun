using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public class ArgumentInsightData
    {

        string _name;
        string _typeName;
        string _defaultValue;

        public ArgumentInsightData(string name, string typeName)
            : this(name, typeName, string.Empty)
        {
        }

        public ArgumentInsightData(string name, string typeName, string defaultValue)
        {
            _name = name;
            _typeName = typeName;
            _defaultValue = defaultValue;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string TypeName
        {
            get
            {
                return _typeName;
            }
        }

        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }

        internal string GetTypeAndDefaultValue()
        {
            return _typeName + (!string.IsNullOrEmpty(_defaultValue) ? " = " + _defaultValue : string.Empty);
        }

        public override string ToString()
        {
            return _name + " : " + this.GetTypeAndDefaultValue();
        }

    }
}
