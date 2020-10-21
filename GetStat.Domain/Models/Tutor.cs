using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Models
{
    public class Tutor
    {
        public Guid TutorId { get; set; }
        public List<Student> Students { get; set; }

    }
}
