using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public class CompletionData : ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData, IComparable
    {

        protected string _text;
        protected string _description;
        protected string _replacing;

        public CompletionData(string text)
            : this(text, text)
        {
        }

        public CompletionData(string text, string description)
            : this(text, description, text)
        {
        }

        public CompletionData(string text, string description, string replacing)
        {
            _text = text;
            _description = description;
            _replacing = replacing;
        }

        public virtual void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, _replacing);
        }

        public virtual object Content
        {
            get
            {
                return this.Text;
            }
        }

        public virtual object Description
        {
            get
            {
                return _description;
            }
        }

        public virtual System.Windows.Media.ImageSource Image
        {
            get
            {
                return null;
            }
        }

        public virtual double Priority
        {
            get
            {
                return 0.0;
            }
        }

        public virtual string Text
        {
            get
            {
                return _text;
            }
        }

        public virtual string Replacing
        {
            get
            {
                return _replacing;
            }
        }

        public virtual int CompareTo(object obj)
        {
            if (obj==null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj is ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData)
            {
                return string.Compare(this.Text, (obj as ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData).Text);
            }
            else
            {
                throw new ArgumentException("不能与 ICompletionData 之外的对象比较。", "obj");
            }
        }
    }
}
