using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DanmakuKun
{
    public class WithSourcePropertyCompletionData : PropertyCompletionData, IWithSourceObject
    {

        protected string _source;

        public WithSourcePropertyCompletionData(string name, string typeName, string source)
            : base(name, typeName)
        {
            _source = source;
        }

        public WithSourcePropertyCompletionData(string name, string typeName, string source, ItemModifiers modifiers)
            : base(name, typeName, modifiers)
        {
            _source = source;
        }

        public WithSourcePropertyCompletionData(string name, string typeName, string source, string description)
            : base(name, typeName, description)
        {
            _source = source;
        }

        public WithSourcePropertyCompletionData(string name, string typeName, string source, ItemModifiers modifiers, string description)
            : base(name, typeName, modifiers, description)
        {
            _source = source;
        }

        public WithSourcePropertyCompletionData(string name, string typeName, string source, ItemModifiers modifiers, string description, string replacing)
            : base(name, typeName, modifiers, description, replacing)
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
