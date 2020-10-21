using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Введите логин")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}
