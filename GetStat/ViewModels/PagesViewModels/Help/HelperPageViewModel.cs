using GetStat.Commands;
using GetStat.Pages.Authorization;
using GetStat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GetStat.Domain.Services;
using GetStat.Pages.Main;

namespace GetStat.ViewModels.PagesViewModels.Help
{
    public class HelperPageViewModel : BaseVm
    {
        private readonly PageService pageService;
        private readonly LoginResponseService _loginResponseService;
        private readonly MediaPlayerService _mediaPlayerService;
        public int Volume { get; set; }
        public bool IsAutoPlay { get; set; }
        public HelperPageViewModel(PageService pageService,LoginResponseService loginResponseService,MediaPlayerService mediaPlayerService)
        {
            this.pageService = pageService;
            _loginResponseService = loginResponseService;
            _mediaPlayerService = mediaPlayerService;
            IsAutoPlay= _mediaPlayerService.IsAutoPlay;
            Volume=_mediaPlayerService.Volume;
        }

        public ICommand CheckPlayCommand => new DelegateCommand(() =>
        {
            _mediaPlayerService.IsAutoPlay = IsAutoPlay;
            _mediaPlayerService.Volume = Volume;
            _mediaPlayerService.Play(true);
        });

        public ICommand SaveCommand => new DelegateCommand(() =>
        {
            GetStatApp.Default.IsAutoPlay = IsAutoPlay;
            GetStatApp.Default.Volume= Volume;
            GetStatApp.Default.Save();

        });

        public ICommand BackCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrEmpty(_loginResponseService.LoginResponse.Token))
            {
                pageService.NavigateWithAnimation(new EnterCodePage(), PageAnimation.SlideAndFadeInFromTop, PageAnimation.SlideAndFadeOutToBottom);
            }
            else
            {
                pageService.NavigateWithAnimation(new MainPage(), PageAnimation.SlideAndFadeInFromTop, PageAnimation.SlideAndFadeOutToBottom);
            }
        });
    }
}
