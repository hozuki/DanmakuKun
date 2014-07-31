using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public class KeywordCompletionData : CompletionData
    {

         public KeywordCompletionData(string text)
            : base(text)
        {
        }

        public KeywordCompletionData(string text, string description)
            : base(text, description)
        {
        }

        public KeywordCompletionData(string text, string description, string replacing)
            : base(text, description, replacing)
        {
        }

        public override System.Windows.Media.ImageSource Image
        {
            get
            {
                return CompletionItemImages.KeywordItemIcon;
            }
        }

    }
}
