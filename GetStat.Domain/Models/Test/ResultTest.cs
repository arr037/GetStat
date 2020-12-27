using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models.Test
{
    public class ResultTest
    {
        public int ResultTestId { get; set; }
        public string FullName { get; set; }
        public int AllCountQuestion { get; set; }
        public int CorrectCountQuestion { get; set; }
        public List<ResultQueston> ResultQuestons { get; set; }
        public string TestName { get; set; }
        public int TestId { get; set; }
        public string AccountId { get; set; }
    }
}
