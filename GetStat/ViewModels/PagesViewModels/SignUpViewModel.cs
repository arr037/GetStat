using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Input;
using Dna;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using GetStat.Helpers;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Services;
using GetStat.ViewModels.Base;
using Microsoft.AspNetCore.Identity;

namespace GetStat.ViewModels.PagesViewModels
{
    public class SignUpViewModel:BaseVM
    {
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        private readonly AuthorizationService _authorizationService;
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }



        public SignUpViewModel(PageService pageService,ModalService modalService,AuthorizationService authorizationService)
        {
            _pageService = pageService;
            _modalService = modalService;
            _authorizationService = authorizationService;
        }

        public ICommand RegistrationCommand => new DelegateCommand<IHavePassword>(async (item) =>
        {
            if (!item.IsEquals)
            {
                _modalService.ShowModalWindow("Ошибка пароля","Пароли не совпадают!");
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

           var checkSignUp =  await _authorizationService.Register(account);
           if (checkSignUp)
           {
               _pageService.Navigate(new ConfirmEmail());
            }

            
            
            
        });
        public ICommand SignInCommand=> new DelegateCommand(() =>
        {
            _pageService.Navigate(new SignIn());
        });


        
    }

}
