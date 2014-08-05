using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DanmakuKun
{
    public sealed class RegexHelper
    {

        private RegexHelper()
        {
        }

        public const string IdentifierPattern = @"\b[a-zA-Z_][\w]*[\s]*\b";
        public const string IdentifierPatternRTL = @"\b[a-zA-Z_][\w]*[\s]*\$";
        public const string IdentifierNullablePattern = @"\b([a-zA-Z_][\w]*)?[\s]*\b";
        public const string IdentifierNullablePatternRTL = @"\b([a-zA-Z_][\w]*)?[\s]*$";
        public static readonly Regex IdentifierRegex;
        public static readonly Regex IdentifierRegexRTL;
        public const string StringPattern = @"""[\s\S]*""[\s]*";
        public static readonly Regex StringRegex;
        public const string DollarOrIdentifierPattern = @"\b([a-zA-Z_][\w]*|\$)[\s]*\b";
        public const string DollarOrIdentifierPatternRTL = @"\b([a-zA-Z_][\w]*|\$)[\s]*$";
        public const string DollarOrIdentifierNullablePattern = @"\b([a-zA-Z_][\w]*|\$)?[\s]*\b";
        public const string DollarOrIdentifierNullablePatternRTL = @"\b([a-zA-Z_][\w]*|\$)?[\s]*$";
        public static readonly Regex DollarOrIdentifierRegex;
        public static readonly Regex DollarOrIdentifierRegexRTL;
        // 二者匹配 System.Collections.List<T> 和 SomeObject.SomeCollection[index]
        public const string LeveledIdentifierWithGenericPattern = @"(\b[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*>)|(\[[\S]+\]))*)[\s]*\b";
        public const string LeveledIdentifierWithGenericPatternRTL = @"(\b[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*>)|(\[[\S]+\]))*)[\s]*$";
        public static readonly Regex LeveledIdentifierWithGenericRegex;
        public static readonly Regex LeveledIdentifierWithGenericRegexRTL;
        //public const string DollarOrLeveledIdentifierPattern = @"(\b[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*>)|(\[[\S]+\]))*)[\s]*\b";
        public const string DollarOrLeveledIdentifierWithGenericPattern = @"(" + @"(([\s]+|^)\$([\.][a-zA-Z_][\w]*([\.]?[a-zA-Z_][\w]*)*)?((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*>)|(\[[\S]+\]))*)" + @"|" + @"(\b[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*>)|(\[[\S]+\]))*)" + @")" + @"[\s]*\b";
        // 字符串较长时所需时间非常非常长
        public const string DollarOrLeveledIdentifierWithGenericPatternRTL = @"(?:" + @"(?:(?:[\s]+|^)\$(?:[\.][a-zA-Z_][\w]*(?:[\.]?[a-zA-Z_][\w]*)*)?(?:(?:<[a-zA-Z_](?:[\.]?[a-zA-Z_][\w]*)*>)|(?:\[[\S]+\]))*)" + @"|" + @"(?:(?:^|[\s]+)\b[a-zA-Z_](?:[\.]?[a-zA-Z_][\w]*)*(?:(?:<[a-zA-Z_](?:[\.]?[a-zA-Z_][\w]*)*>)|(?:\[[\S]+\]))*)" + @")" + @"[\s]*$";
        public static readonly Regex DollarOrLeveledIdentifierWithGenericRegex;
        public static readonly Regex DollarOrLeveledIdentifierWithGenericRegexRTL;
        // version: 1.0
        //public const string LeveledIdentifierPatternRTLInString = @"^(?:([a-zA-Z_][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*)*$";
        // version: 1.1
        public const string LeveledIdentifierAndArrayPatternRTLInString = @"^(?:([a-zA-Z_][\w]*)[\s]*(?:\[[^\[\]]*(?:(?:(?'open'\[)[^\[\]]*)+(?:(?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*(?:\[[^\[\]]*(?:(?:(?'open'\[)[^\[\]]*)+(?:(?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)*$";
        public static readonly Regex LeveledIdentifierAndArrayRegexRTLInString;
        //public const string DollarOrLeveledIdentifierPatternRTL = @"(?:[\s]+|^)(\$|(?:([a-zA-Z_][\w]*)[\s]*))(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*)*$";
        public const string DollarOrLeveledIdentifierPatternRTL = @"(?:^|[^\w$_])(?:([a-zA-Z_$][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*)*$";
        public static readonly Regex DollarOrLeveledIdentifierRegexRTL;
        // 未加数组的。加的方法见 http://www.cnblogs.com/qiantuwuliang/archive/2011/06/11/2078482.html
        //public const string LeveledIdentifierAndArrayPatternRTL = @"(?:[\s]+|^)(?:([a-zA-Z_][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*)*$";
        // 注意，由于 $ 并无子属性是数组，所以不将 $ 加入搜索
        // 上面的限制于2014.08.02取消
        //public const string LeveledIdentifierAndArrayPatternRTL = @"(?:[\s]+|^)(?:([a-zA-Z_][\w]*)[\s]*(?:\[[^\[\]]*(((?'open'\[)[^\[\]]*)+((?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*(?:\[[^\[\]]*(((?'open'\[)[^\[\]]*)+((?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)*$";
        //public const string LeveledIdentifierAndArrayPatternRTL = @"(?:[^\$]|^)(?:([a-zA-Z_][\w]*)[\s]*(?:\[[^\[\]]*(((?'open'\[)[^\[\]]*)+((?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*(?:\[[^\[\]]*(((?'open'\[)[^\[\]]*)+((?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)*$";
        public const string DollarOrLeveledIdentifierAndArrayPatternRTL = @"(?:[^$_]|^)(?:([a-zA-Z_$][\w]*)[\s]*(?:\[[^\[\]]*(((?'open'\[)[^\[\]]*)+((?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)(?:\.[\s]*([a-zA-Z_$][\w]*)[\s]*(?:\[[^\[\]]*(((?'open'\[)[^\[\]]*)+((?'-open'\])[^\[\]]*)+)*(?(open)(?!))\][\s]*)*)*$";
        public static readonly Regex DollarOrLeveledIdentifierAndArrayRegexRTL;

        public const string FunctionCallIdentifierPatternRTL = @"(?:^|\b|[^\w_$]?)(?:(?:[a-zA-Z_$][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_$][\w]*)[\s]*)*$";
        public static readonly Regex FunctionCallIdentifierRegexRTL;
        public const string FunctionDefinitionIdentifierPatternRTL = @"(?:^|\b|[^\w_$]?)function[\s][\s]*(?:(?:[a-zA-Z_$][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_$][\w]*)[\s]*)*$";
        public static readonly Regex FunctionDefinitionIdentifierRegexRTL;
        public const string GraphicsFunctionCallPattern = @"[\s\S]*\.graphics\.([^\.][^\.]*)$";
        public static readonly Regex GraphicsFunctionCallRegex;

        static RegexHelper()
        {
            IdentifierRegex = new Regex(IdentifierPattern, RegexOptions.Compiled);
            IdentifierRegexRTL = new Regex(IdentifierPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            StringRegex = new Regex(StringPattern, RegexOptions.Compiled);
            DollarOrIdentifierRegex = new Regex(DollarOrIdentifierPattern, RegexOptions.Compiled);
            DollarOrIdentifierRegexRTL = new Regex(DollarOrIdentifierPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            LeveledIdentifierWithGenericRegex = new Regex(LeveledIdentifierWithGenericPattern, RegexOptions.Compiled);
            LeveledIdentifierWithGenericRegexRTL = new Regex(LeveledIdentifierWithGenericPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            DollarOrLeveledIdentifierWithGenericRegex = new Regex(DollarOrLeveledIdentifierWithGenericPattern, RegexOptions.Compiled);
            DollarOrLeveledIdentifierWithGenericRegexRTL = new Regex(DollarOrLeveledIdentifierWithGenericPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            LeveledIdentifierAndArrayRegexRTLInString = new Regex(LeveledIdentifierAndArrayPatternRTLInString, RegexOptions.Compiled | RegexOptions.RightToLeft);
            DollarOrLeveledIdentifierRegexRTL = new Regex(DollarOrLeveledIdentifierPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            DollarOrLeveledIdentifierAndArrayRegexRTL = new Regex(DollarOrLeveledIdentifierAndArrayPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            FunctionCallIdentifierRegexRTL = new Regex(FunctionCallIdentifierPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            FunctionDefinitionIdentifierRegexRTL = new Regex(FunctionDefinitionIdentifierPatternRTL, RegexOptions.Compiled | RegexOptions.RightToLeft);
            GraphicsFunctionCallRegex = new Regex(GraphicsFunctionCallPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 初始化各个正则表达式，强制其编译。
        /// </summary>
        public static void Initialize()
        {
            IdentifierRegex.Match(string.Empty);
            IdentifierRegexRTL.Match(string.Empty);
            StringRegex.Match(string.Empty);
            DollarOrIdentifierRegex.Match(string.Empty);
            DollarOrIdentifierRegexRTL.Match(string.Empty);
            LeveledIdentifierWithGenericRegex.Match(string.Empty);
            LeveledIdentifierWithGenericRegexRTL.Match(string.Empty);
            DollarOrLeveledIdentifierWithGenericRegex.Match(string.Empty);
            DollarOrLeveledIdentifierWithGenericRegexRTL.Match(string.Empty);
            LeveledIdentifierAndArrayRegexRTLInString.Match(string.Empty);
            DollarOrLeveledIdentifierRegexRTL.Match(string.Empty);
            DollarOrLeveledIdentifierAndArrayRegexRTL.Match(string.Empty);
            FunctionCallIdentifierRegexRTL.Match(string.Empty);
            FunctionDefinitionIdentifierRegexRTL.Match(string.Empty);
            GraphicsFunctionCallRegex.Match(string.Empty);
        }

    }
}
