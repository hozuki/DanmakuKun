using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DanmakuKun
{
    public class FunctionCompletionData : CompletionData
    {

        protected string _source;
        protected string _returnTypeName;

        public const string DefaultReturnTypeName = "int";

        public FunctionCompletionData(string name, string returnTypeName)
            : this(name, returnTypeName, null)
        {
        }

        public FunctionCompletionData(string name, string returnTypeName, string description)
            : this(name, returnTypeName, description, null)
        {
        }

        public FunctionCompletionData(string name, string returnTypeName, string description, ItemModifiers modifiers)
            : this(name, returnTypeName, description, null, modifiers)
        {
        }

        public FunctionCompletionData(string name, string returnTypeName, string description, string source)
            : this(name, returnTypeName, description, source, ItemModifiers.None)
        {
        }

        public FunctionCompletionData(string name, string returnTypeName, string description, string source, ItemModifiers modifiers)
            : this(name, returnTypeName, description, source, modifiers, name)
        {
        }

        public FunctionCompletionData(string name, string returnTypeName, string description, string source, ItemModifiers modifiers, string replacing)
            : base(name, description, source, modifiers, replacing)
        {
            _returnTypeName = returnTypeName;
            if (string.IsNullOrEmpty(_returnTypeName))
            {
                _returnTypeName = DefaultReturnTypeName;
            }
        }

        public virtual string ReturnTypeName
        {
            get
            {
                return _returnTypeName;
            }
        }

        public override string Text
        {
            get
            {
                return _text + " : " + _returnTypeName;
            }
        }

        public override ImageSource Image
        {
            get
            {
                return CompletionItemImages.FunctionItemIcon;
            }
        }

        public override object Content
        {
            get
            {
                TextBlock txt = new TextBlock();
                txt.Inlines.Add(_text + " : ");
                txt.Inlines.Add(new Bold(new Run(_returnTypeName)));
                return txt;
            }
        }

    }
}
