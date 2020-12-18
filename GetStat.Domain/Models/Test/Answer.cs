using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Questions
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string Ans { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }    
    }
}
