using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GetStat.Domain.Models.Test
{
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }
        public string Quest { get; set; }
        public int CorrectAnswer { get; set; }
        public List<Answer> Answers { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }

    }
}
