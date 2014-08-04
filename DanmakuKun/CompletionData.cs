using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace DanmakuKun
{
    public class CompletionData : ICompletionData, IComparable, IWithSourceObject
    {

        protected string _text;
        protected string _description;
        protected string _replacing;
        protected string _source;
        protected ItemModifiers _modifiers;

        public const string DefaultSource = "(全局)";
        public const string DefaultName = "(NoName)";

        public CompletionData(string text)
            : this(text, null)
        {
        }

        public CompletionData(string text, string description)
            : this(text, description, null)
        {
        }

        public CompletionData(string text, string description, ItemModifiers modifiers)
            : this(text, description, null, modifiers)
        {
        }

        public CompletionData(string text, string description, string source)
            : this(text, description, source, ItemModifiers.None)
        {
        }

        public CompletionData(string text, string description, string source, ItemModifiers modifiers)
            : this(text, description, source, modifiers, text)
        {
        }

        public CompletionData(string text, string description, string source, ItemModifiers modifiers, string replacing)
        {
            _text = text;
            if (_text == null)
            {
                _text = DefaultName;
            }
            _source = source;
            if (_source == null)
            {
                _source = DefaultSource;
            }
            _description = description;
            if (_description == null)
            {
                _description = _text;
            }
            _replacing = replacing;
            if (_replacing == null)
            {
                _replacing = _text;
            }
            _modifiers = modifiers;
        }

        public virtual void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
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

        public virtual ImageSource Image
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

        public virtual ItemModifiers Modifiers
        {
            get
            {
                return _modifiers;
            }
        }

        public virtual int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            var o1 = obj as ICompletionData;
            var o2 = obj as IWithSourceObject;
            if (o1 != null)
            {
                if (o2 != null)
                {
                    return string.Compare(Text + "@" + Source, o1.Text + "@" + o2.Source);
                }
                else
                {
                    return string.Compare(this.Text, o1.Text);
                }
            }
            else
            {
                throw new ArgumentException("不能与 ICompletionData 之外的对象比较。", "obj");
            }
        }

        public virtual string Source
        {
            get
            {
                return _source;
            }
        }
    }
}
