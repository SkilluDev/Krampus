using System;
using System.Collections.Generic;
using System.Linq;

namespace KrampUtils {
    public static class MoreLinq {
        public static TItem MinBy<TItem, TSelector>(this IEnumerable<TItem> enumerable, Func<TItem, TSelector> selector) where TSelector : IComparable<TSelector> =>
            enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) < 0 ? a : b);

        public static TItem MaxBy<TItem, TSelector>(this IEnumerable<TItem> enumerable, Func<TItem, TSelector> selector) where TSelector : IComparable<TSelector> =>
            enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) > 0 ? a : b);

        public static IEnumerable<TItem> NullIfEmpty<TItem>(this IEnumerable<TItem> enumerable) => enumerable.Count() == 0 ? null : enumerable;

        public static TItem UnityRandomElement<TItem>(this IEnumerable<TItem> enumerable) =>
            enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count()));


        public static TItem UnityRandomElement<TItem>(this IEnumerable<TItem> enumerable, Random rand) =>
            enumerable.ElementAt(rand.Next(0, enumerable.Count()));

    }
}