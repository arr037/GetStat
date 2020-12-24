using System.ComponentModel.DataAnnotations.Schema;

namespace GetStat.Domain.Models.Test
{
    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerId { get; set; }
        public string Ans { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
