using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Pages.Authorization;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Authorization
{
    public class EnterCodePageViewModel
    {
        private readonly PageService _pageService;
        public string Code { get; set; }

        public EnterCodePageViewModel(PageService pageService)
        {
            _pageService = pageService;
        }

        public ICommand GetSignInPage => new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new SignIn());
        });
    }
}
