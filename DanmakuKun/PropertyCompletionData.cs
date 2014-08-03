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
        protected ItemModifiers _modifiers;

        public PropertyCompletionData(string name, string typeName)
            : this(name, typeName, ItemModifiers.None)
        {
        }

        public PropertyCompletionData(string name, string typeName, ItemModifiers modifiers)
            : base(name)
        {
            _typeName = typeName;
            _modifiers = modifiers;
        }

        public PropertyCompletionData(string name, string typeName, string description)
            : this(name, typeName, ItemModifiers.None, description)
        {
        }

        public PropertyCompletionData(string name, string typeName, ItemModifiers modifiers, string description)
            : base(name, description)
        {
            _typeName = typeName;
            _modifiers = modifiers;
        }

        public PropertyCompletionData(string name, string typeName, ItemModifiers modifiers, string description, string replacing)
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

        public virtual ItemModifiers Modifiers
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
                    case ItemModifiers.ReadOnly:
                        return CompletionItemImages.PropertyItemIconReadOnly;
                    case ItemModifiers.WriteOnly:
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
