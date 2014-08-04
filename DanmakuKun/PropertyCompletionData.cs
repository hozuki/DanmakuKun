using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text;
using System.Windows.Media;

namespace DanmakuKun
{
    public class PropertyCompletionData : FieldCompletionData
    {

        public PropertyCompletionData(string name, string typeName)
            : base(name, typeName)
        {
        }

        public PropertyCompletionData(string name, string typeName, string description)
            : base(name, typeName, description)
        {
        }

        public PropertyCompletionData(string name, string typeName, string description, ItemModifiers modifiers)
            : base(name, typeName, description, modifiers)
        {
        }

        public PropertyCompletionData(string name, string typeName, string description, string source)
            : base(name, typeName, description, source)
        {
        }

        public PropertyCompletionData(string name, string typeName, string description, string source, ItemModifiers modifiers)
            : base(name, typeName, description, source, modifiers)
        {
        }

        public PropertyCompletionData(string name, string typeName, string description, string source, ItemModifiers modifiers, string replacing)
            : base(name, typeName, description, source, modifiers, replacing)
        {
        }

        public override ImageSource Image
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

    }
}
