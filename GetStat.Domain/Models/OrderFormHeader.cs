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
        public string MaxQuestion { get;  }
        public DateTime StartDay { get;  }
        public TimeSpan StartTime { get;  }
        public TimeSpan EndTime { get;  }
        public TimeSpan DeadLine { get; }

        public int PageIndex { get; }
        public int PageCount { get; }

        public int PageIndexPlus1 => PageIndex + 1;

        public OrderFormHeader(
                string targetName,
                int pageIndex,
                int pageCount,
                DateTime startDay,
                TimeSpan startTime,
                TimeSpan endTime,
                string maxQuestion,TimeSpan deadLine)
        {
            TestName = targetName;
            PageIndex = pageIndex;
            PageCount = pageCount;
            StartDay = startDay;
            StartTime = startTime;
            EndTime = endTime;
            MaxQuestion = maxQuestion;
            DeadLine = deadLine;
        }

        public OrderFormHeader UpdatePageIndexCount(int pageIndex, int pageCount)
        {
            return
                new OrderFormHeader(
                    TestName,
                    pageIndex,
                    pageCount,StartDay,StartTime,EndTime,MaxQuestion,DeadLine
                );
        }
    }
}
