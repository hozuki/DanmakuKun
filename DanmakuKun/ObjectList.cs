using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    // 不能声明为接口，否则 CompletionList 和 FunctionInsightList 在被作为类型调用时，IDictionary 中无法隐式转换而出错。
    public abstract class ObjectList<T>
    {

        public abstract IList<T> List { get; }
        public abstract void Concat(ObjectList<T> sourceList);

    }
}
