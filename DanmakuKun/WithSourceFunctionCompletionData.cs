using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DanmakuKun
{
    public class WithSourceFunctionCompletionData : FunctionCompletionData, IWithSourceObject
    {

        protected string _source;

        

        public WithSourceFunctionCompletionData(string text, string returnTypeName, string source)
            : base(text, returnTypeName)
        {
            _source = source;
        }

        public WithSourceFunctionCompletionData(string text, string returnTypeName, string source, string description)
            : base(text, returnTypeName, description)
        {
            _source = source;
        }

        public WithSourceFunctionCompletionData(string text, string returnTypeName, string source, string description, string replacing)
            : base(text, returnTypeName, description, replacing)
        {
            _source = source;
        }

        public virtual string Source
        {
            get
            {
                return _source;
            }
        }

        //public override object Content
        //{
        //    get
        //    {
        //        TextBlock txt = new TextBlock();
        //        txt.Inlines.Add(this.Text);
        //        txt.Inlines.Add(new Italic(new Run(" @" + this.Source)));
        //        return txt;
        //    }
        //}

        public override int CompareTo(object obj)
        {
            var o1 = obj as ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData;
            var o2 = obj as IWithSourceObject;
            if (o1 != null && o2 != null)
            {
                return string.Compare(Text + "@" + Source, o1.Text + "@" + o2.Source);
            }
            else
            {
                return base.CompareTo(obj);
            }
        }

    }
}
