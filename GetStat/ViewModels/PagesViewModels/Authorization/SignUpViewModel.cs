using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.Models;
using GetStat.Helpers;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Services;
using GetStat.ViewModels.Base;

namespace GetStat.ViewModels.PagesViewModels.Authorization
{
    public class SignUpViewModel : BaseVM
    {
        private readonly AuthorizationService _authorizationService;
        private readonly ModalService _modalService;
        private readonly PageService _pageService;


        public SignUpViewModel(PageService pageService, ModalService modalService,
            AuthorizationService authorizationService)
        {
            _pageService = pageService;
            _modalService = modalService;
            _authorizationService = authorizationService;
        }

        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }

        public ICommand RegistrationCommand => new DelegateCommand<IHavePassword>(async item =>
        {
            if (!item.IsEquals)
            {
                _modalService.ShowModalWindow("Ошибка пароля", "Пароли не совпадают!");
                return;
            }

            var account = new Account
            {
                UserName = UserName,
                Email = Email,
                PasswordHash = item.SecureString.Unsecure(),
                Surname = Surname,
                MiddleName = MiddleName,
                Name = Name
            };

            var checkSignUp = await _authorizationService.Register(account);
            if (checkSignUp) _pageService.Navigate(new ConfirmEmail());
        });

        public ICommand SignInCommand => new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new SignIn());
        });
    }
}