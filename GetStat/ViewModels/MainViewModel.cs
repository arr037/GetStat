using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main;
using GetStat.Pages.Main.Test;
using GetStat.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace GetStat.ViewModels
{
    public class MainViewModel : BaseVM

    {
        private readonly ModalService _modalService;
        private readonly EventBus _eventBus;
        private readonly PageService _pageService;
        public ModalButton Button { get; set; }
        public MainViewModel(PageService pageService, 
            ModalService modalService,EventBus eventBus,MediaPlayerService mediaPlayerService)
        {
            _= mediaPlayerService.Init();

            ReadJson();
            _pageService = pageService;
            _modalService = modalService;
            _eventBus = eventBus;
            pageService.OnPageChanged += page => CurrentPage = page;
            pageService.NavigateWithAnimation(new SignIn(),PageAnimation.SlideAndFadeInFromTop,PageAnimation.SlideAndFadeOutToLeft);

            modalService.OnModalWindowChanged += (title, text,button) =>
            {
                ModalTitle = title;
                ModalText = text;
                Button = button;
            };
            modalService.HideModalWindow();
        }

        public Page CurrentPage { get; set; }
        public string ModalTitle { get; set; }
        public string ModalText { get; set; }

        public ICommand CloseModalWindowCommand => new DelegateCommand(() => { _modalService.HideModalWindow(); });
        public ICommand CancelCommand => new DelegateCommand(() =>
        {
           _= _eventBus.Publish(new OnCancelRequestToHub());
           _modalService.HideModalWindow();
        });

        private void ReadJson()
        {
            var str = File.ReadAllText("Config.txt");
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new System.Exception("Пожалуйста введите url адрес сервера");
            }

            Config.UrlAddress = str;
        }

    }
}