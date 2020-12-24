using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Test
{
    public class Setting
    {
        public Guid SettingId { get; set; }
        public string TestName { get; set; }
        public string MaxQuestion { get; set; }
        public DateTime  StartDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Code { get; set; }

    }
}
