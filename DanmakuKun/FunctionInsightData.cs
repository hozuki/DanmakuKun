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

        private string _name;
        private string _source;
        private IList<ArgumentInsightData> _arguments;
        private ReadOnlyCollection<ArgumentInsightData> _arguments_r;
        private string _description;
        private string _returnTypeName;

        public FunctionInsightData(string name, string returnTypeName, string source, string description, params ArgumentInsightData[] arguments)
        {
            _name = name;
            _source = source;
            _description = description;
            _returnTypeName = returnTypeName;
            if (arguments != null && arguments.Length > 0)
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

        public ReadOnlyCollection<ArgumentInsightData> Arguments
        {
            get
            {
                return _arguments_r;
            }
        }

        public override string ToString()
        {
            string s = "@" + _source + "\n";
            s += "function " + _name + "(";
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
            s += "\n\n" + _description;
            return s;
        }

        public virtual UIElement ToContent()
        {
            var tb = new TextBlock();
            tb.Inlines.Add("@" + _source + "\n");
            tb.Inlines.Add("function " + _name + "(");
            int len = _arguments.Count;
            for (var i = 0; i < len; i++)
            {
                tb.Inlines.Add(new Bold(new Run(_arguments[i].Name)));
                tb.Inlines.Add(" : " + _arguments[i].GetTypeAndDefaultValue());
                if (i != len - 1)
                {
                    tb.Inlines.Add(", ");
                }
            }
            tb.Inlines.Add(") : " + _returnTypeName);
            tb.Inlines.Add("\n\n" + _description);
            tb.TextWrapping = TextWrapping.Wrap;
            return tb;
        }

    }
}
