using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DanmakuKun
{
    public class FunctionOverloadProvider : IOverloadProvider
    {

        protected IList<FunctionInsightData> _functions;
        protected int _selectedIndex = -1;
        protected int _oldSelectedIndex = -1;
        private object currentContent;
        private object currentHeader;

        public FunctionOverloadProvider(IEnumerable<FunctionInsightData> functions)
        {
            if (functions != null && functions.Count() > 0)
            {
                _functions = functions.ToList();
            }
            else
            {
                _functions = new List<FunctionInsightData>();
            }
            _selectedIndex = 0;
        }

        public virtual int Count
        {
            get
            {
                return _functions.Count;
            }
        }

        public virtual object CurrentContent
        {
            get
            {
                if (currentContent != null)
                {
                    currentContent = null;
                }
                currentContent = _selectedIndex >= 0 ? _functions[_selectedIndex].GetFooterContent() : null;
                return currentContent;
            }
        }

        public virtual object CurrentHeader
        {
            get
            {
                if (currentHeader != null)
                {
                    currentHeader = null;
                }
                currentHeader = _selectedIndex >= 0 ? _functions[_selectedIndex].GetHeaderContent(false) : null;
                return currentHeader;
            }
        }

        public virtual string CurrentIndexText
        {
            get
            {
                return _selectedIndex >= 0 ? string.Format("第 {0} 个, 共 {1} 个", _selectedIndex + 1, Count) : null;
            }
        }

        public virtual int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                if (_selectedIndex < 0)
                {
                    _selectedIndex = 0;
                }
                if (_selectedIndex >= Count)
                {
                    _selectedIndex = Count - 1;
                }
                if (_selectedIndex != _oldSelectedIndex)
                {
                    if (PropertyChanged != null)
                    {
                        // Binding 的作用机制见 http://blog.csdn.net/fwj380891124/article/details/8107646
                        PropertyChanged(this, new PropertyChangedEventArgs("CurrentContent"));
                        PropertyChanged(this, new PropertyChangedEventArgs("CurrentHeader"));
                        PropertyChanged(this, new PropertyChangedEventArgs("CurrentIndexText"));
                    }
                    _oldSelectedIndex = _selectedIndex;
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
