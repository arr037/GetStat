using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GetStat.Commands;
using GetStat.Models;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class ConfirmEmailViewModel:BaseVM
    {
        private readonly PageService _pageService;
        private readonly AuthorizationService _authorizationService;

        public ConfirmEmailViewModel(PageService pageService,AuthorizationService authorizationService)
        {
            _pageService = pageService;
            _authorizationService = authorizationService;
        }

        public string Code { get; set; }
        public DelegateCommand CheckCode=> new DelegateCommand(async () =>
        {
           var res=  await _authorizationService.ConfirmEmail();
           if (res)
           {
               MessageBox.Show("okay");

           }
           else
           {
               MessageBox.Show("no");
           }
        });

        public DelegateCommand GoBack=> new DelegateCommand(() =>
        {
            _pageService.GoToBack();
        },o => _pageService.CanGoToBack);
    }
}
