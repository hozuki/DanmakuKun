using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DanmakuKun
{
    public class CompletionList : ObjectList<CompletionData>, IEnumerable<CompletionData>, ICompletionData
    {

        IList<CompletionData> _list;

        public CompletionList(params CompletionData[] data)
        {
            _list = new List<CompletionData>();
            if (data != null && data.Length > 0)
            {
                foreach (var item in data)
                {
                    _list.Add(item);
                }
            }
        }

        public CompletionList(IEnumerable<CompletionData> data)
        {
            _list = new List<CompletionData>();
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    _list.Add(item);
                }
            }
        }

        public static implicit operator CompletionList(CompletionData data)
        {
            var cList = new CompletionList(data);
            return cList;
        }

        //public static implicit operator CompletionList(IEnumerable<CompletionData> data)
        //{
        //    var cList = new CompletionList(data);
        //    return cList;
        //}

        public override IList<CompletionData> List
        {
            get
            {
                return _list;
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (_list.Count > 0)
            {
                _list[0].Complete(textArea, completionSegment, insertionRequestEventArgs);
            }
            else
            {
                throw new InvalidOperationException("无法在列表为空时完成替换操作。");
            }
        }

        public object Content
        {
            get
            {
                return _list.Count > 0 ? _list[0].Content : null;
            }
        }

        public object Description
        {
            get
            {
                if (_list.Count > 1)
                {
                    string s = _list[0].Description as string;
                    s += "\n(+" + (_list.Count - 1).ToString() + " 实例)\n";
                    s += "来源: ";
                    for (var i = 0; i < _list.Count; i++)
                    {
                        IWithSourceObject ws = _list[i] as IWithSourceObject;
                        s += ws != null ? ws.Source : FunctionInsightData.DefaultSource;
                        if (i < _list.Count - 1)
                        {
                            s += ", ";
                        }
                    }
                    return s;
                }
                else if (_list.Count == 0)
                {
                    return null;
                }
                else
                {
                    string s = _list[0].Description as string;
                    IWithSourceObject ws = _list[0] as IWithSourceObject;
                    s += "\n来源: " + (ws != null ? ws.Source : FunctionInsightData.DefaultSource);
                    return s;
                }
            }
        }

        public System.Windows.Media.ImageSource Image
        {
            get
            {
                return _list.Count > 0 ? _list[0].Image : null;
            }
        }

        public double Priority
        {
            get
            {
                return _list.Count > 0 ? _list[0].Priority : 0d;
            }
        }

        public string Text
        {
            get
            {
                return _list.Count > 0 ? _list[0].Text : null;
            }
        }

        public IEnumerator<CompletionData> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public CompletionData this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public override void Concat(ObjectList<CompletionData> sourceList)
        {
            if (sourceList == null)
            {
                throw new ArgumentException("sourceList");
            }
            foreach (var item in sourceList.List)
            {
                _list.Add(item);
            }
        }
    }
}
