using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Services;

namespace GetStat.ViewModels
{
    public class MainViewModel : BaseVM

    {
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        public Page CurrentPage { get; set; }
        public string ModalTitle { get; set; }
        public string ModalText { get; set; }

        public MainViewModel(PageService pageService,ModalService modalService)
        {
            _pageService = pageService;
            _modalService = modalService;
            pageService.OnPageChanged += page => CurrentPage = page;
            pageService.Navigate(new SignIn());

            modalService.OnModalWindowChanged += (title, text) =>
            {
                ModalTitle = title;
                ModalText = text;
            };
            modalService.HideModalWindow();
        }

        public ICommand CloseModalWindowCommand=> new DelegateCommand(() =>
        {
            _modalService.HideModalWindow(); 
        });
    }
}