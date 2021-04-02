using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models
{
    public class QueueTest
    {
        public int QueueTestId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string FullName { get; set; }
        public string ConnectionId { get; set; }
    }
}
