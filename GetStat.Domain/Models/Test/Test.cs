using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Test
{
    public class Test
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestId { get; set; }
        public List<Question> Questions { get; set; }
        public Setting Settings { get; set; }
        public string AccountId { get; set; }
        [NotMapped] public int QuestionCount { get; set; }
    }

}
