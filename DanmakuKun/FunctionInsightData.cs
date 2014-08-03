using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DanmakuKun
{
    public class FunctionInsightData : IWithSourceObject
    {

        public const string DefaultName = "(NoName)";
        public const string DefaultSource = "(全局)";
        public const string DefaultReturnType = "void";

        protected string _name;
        protected string _source;
        protected IList<ArgumentInsightData> _arguments;
        protected ReadOnlyCollection<ArgumentInsightData> _arguments_r;
        protected string _description;
        protected string _returnTypeName;
        protected string _returnDescription;
        protected string _remarks;
        protected FunctionModifiers _modifiers;

        public FunctionInsightData(string name, string returnTypeName, string source, string description, FunctionModifiers modifiers, string returnDescription, string remarks, IEnumerable<ArgumentInsightData> arguments)
        {
            InitializeInternal(name, returnTypeName, source, description, modifiers, returnDescription, remarks, arguments);
        }

        private void InitializeInternal(string name, string returnTypeName, string source, string description, FunctionModifiers modifiers, string returnDescription, string remarks, IEnumerable<ArgumentInsightData> arguments)
        {
            _name = name;
            if (string.IsNullOrEmpty(_name))
            {
                _name = DefaultName;
            }
            _source = source;
            if (string.IsNullOrEmpty(_source))
            {
                _source = DefaultSource;
            }
            _description = description;
            if (_description == null)
            {
                _description = string.Empty;
            }
            _modifiers = modifiers;
            _returnTypeName = returnTypeName;
            if (string.IsNullOrEmpty(_returnTypeName))
            {
                _returnTypeName = DefaultReturnType;
            }
            _returnDescription = returnDescription;
            if (_returnDescription == null)
            {
                _returnDescription = string.Empty;
            }
            _remarks = remarks;
            if (_remarks == null)
            {
                _remarks = string.Empty;
            }
            if (arguments != null && arguments.Count() > 0)
            {
                _arguments = new List<ArgumentInsightData>(arguments);
            }
            else
            {
                _arguments = new List<ArgumentInsightData>();
            }
            _arguments_r = new ReadOnlyCollection<ArgumentInsightData>(_arguments);
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string ReturnDescription
        {
            get
            {
                return _returnDescription;
            }
        }

        public string Remarks
        {
            get
            {
                return _remarks;
            }
        }

        public FunctionModifiers Modifiers
        {
            get
            {
                return _modifiers;
            }
        }

        public ReadOnlyCollection<ArgumentInsightData> Arguments
        {
            get
            {
                return _arguments_r;
            }
        }

        public virtual string GetFunctionHeader()
        {
            string s = "function " + _name + "(";
            int len = _arguments.Count;
            for (var i = 0; i < len; i++)
            {
                s += Arguments[i].ToString();
                if (i != len - 1)
                {
                    s += ", ";
                }
            }
            s += ") : " + _returnTypeName;
            s += " @" + _source;
            return s;
        }

        public override string ToString()
        {
            return (GetContent() as TextBlock).Text;
        }

        public virtual UIElement GetHeaderContent(bool withSource)
        {
            var tb = new TextBlock();
            tb.Inlines.Add("function " + _name + "(");
            int len = _arguments.Count;
            for (var i = 0; i < len; i++)
            {
                tb.Inlines.Add(new Bold(new Run(_arguments[i].Name)));
                tb.Inlines.Add(" : " + _arguments[i].GetTypeAndDefaultValue());
                if (i < len - 1)
                {
                    tb.Inlines.Add(", ");
                }
            }
            tb.Inlines.Add(") : " + _returnTypeName);
            if (withSource)
            {
                tb.Inlines.Add(" @" + _source);
            }
            if ((_modifiers & FunctionModifiers.Static) != 0)
            {
                tb.Inlines.Add(" [静态]");
            }
            return tb;
        }

        public virtual UIElement GetFooterContent()
        {
            var tb = new TextBlock();
            tb.Inlines.Add(_description);
            foreach (var arg in _arguments)
            {
                if (!string.IsNullOrEmpty(arg.Description))
                {
                    tb.Inlines.Add("\n");
                    tb.Inlines.Add(new Italic(new Bold(new Run((arg.Name)))));
                    tb.Inlines.Add(new Italic(new Run(" : " + arg.TypeName + " : " + arg.Description)));
                }
            }
            if (!string.IsNullOrEmpty(_returnDescription))
            {
                tb.Inlines.Add("\n\n");
                tb.Inlines.Add(new Bold(new Run("返回")));
                tb.Inlines.Add("\n" + _returnTypeName + " : ");
                tb.Inlines.Add(_returnDescription);
            }
            if (!string.IsNullOrEmpty(_remarks))
            {
                tb.Inlines.Add("\n\n");
                tb.Inlines.Add(new Bold(new Run("说明")));
                tb.Inlines.Add("\n" + _remarks);
            }
            tb.TextWrapping = TextWrapping.Wrap;
            return tb;
        }

        /// <summary>
        /// 默认会带上源。
        /// </summary>
        /// <returns></returns>
        public UIElement GetContent()
        {
            return this.GetContent(true);
        }

        public virtual UIElement GetContent(bool withSource)
        {
            var tb = new TextBlock();
            tb.Inlines.Add(GetHeaderContent(withSource));
            tb.Inlines.Add("\n\n");
            tb.Inlines.Add(GetFooterContent());
            return tb;
        }

    }
}
