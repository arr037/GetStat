using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Tabs;

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
    }
}
