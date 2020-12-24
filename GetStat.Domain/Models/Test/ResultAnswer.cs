using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Test
{
    public class ResultAnswer
    {
        public string Answer { get; set; }
        public bool IsCorrect { get; set; } = false;
        public bool IsUserCorrect { get; set; } = false;
    }
}
