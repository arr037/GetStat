using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Questions;

namespace GetStat.Domain.Models.Test
{
    public class Test
    {
        public Guid TestId { get; set; }
        public List<Question> Questions { get; set; }
        public Setting Settings { get; set; }
        public virtual string AccountId { get; set; }
    }
}
