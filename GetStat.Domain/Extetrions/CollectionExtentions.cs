using System;
using System.Collections.Generic;
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
      

        public static List<T> ToRandom<T>(this List<T> array)
        {
            Random rand = new Random();

            for (int i = array.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = array[j];
                array[j] = array[i];
                array[i] = tmp;
            }

            return array.ToList();
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
    }
}
