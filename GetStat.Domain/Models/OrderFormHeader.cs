using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Reporting
{
    /// <summary>
    /// Represents a header part of an order form.
    /// </summary>
    public sealed class OrderFormHeader
    {
        public string TestName { get; }
        public string MaxQuestion { get; set; }
        public DateTime StartDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan DeadLine { get; set; }

        public int PageIndex { get; }
        public int PageCount { get; }

        public int PageIndexPlus1 => PageIndex + 1;

        public OrderFormHeader(
                string targetName,
                int pageIndex,
                int pageCount)
        {
            TestName = targetName;
            PageIndex = pageIndex;
            PageCount = pageCount;
        }

        public OrderFormHeader UpdatePageIndexCount(int pageIndex, int pageCount)
        {
            return
                new OrderFormHeader(
                    TestName,
                    pageIndex,
                    pageCount
                );
        }
    }
}
