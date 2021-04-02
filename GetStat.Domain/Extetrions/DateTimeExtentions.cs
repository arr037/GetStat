using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Extetrions
{
    public static class DateTimeExtentions
    {
        public static DateTime GmtToPacific(this DateTime dateTime, string timeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime,
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo));
        }
    }
}
