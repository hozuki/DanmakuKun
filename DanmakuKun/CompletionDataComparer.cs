using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public sealed class CompletionDataComparer : IComparer<CompletionData>
    {
        public int Compare(CompletionData x, CompletionData y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            return string.Compare(x.Text, y.Text);
        }
    }
}
