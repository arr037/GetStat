using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Event
{
    public class BaseEventMessage
    {
        public string FullName { get; set; }
        public string Text { get; set; }
    }
}
