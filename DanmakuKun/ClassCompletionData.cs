using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DanmakuKun
{
    public class ClassCompletionData : CompletionData
    {

         public ClassCompletionData(string name)
            : this(name, null)
        {
        }

        public ClassCompletionData(string name, string description)
            : this(name, description, null)
        {
        }

        public ClassCompletionData(string name, string description, string source)
            : this(name, description, source, null)
        {
        }

        public ClassCompletionData(string name, string description, string source, string replacing)
            : base(name, description, source, DV.DefaultModifiers, replacing)
        {
            _source = DV.ClassListName;
        }

        public override ImageSource Image
        {
            get
            {
                return CompletionItemImages.ClassItemIcon;
            }
        }

    }
}
