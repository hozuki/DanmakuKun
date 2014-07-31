using System;
using System.Collections.Generic;
using System.Text;

namespace FastColoredTextBoxNS
{
    class LinesAccessor : IList<string>
    {
        FastColoredTextBox tb;

        public LinesAccessor(FastColoredTextBox tb)
        {
            this.tb = tb;
        }

        public int IndexOf(string item)
        {
            for (int i = 0; i < tb.LinesCount; i++)
                if (tb[i].Text == item)
                    return i;

            return -1;
        }

        public void Insert(int index, string item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public string this[int index]
        {
            get
            {
                return tb[index].Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(string item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(string item)
        {
            for (int i = 0; i < tb.LinesCount; i++)
                if (tb[i].Text == item)
                    return true;

            return false;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            for (int i = 0; i < tb.LinesCount; i++)
                array[i + arrayIndex] = tb[i].Text;
        }

        public int Count
        {
            get { return tb.LinesCount; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(string item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < tb.LinesCount; i++)
                yield return tb[i].Text;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
