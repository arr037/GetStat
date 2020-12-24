using System.Windows;
using GetStat.Commands;
using GetStat.Models;
using GetStat.Pages.Main;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class ConfirmEmailViewModel : BaseVM
    {
        private readonly AuthorizationService _authorizationService;
        private readonly ModalService _modalService;
        private readonly PageService _pageService;

        public ConfirmEmailViewModel(PageService pageService, 
            AuthorizationService authorizationService,
            ModalService modalService)
        {
            _pageService = pageService;
            _authorizationService = authorizationService;
            _modalService = modalService;
        }

        public DelegateCommand CheckCode => new DelegateCommand(async () =>
        {
            var res = await _authorizationService.ConfirmEmail();
            if (res)
                _pageService.NavigateWithAnimation(new MainPage());
            else
                _modalService.ShowModalWindow("Ошибка","Потдвердите свой email");
        });

        public DelegateCommand GoBack =>
            new DelegateCommand(() =>
            {
                _pageService.GoToBack();
            }, o => _pageService.CanGoToBack);
    }
}