using System.Windows;
using GetStat.Commands;
using GetStat.Models;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class ConfirmEmailViewModel : BaseVM
    {
        private readonly AuthorizationService _authorizationService;
        private readonly PageService _pageService;

        public ConfirmEmailViewModel(PageService pageService, AuthorizationService authorizationService)
        {
            _pageService = pageService;
            _authorizationService = authorizationService;
        }

        public string Code { get; set; }

        public DelegateCommand CheckCode => new DelegateCommand(async () =>
        {
            var res = await _authorizationService.ConfirmEmail();
            if (res)
                MessageBox.Show("okay");
            else
                MessageBox.Show("no");
        });

        public DelegateCommand GoBack =>
            new DelegateCommand(() =>
            {
                _pageService.GoToBack();
            }, o => _pageService.CanGoToBack);
    }
}