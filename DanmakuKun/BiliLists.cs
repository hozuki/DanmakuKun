using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DanmakuKun
{
    public sealed class BiliLists
    {

        public readonly static IDictionary<string, CompletionList> PresetFuncAndProp;
        public readonly static IDictionary<string, CompletionList> UserFuncAndProp;
        public readonly static IDictionary<string, CompletionList> LocalFuncAndProp;
        public readonly static IDictionary<string, FunctionInsightList> PresetInsight;
        public readonly static IDictionary<string, FunctionInsightList> UserInsight;
        public readonly static IDictionary<string, FunctionInsightList> LocalInsight;
        /// <summary>
        /// 存放各种全局静态不可修改表：Keywords, Snippets, Classes, @ObjectFields。
        /// </summary>
        public readonly static IDictionary<string, CompletionList> PresetGlobalStatic;

        private BiliLists()
        {
        }

        static BiliLists()
        {
            PresetFuncAndProp = new Dictionary<string, CompletionList>();
            UserFuncAndProp = new Dictionary<string, CompletionList>();
            LocalFuncAndProp = new Dictionary<string, CompletionList>();
            PresetInsight = new Dictionary<string, FunctionInsightList>();
            UserInsight = new Dictionary<string, FunctionInsightList>();
            LocalInsight = new Dictionary<string, FunctionInsightList>();
            PresetGlobalStatic = new Dictionary<string, CompletionList>();
        }

        /// <summary>
        /// 什么都不做。
        /// </summary>
        public static void Initialize()
        {
        }

    }
}
