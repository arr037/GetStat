using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GetStat.Domain.Models
{
    public class Account : IdentityUser
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите ваше фамилию")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Введите ваше отчество")]
        public string MiddleName { get; set; }

        public override string PasswordHash { get; set; }
        public List<Test.Test> Tests { get; set; }

    }
}