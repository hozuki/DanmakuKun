using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DanmakuKun
{
    public class ConstantCompletionData : FieldCompletionData
    {

         public ConstantCompletionData(string name, string typeName)
            : base(name, typeName, null, ItemModifiers.Static | ItemModifiers.ReadOnly)
        {
        }

        public ConstantCompletionData(string name, string typeName, string description)
            : base(name, typeName, description, ItemModifiers.Static | ItemModifiers.ReadOnly)
        {
        }

        public ConstantCompletionData(string name, string typeName, string description, string source)
            : base(name, typeName, description, source, ItemModifiers.Static | ItemModifiers.ReadOnly)
        {
        }

        public ConstantCompletionData(string name, string typeName, string description, string source, string replacing)
            : base(name, typeName, description, source, ItemModifiers.Static | ItemModifiers.ReadOnly, replacing)
        {
        }

        public override ImageSource Image
        {
            get
            {
                return CompletionItemImages.ConstantItemIcon;
            }
        }

    }
}
