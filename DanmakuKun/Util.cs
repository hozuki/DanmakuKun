using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public static class Util
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

        public static void OneListToAnother<TSource, TDest>(IList<TSource> source, IList<TDest> destination) where TSource : TDest
        {
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


        public static void CombineListsToOne<TSource, TDest>(IList<TDest> destination, bool sorted, params IList<TSource>[] sources)
            where TSource : TDest, IComparable
            where TDest : class, ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
        {
            // 开始操作时，不清空目标数组
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
            if (sorted)
            {
                var tempList = new SortedList<TSource, TSource>();
                foreach (var list in sources)
                {
                    foreach (var item in list)
                    {
                        //System.Diagnostics.Debug.Print(item.Text);
                        tempList.Add(item, item);
                    }
                }
                foreach (var item in tempList)
                {
                    destination.Add(item.Value);
                }
            }
            else
            {
                foreach (var list in sources)
                {
                    foreach (var item in list)
                    {
                        destination.Add(item);
                    }
                }
            }
        }
    }
}
