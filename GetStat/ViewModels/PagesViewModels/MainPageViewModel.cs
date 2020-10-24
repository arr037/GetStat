using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class MainPageViewModel
    {
        private readonly AuthorizationService _authorizationService;
        private readonly PageService _pageService;

        public MainPageViewModel(AuthorizationService authorizationService,PageService pageService)
        {
            _authorizationService = authorizationService;
            _pageService = pageService;
        }

        //public ICommand LogOutCommand=> new DelegateCommand(async () =>
        //{
        //    await _authorizationService.LogOut();
        //    _pageService.Navigate(new SignIn());
        //});
    }
}
