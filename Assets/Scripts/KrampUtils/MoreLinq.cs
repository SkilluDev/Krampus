using System;
using System.Collections.Generic;
using System.Linq;

namespace KrampUtils {
    public static class MoreLinq {
        public static TItem MinBy<TItem, TSelector>(this IEnumerable<TItem> enumerable, Func<TItem, TSelector> selector) where TSelector : IComparable<TSelector> =>
            enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) < 0 ? a : b);

        public static TItem MaxBy<TItem, TSelector>(this IEnumerable<TItem> enumerable, Func<TItem, TSelector> selector) where TSelector : IComparable<TSelector> =>
        enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) > 0 ? a : b);


        public static TItem Nth<TItem>(this IEnumerable<TItem> enumerable, int n) {
            var it = enumerable.GetEnumerator();
            for (int i = 0; i < n; i++) if (!it.MoveNext()) throw new IndexOutOfRangeException("Enumerable cannot move to n-th element.");
            return it.Current;
        }

        public static TItem UnityRandomElement<TItem>(this IEnumerable<TItem> enumerable) =>
            enumerable.Nth(UnityEngine.Random.Range(0, enumerable.Count()));


        public static TItem UnityRandomElement<TItem>(this IEnumerable<TItem> enumerable, Random rand) =>
            enumerable.Nth(rand.Next(0, enumerable.Count()));

    }
}