using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Tabs;
using GetStat.Domain.Models.Test;

namespace GetStat.Domain.Extetrions
{
    public static class CollectionExtentions
    {
        public static ITab AddUnique<T>(this IList<T> collection,T item) where T: ITab
        {
            var first = collection.FirstOrDefault(x => x.Name == item.Name);
            if (first!=null)
                return first;

            collection.Add(item);
            return item;
        }
        public static IEnumerable<T> Do<T>(this IEnumerable<T> self, Action<T> action) {
            foreach(var item in self) {
                action(item);
                yield return item;
            }
        }
      

        public static ObservableCollection<T> ToRandom<T>(this IEnumerable<T> array)
        {
            Random rand = new Random();

            var list = array.ToList();
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = list[j];
                list[j] = list[i];
                list[i] = tmp;
            }

            return new ObservableCollection<T>(list);
        }

        public static List<T> ToRandom<T>(this IEnumerable<T> array, int value = -1)
        {
            Random rand = new Random();

            var list = array.ToList();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = list[j];
                list[j] = list[i];
                list[i] = tmp;
            }

            return value < 0 ? list : list.Take(value).ToList();
        }


        public static IEnumerable<TSource> Duplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            var grouped = source.GroupBy(selector);
            var moreThan1 = grouped.Where(i => i.IsMultiple());
            return moreThan1.SelectMany(i => i);
        }

        public static IEnumerable<TSource> Duplicates<TSource, TKey>(this IEnumerable<TSource> source)
        {
            return source.Duplicates(i => i);
        }

        public static bool IsMultiple<T>(this IEnumerable<T> source)
        {
            var enumerator = source.GetEnumerator();
            return enumerator.MoveNext() && enumerator.MoveNext();
        }
    }
}
