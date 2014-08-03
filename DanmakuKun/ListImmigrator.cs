using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuKun
{
    public static class ListImmigrator
    {

        /// <summary>
        /// 将一个列表添加到另一个中，如果有重复项，则会认为是重载。适合 FunctionInsightList 和 CompletionList 使用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="sources"></param>
        public static void Concat<T, T2>(IDictionary<string, T> destination, params IDictionary<string, T>[] sources) where T : ObjectList<T2>
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (sources == null)
            {
                return;
            }
            foreach (var source in sources)
            {
                foreach (var entry in source)
                {
                    if (destination.Keys.Contains(entry.Key))
                    {
                        T targetList;
                        destination.TryGetValue(entry.Key, out targetList);
                        if (targetList != null)
                        {
                            targetList.Concat(entry.Value);
                        }
                    }
                    else
                    {
                        destination.Add(entry.Key, entry.Value);
                    }
                }
            }
        }

    }
}
