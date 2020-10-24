using System.Windows.Input;
using GetStat.Animation;
using GetStat.Commands;
using GetStat.Domain.ViewModels;
using GetStat.Helpers;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main;
using GetStat.Services;
using GetStat.ViewModels.Base;

namespace GetStat.ViewModels.PagesViewModels
{
    public class SignInViewModel : BaseVM
    {
        private readonly PageService _pageService;
        private readonly AuthorizationService _authorizationService;


        public SignInViewModel(PageService pageService,AuthorizationService authorizationService)
        {
            _pageService = pageService;
            _authorizationService = authorizationService;
        }

        public string UserName { get; set; }

        public ICommand SignIn => new DelegateCommand<IHavePassword>(async item =>
        {
            //var pass = item?.SecureString.Unsecure();
            var model = new LoginViewModel
            {
                UserName = UserName,
                Password = item?.SecureString.Unsecure()
            };

             var b = await _authorizationService.Login(model);
             if (b)
             {
                 _pageService.Navigate(new MainPage());
             }


        });


        public ICommand SignUp => new DelegateCommand( () =>
        {
            _pageService.NavigateWithAnimation(new SignUp());
        });
    }
}