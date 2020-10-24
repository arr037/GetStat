﻿using System;

namespace GetStat.Domain.Models
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Group Group { get; set; }
        public Tutor Tutor { get; set; }
    }
}