using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Questions
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string Quest { get; set; }

        [NotMapped]
        public int CurrectAnswer =>
            Answers.Any(x => x.IsSelected) ? 
                Answers.First(x => x.IsSelected).AnswerId : -1;
        public List<Answer> Answers { get; set; }
        
    }
}
