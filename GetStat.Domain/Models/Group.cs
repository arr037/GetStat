using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public int StudentCount { get; set; }
        public int Course { get; set; }

    }
}
