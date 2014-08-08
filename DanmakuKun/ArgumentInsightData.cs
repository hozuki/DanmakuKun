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
        protected ItemModifiers _modifiers;

        public ArgumentInsightData(string name, string typeName)
            : this(name, typeName, string.Empty)
        {
        }

        public ArgumentInsightData(string name, string typeName, string defaultValue)
            : this(name, typeName, DV.DefaultModifiers, defaultValue)
        {
        }

        public ArgumentInsightData(string name, string typeName, ItemModifiers modifiers, string defaultValue)
            : this(name, typeName, modifiers, defaultValue, string.Empty)
        {
        }

        public ArgumentInsightData(string name, string typeName, ItemModifiers modifiers, string defaultValue, string description)
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
            _modifiers = modifiers;
            if (_defaultValue == null)
            {
                _defaultValue = string.Empty;
            }
            _description = description;
            if (_description == null)
            {
                _description = string.Empty;
            }
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

        public ItemModifiers Modifiers
        {
            get
            {
                return _modifiers;
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
