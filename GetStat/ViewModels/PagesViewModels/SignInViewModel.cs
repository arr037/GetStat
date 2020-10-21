using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Helpers;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Services;
using GetStat.ViewModels.Base;

namespace GetStat.ViewModels.PagesViewModels
{
    public class SignInViewModel:BaseVM
    {
        private readonly PageService _pageService;
        public string UserName { get; set; }


        public SignInViewModel(PageService pageService)
        {
            _pageService = pageService;
        }

        public ICommand SignIn=> new DelegateCommand<IHavePassword>((item) =>
        {
            var pass = item?.SecureString.Unsecure();
        });


        public ICommand SignUp=> new DelegateCommand(() =>
        {
            _pageService.Navigate(new SignUp());
        });
    }
}
