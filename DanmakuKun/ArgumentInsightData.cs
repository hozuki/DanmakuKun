using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public class ArgumentInsightData
    {

        public const string DefaultType = "int";
        public const string DefaultName = "(NoName)";

        protected string _name;
        protected string _typeName;
        protected string _defaultValue;
        protected string _description;

        public ArgumentInsightData(string name, string typeName)
            : this(name, typeName, string.Empty)
        {
        }

        public ArgumentInsightData(string name, string typeName, string defaultValue)
            : this(name, typeName, defaultValue, string.Empty)
        {
        }

        public ArgumentInsightData(string name, string typeName, string defaultValue, string description)
        {
            _name = name;
            if (string.IsNullOrEmpty(_name))
            {
                _name = DefaultName;
            }
            _typeName = typeName;
            if (string.IsNullOrEmpty(_typeName))
            {
                _typeName = DefaultType;
            }
            _defaultValue = defaultValue;
            if (_defaultValue == null)
            {
                _defaultValue = string.Empty;
            }
            _description = description;
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

        public string Description
        {
            get
            {
                return _description;
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
