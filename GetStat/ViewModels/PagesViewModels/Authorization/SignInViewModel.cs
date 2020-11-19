using System.Threading.Tasks;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.ViewModels;
using GetStat.Helpers;
using GetStat.Models;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main;
using GetStat.Services;
using GetStat.ViewModels.Base;

namespace GetStat.ViewModels.PagesViewModels.Authorization
{
    public class SignInViewModel : BaseVM
    {
        private readonly PageService _pageService;
        private readonly AuthorizationService _authorizationService;
        public  bool IsLogging { get; set; }
        public SignInViewModel(PageService pageService,AuthorizationService authorizationService)
        {
            _pageService = pageService;
            _authorizationService = authorizationService;
            }

   


        public string UserName { get; set; }

        public ICommand SignIn => new DelegateCommand<IHavePassword>(async item =>
        {

            if (!IsLogging)
            {
                await RunCommandAsync(() => IsLogging, async () =>
                {
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
            }




        });


        public ICommand SignUp => new DelegateCommand( () =>
        {
            _pageService.NavigateWithAnimation(new SignUp());
        });

        public ICommand SignInWithCode=> new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new EnterCodePage());
        });
    }
}