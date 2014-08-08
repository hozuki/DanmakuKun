using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DanmakuKun
{
    public class FunctionCompletionData : CompletionData, IWithTypeObject
    {

        protected string _typeName;

        public FunctionCompletionData(string name, string typeName)
            : this(name, typeName, null)
        {
        }

        public FunctionCompletionData(string name, string typeName, string description)
            : this(name, typeName, description, null)
        {
        }

        public FunctionCompletionData(string name, string typeName, string description, ItemModifiers modifiers)
            : this(name, typeName, description, null, modifiers)
        {
        }

        public FunctionCompletionData(string name, string typeName, string description, string source)
            : this(name, typeName, description, source, ItemModifiers.None)
        {
        }

        public FunctionCompletionData(string name, string typeName, string description, string source, ItemModifiers modifiers)
            : this(name, typeName, description, source, modifiers, name)
        {
        }

        public FunctionCompletionData(string name, string typeName, string description, string source, ItemModifiers modifiers, string replacing)
            : base(name, description, source, modifiers, replacing)
        {
            _typeName = typeName;
            if (string.IsNullOrEmpty(_typeName))
            {
                _typeName = DV.DefaultTypeName;
            }
        }

        public virtual string TypeName
        {
            get
            {
                return _typeName;
            }
        }

        public override string Text
        {
            get
            {
                return _text + " : " + _typeName;
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
                txt.Inlines.Add(new Bold(new Run(_typeName)));
                return txt;
            }
        }

    }
}
