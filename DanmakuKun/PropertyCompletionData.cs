using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text;

namespace DanmakuKun
{
    public class PropertyCompletionData : CompletionData
    {

        protected string _typeName;
        protected PropertyModifiers _modifiers;

        public PropertyCompletionData(string name, string typeName)
            : this(name, typeName, PropertyModifiers.None)
        {
        }

        public PropertyCompletionData(string name, string typeName, PropertyModifiers modifiers)
            : base(name)
        {
            _typeName = typeName;
            _modifiers = modifiers;
        }

        public PropertyCompletionData(string name, string typeName, string description)
            : this(name, typeName, PropertyModifiers.None, description)
        {
        }

        public PropertyCompletionData(string name, string typeName, PropertyModifiers modifiers, string description)
            : base(name, description)
        {
            _typeName = typeName;
            _modifiers = modifiers;
        }

        public PropertyCompletionData(string name, string typeName, PropertyModifiers modifiers, string description, string replacing)
            : base(name, description, replacing)
        {
            _typeName = typeName;
            _modifiers = modifiers;
        }

        public virtual string TypeName
        {
            get
            {
                return _typeName;
            }
        }

        public virtual PropertyModifiers Modifiers
        {
            get
            {
                return _modifiers;
            }
        }

        public override string Text
        {
            get
            {
                return _text + " : " + _typeName;
            }
        }

        public override System.Windows.Media.ImageSource Image
        {
            get
            {
                switch (_modifiers)
                {
                    case PropertyModifiers.ReadOnly:
                        return CompletionItemImages.PropertyItemIconReadOnly;
                    case PropertyModifiers.WriteOnly:
                        return CompletionItemImages.PropertyItemIconWriteOnly;
                    default:
                        return CompletionItemImages.PropertyItemIcon;
                }
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
