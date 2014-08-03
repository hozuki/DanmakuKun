using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public static class Utils
    {

        private const int CharHashTableUpperBound = 256; // 0-255

        private static int[] charHashTable = new int[CharHashTableUpperBound + 1];

        public static int CountChar(this string content, char character)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }
            int r = 0;
            foreach (var c in content)
            {
                if (c == character)
                {
                    r++;
                }
            }
            return r;
        }

        public static int CountCharCStyle(this string content, char character)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }
            int r = 0;
            int len = content.Length;
            if (character == '"')
            {
                for (var i = 0; i < len; i++)
                {
                    if (content[i] == character && (i == 0 || content[i - 1] != '\\'))
                    {
                        r++;
                    }
                }
            }
            else if (character == '\'')
            {
                for (var i = 0; i < len; i++)
                {
                    //if (content[i] == character && (i == 0 || (content[i - 1] != '\\' && content[i - 1] != '"')))
                    if (content[i] == character && (i == 0 || content[i - 1] != '\\'))
                    {
                        r++;
                    }
                }
            }
            else
            {
                for (var i = 0; i < len; i++)
                {
                    if (content[i] == character)
                    {
                        r++;
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// 使用空间换时间的方法，在一个字符串中查找字符组的匹配。
        /// </summary>
        /// <param name="content"></param>
        /// <param name="chars">字符数组。该字符数组不能大于 CharHashTableUpperBound。</param>
        /// <returns></returns>
        public static int CountChars(this string content, char[] chars)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }
            if (chars == null || chars.Length == 0)
            {
                return 0;
            }
            foreach (var c in chars)
            {
                if ((int)c > CharHashTableUpperBound)
                {
                    throw new ArgumentOutOfRangeException("不能检测大于 " + CharHashTableUpperBound.ToString() + " 的字符。");
                }
            }
            charHashTable.Initialize();
            foreach (var c in content)
            {
                if ((int)c < CharHashTableUpperBound)
                {
                    charHashTable[(int)c]++;
                }
            }
            int r = 0;
            foreach (var c in chars)
            {
                r += charHashTable[(int)c];
            }
            return r;
        }

        /// <summary>
        /// 使用多重遍历统计字符串中字符数组的匹配数。时间复杂度为 O(n*m)，慎用！
        /// </summary>
        /// <param name="content"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static int CountChars2(this string content, char[] chars)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }
            if (chars == null || chars.Length == 0)
            {
                return 0;
            }
            int r = 0;
            foreach (var c1 in content)
            {
                foreach (var c2 in chars)
                {
                    if (c1 == c2)
                    {
                        r++;
                    }
                }
            }
            return r;
        }

        public static void OneListToAnother(CompletionList source, IList<ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData> destination)
        {
            // 该复制操作不支持识别重载
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }


        //public static void CombineListsToOne<TSource, TDest>(IList<TDest> destination, bool sorted, params IList<TSource>[] sources)
        public static void CombineListsToOne(IList<ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData> destination, bool sorted, params CompletionList[] sources)
        //where TSource : TDest, IComparable
        //where TDest : class, ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
        {
            // 开始操作时，不清空目标数组
            // 目标数组内部参与排序
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            for (var i = 0; i < sources.Length; i++)
            {
                if (sources[i] == null)
                {
                    throw new ArgumentNullException("sources[" + i.ToString() + "]");
                }
            }
            //var tempList = new SortedList<TSource, TSource>();
            IDictionary<string, CompletionList> tempList;
            if (sorted)
            {
                tempList = new SortedList<string, CompletionList>();
            }
            else
            {
                tempList = new Dictionary<string, CompletionList>();
            }
            foreach (var completionData in destination)
            {
                var dataList = completionData as CompletionList;
                if (dataList != null)
                {
                    if (tempList.Keys.Contains(completionData.Text))
                    {
                        tempList[dataList.Text].Concat(dataList);
                    }
                    else
                    {
                        tempList.Add(dataList.Text, dataList);
                    }
                }
            }
            foreach (var completionLists in sources)
            {
                foreach (var completionData in completionLists)
                {
                    // 如果没有，就加入；有，就合并
                    if (tempList.Keys.Contains(completionData.Text))
                    {
                        tempList[completionData.Text].List.Add(completionData);
                    }
                    else
                    {
                        tempList.Add(completionData.Text, completionData);
                    }
                }
            }
            destination.Clear();
            foreach (var item in tempList)
            {
                destination.Add(item.Value);
            }
        }
    }
}
