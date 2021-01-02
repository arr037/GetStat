using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Commands;
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
        private readonly PageService _pageService;

        public MainViewModel(PageService pageService, ModalService modalService,SignalRTestService testService)
        {
            testService.Connect();
            _pageService = pageService;
            _modalService = modalService;
            pageService.OnPageChanged += page => CurrentPage = page;
            pageService.Navigate( new MainPage());

            modalService.OnModalWindowChanged += (title, text) =>
            {
                ModalTitle = title;
                ModalText = text;
            };
            modalService.HideModalWindow();
        }

        public Page CurrentPage { get; set; }
        public string ModalTitle { get; set; }
        public string ModalText { get; set; }

        public ICommand CloseModalWindowCommand => new DelegateCommand(() => { _modalService.HideModalWindow(); });

    }
}