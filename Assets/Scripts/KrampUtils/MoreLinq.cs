using System;
using System.Collections.Generic;
using System.Linq;

namespace KrampUtils {
    public static class MoreLinq {
        private class GenericComparer<TItem> : IComparer<TItem> {
            private readonly Func<TItem, TItem, int> m_func;
            public GenericComparer(Func<TItem, TItem, int> func) {
                this.m_func = func;
            }
            public int Compare(TItem x, TItem y) {
                return m_func(x, y);
            }
        }

        public static TItem MinBy<TItem, TSelector>(this IEnumerable<TItem> enumerable, Func<TItem, TSelector> selector) where TSelector : IComparable<TSelector> =>
            enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) < 0 ? a : b);

        public static TItem MaxBy<TItem, TSelector>(this IEnumerable<TItem> enumerable, Func<TItem, TSelector> selector) where TSelector : IComparable<TSelector> =>
            enumerable.Aggregate((a, b) => selector(a).CompareTo(selector(b)) > 0 ? a : b);

        public static IEnumerable<TItem> NullIfEmpty<TItem>(this IEnumerable<TItem> enumerable) => enumerable.Count() == 0 ? null : enumerable;

        public static TItem UnityRandomElement<TItem>(this IEnumerable<TItem> enumerable) =>
            enumerable.Count()>0?enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count())):default(TItem);


        public static TItem UnityRandomElement<TItem>(this IEnumerable<TItem> enumerable, Random rand) =>
            enumerable.ElementAt(rand.Next(0, enumerable.Count()));

        public static IEnumerable<TItem> OrderBy<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, TItem, int> func) {
            return enumerable.OrderBy(w => w, new GenericComparer<TItem>(func));
        }

        public static IEnumerable<TItem> EmptyIfNull<TItem>(this IEnumerable<TItem> enumerable) => enumerable ?? Array.Empty<TItem>();

    }
}
