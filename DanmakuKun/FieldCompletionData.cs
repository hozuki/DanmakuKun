using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public class FieldCompletionData : CompletionData
    {

        public FieldCompletionData(string name)
            : base(name)
        {
        }

        public FieldCompletionData(string name, string description)
            : base(name, description)
        {
        }

        public FieldCompletionData(string name, string description, string replacing)
            : base(name, description, replacing)
        {
        }

    }
}
