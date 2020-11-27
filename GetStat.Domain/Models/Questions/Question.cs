using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Questions
{
    public class Question
    {
        public string Quest { get; set; }
        public int QuestionNum { get; set; }
        
        public ObservableCollection<Answer> Answers { get; set; }
        
    }
}
