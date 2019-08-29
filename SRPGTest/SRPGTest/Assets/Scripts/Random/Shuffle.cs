using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomUtils
{
    namespace Shuffle
    {
        /// <summary>
        /// A class containing utilities to shuffle collections
        /// Uses the Fisher-Yates shuffle algorithm
        /// </summary>
        public static class ShuffleUtils
        {
            public static void Shuffle<T>(this List<T> items)
            {
                int n = items.Count;
                while (n-- > 1)
                {
                    int k = RandomU.instance.RandomInt(0, n + 1);
                    T temp = items[k];
                    items[k] = items[n];
                    items[n] = temp;
                }
            }

            public static void Shuffle<T>(this T[] items)
            {
                int n = items.Length;
                while (n-- > 1)
                {
                    int k = RandomU.instance.RandomInt(0, n + 1);
                    T temp = items[k];
                    items[k] = items[n];
                    items[n] = temp;
                }
            }
        }
    }
}
