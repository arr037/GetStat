using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Questions
{
    public class Answer
    {
        public string Ans { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}
