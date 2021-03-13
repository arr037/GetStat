using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Test
{
    public class ResultQueston
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public byte[] Image { get; set; }
        public List<ResultAnswer> ResultAnswers { get; set; }
    }
}
