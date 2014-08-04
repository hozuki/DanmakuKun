using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DanmakuKun
{
    public class SnippetCompletionData : CompletionData
    {

        public SnippetCompletionData(string name, string replacing)
            : this(name, null, replacing)
        {
        }

        public SnippetCompletionData(string name, string description, string replacing)
            : base(name, description, null, ItemModifiers.None, replacing)
        {
        }

        public override ImageSource Image
        {
            get
            {
                return CompletionItemImages.SnippetItemIcon;
            }
        }

    }
}
