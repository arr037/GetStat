using GetStat.Commands;
using GetStat.Pages.Authorization;
using GetStat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStat.ViewModels.PagesViewModels.Help
{
    public class HelperPageViewModel : BaseVm
    {
        private readonly PageService pageService;

        public HelperPageViewModel(PageService pageService)
        {
            this.pageService = pageService;
        }

        public ICommand BackCommand => new DelegateCommand(() =>
        {
            pageService.NavigateWithAnimation(new EnterCodePage(), PageAnimation.SlideAndFadeInFromTop, PageAnimation.SlideAndFadeOutToBottom);
        });
    }
}
