using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetKit.Windows.Controls;
using DotNetKit.Windows.Documents;
using GetStat.Domain.Models.Test;

namespace GetStat.Reporting
{
    public sealed class OrderFormPage
        : IDataGridPrintable<ResultTest>
    {
        public OrderFormHeader Header { get; }
        public IReadOnlyList<ResultTest> Items { get; }


        #region IDataGridPrintable
        IEnumerable<ResultTest> IDataGridPrintable<ResultTest>.Items => Items;

        object IDataGridPrintable<ResultTest>.CreatePage(IReadOnlyList<ResultTest> items, int pageIndex, int pageCount)
        {
            var header = Header.UpdatePageIndexCount(pageIndex, pageCount);
            return new OrderFormPage(header, items);
        }
        #endregion

        public OrderFormPage(OrderFormHeader header, IReadOnlyList<ResultTest> items)
        {
            Header = header;
            Items = items;
        }

        /// <summary>
        /// Constructs with random data.
        /// </summary>
        //public OrderFormPage()
        //{
        //    Items = new ObservableCollection<ResultTest>(); 
        //    Header =
        //        new OrderFormHeader(
        //            "Foo Bar Inc.",
        //            new DateTime(2017, 01, 15),
        //            Items.Sum(item => item.AllCountQuestion),
        //            pageIndex: 0,
        //            pageCount: 1
        //        );
        //}

    }
}
