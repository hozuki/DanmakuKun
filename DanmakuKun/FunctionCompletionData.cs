using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public class FunctionCompletionData : CompletionData
    {

        protected string _returnTypeName;

        public FunctionCompletionData(string text, string returnTypeName)
            : base(text)
        {
            _returnTypeName = returnTypeName;
        }

        public FunctionCompletionData(string text, string returnTypeName, string description)
            : base(text, description)
        {
            _returnTypeName = returnTypeName;
        }

        public FunctionCompletionData(string text, string returnTypeName, string description, string replacing)
            : base(text, description, replacing)
        {
            _returnTypeName = returnTypeName;
        }

        public virtual string ReturnTypeName
        {
            get
            {
                return _returnTypeName;
            }
        }

        public override string Text
        {
            get
            {
                return _text + " : " + _returnTypeName;
            }
        }

        public override System.Windows.Media.ImageSource Image
        {
            get
            {
                return CompletionItemImages.FunctionItemIcon;
            }
        }

    }
}
