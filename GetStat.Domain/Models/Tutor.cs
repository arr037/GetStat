using System;
using System.Collections.Generic;

namespace GetStat.Domain.Models
{
    public class Tutor
    {
        public Guid TutorId { get; set; }
        public List<Student> Students { get; set; }
    }
}