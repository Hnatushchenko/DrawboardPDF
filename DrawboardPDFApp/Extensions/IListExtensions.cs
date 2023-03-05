using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Extensions
{
    public static class IListExtensions
    {
        public static void Sort<T>(this IList list, Comparison<T> comparison)
        {
            ArrayList.Adapter(list).Sort(new ComparisonComparer<T>(comparison));
        }
    }

    public class ComparisonComparer<T> : IComparer<T>, IComparer
    {
        private readonly Comparison<T> comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return comparison(x, y);
        }

        public int Compare(object x, object y)
        {
            return comparison((T)x, (T)y);
        }
    }
}
