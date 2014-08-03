using System;
using System.Linq;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DanmakuKun
{
    public class FunctionInsightList : ObjectList<FunctionInsightData>, IEnumerable<FunctionInsightData>
    {

        IList<FunctionInsightData> _list;

        public FunctionInsightList(params FunctionInsightData[] functions)
        {
            _list = new List<FunctionInsightData>();
            if (functions != null && functions.Length > 0)
            {
                foreach (var item in functions)
                {
                    _list.Add(item);
                }
            }
        }

        public FunctionInsightList(IEnumerable<FunctionInsightData> functions)
        {
            _list = new List<FunctionInsightData>();
            if (functions != null && functions.Count() > 0)
            {
                foreach (var item in functions)
                {
                    _list.Add(item);
                }
            }
        }

        public static implicit operator FunctionInsightList(FunctionInsightData functions)
        {
            var fList = new FunctionInsightList(functions);
            return fList;
        }

        public override IList<FunctionInsightData> List
        {
            get
            {
                return _list;
            }
        }

        private InsightWindow GetNullItemInsightWindow(TextArea textArea)
        {
            InsightWindow insightWindow = new InsightWindow(textArea);
            insightWindow.Closed += (s, e) =>
            {
                insightWindow = null;
            };
            return insightWindow;
        }

        private InsightWindow GetSingleItemInsightWindow(TextArea textArea)
        {
            InsightWindow insightWindow = GetNullItemInsightWindow(textArea);
            FunctionInsightData data = _list[0];
            insightWindow.Content = data.GetContent();
            return insightWindow;
        }

        private OverloadInsightWindow GetMultiItemInsightWindow(TextArea textArea, IEnumerable<FunctionInsightData> list)
        {
            var insightWindow = new OverloadInsightWindow(textArea);
            insightWindow.Closed += (s, e) =>
            {
                insightWindow = null;
            };
            insightWindow.Provider = new FunctionOverloadProvider(list);
            return insightWindow;
        }

        public InsightWindow GetInsightWindow(TextArea textArea, FunctionModifiers filter)
        {
            if (_list.Count == 1)
            {
                if (filter == FunctionModifiers.None || (_list[0].Modifiers & filter) != 0)
                {
                    return GetSingleItemInsightWindow(textArea);
                }
                else
                {
                    return GetNullItemInsightWindow(textArea);
                }
            }
            else if (_list.Count == 0)
            {
                return GetNullItemInsightWindow(textArea);
            }
            else
            {
                bool matchesFilter = false;
                foreach (var item in _list)
                {
                    if (filter == FunctionModifiers.None || (item.Modifiers & filter) != 0)
                    {
                        matchesFilter = true;
                        break;
                    }
                }
                if (matchesFilter)
                {
                    IList<FunctionInsightData> newList = new List<FunctionInsightData>();
                    foreach (var item in _list)
                    {
                        if (filter == FunctionModifiers.None || (item.Modifiers & filter) != 0)
                        {
                            newList.Add(item);
                        }
                    }
                    return GetMultiItemInsightWindow(textArea, newList);
                }
                else
                {
                    return GetNullItemInsightWindow(textArea);
                }
            }
        }

        public IEnumerator<FunctionInsightData> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public FunctionInsightData this[int index]
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

        public override void Concat(ObjectList<FunctionInsightData> sourceList)
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
