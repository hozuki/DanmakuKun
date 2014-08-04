using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DanmakuKun
{
    public class KeywordCompletionData : CompletionData
    {

        public KeywordCompletionData(string name)
            : this(name, null)
        {
        }

        public KeywordCompletionData(string name, string description)
            : this(name, description, null)
        {
        }

        public KeywordCompletionData(string name, string description, string source)
            : this(name, description, source, null)
        {
        }

        public KeywordCompletionData(string name, string description, string source, string replacing)
            : base(name, description, source, ItemModifiers.None, replacing)
        {
        }

        //public override object Content
        //{
        //    get
        //    {
        //        var brush = Brushes.Blue;
        //        TextBlock tb = new TextBlock();
        //        tb.Inlines.Add(_text);
        //        tb.Foreground = brush;
        //        return tb;
        //    }
        //}

        public override ImageSource Image
        {
            get
            {
                return CompletionItemImages.KeywordItemIcon;
            }
        }

    }
}
