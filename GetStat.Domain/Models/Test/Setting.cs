using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Test
{
    public class Setting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }
        public string TestName { get; set; }
        public string MaxQuestion { get; set; }
        public DateTime  StartDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan DeadLine { get; set; }
        public string Code { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }

    }
}
