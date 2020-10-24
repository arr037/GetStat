using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class AboutUserViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string ShortName { get; set; }


        public AboutUserViewModel(AuthorizationService authorizationService)
        {
            Name = authorizationService.LoginResponse.Name;
            Surname = authorizationService.LoginResponse.Surname;
            MiddleName = authorizationService.LoginResponse.MiddleName;

            ShortName = Name?.FirstOrDefault() + Surname?.FirstOrDefault().ToString();
        }
    }
}
