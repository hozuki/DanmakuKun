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
    public class KeywordCompletionData : CompletionData, IWithSourceObject
    {

        private string _source;

        public KeywordCompletionData(string text)
            : this(text, string.Empty)
        {
        }

        public KeywordCompletionData(string text, string description)
            : this(text, description, null)
        {
        }

        public KeywordCompletionData(string text, string description, string source)
            : base(text, description)
        {
            _source = source;
            if (_source == null)
            {
                _source = string.Empty;
            }
        }

        public KeywordCompletionData(string text, string description, string source, string replacing)
            : base(text, description, replacing)
        {
            _source = source;
            if (_source == null)
            {
                _source = string.Empty;
            }
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

        public override System.Windows.Media.ImageSource Image
        {
            get
            {
                return CompletionItemImages.KeywordItemIcon;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
        }
    }
}
