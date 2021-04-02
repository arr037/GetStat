using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Domain.ViewModels;
using GetStat.Domain.Web;
using GetStat.Models;
using GetStat.Pages.Authorization;
using GetStat.Pages.Help;
using GetStat.Pages.Main;
using GetStat.Pages.Main.Test;
using GetStat.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace GetStat.ViewModels.PagesViewModels.Authorization
{
    public class EnterCodePageViewModel:BaseVM
    {
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        private readonly EventBus _eventBus;
        private readonly CodeHubService _hubService;
        public string Code { get; set; }
        public string FullName { get; set; }
        public bool IsLogging { get; set; }

        public HubConnectionState ConnectionState { get; set; }
        public EnterCodePageViewModel(PageService pageService,ModalService modalService,EventBus eventBus, CodeHubService hubService)
        {
            _pageService = pageService;
            _modalService = modalService;
            _eventBus = eventBus;
            _hubService = hubService;

            eventBus.Subscribe<OnCancelRequestToHub>(CancelHubRequest);

            if (ConnectionState != HubConnectionState.Connected)
            {
                Task.Factory.StartNew(ConnectToServer);
            }

            hubService.OnConnectionStateChanged += (state) =>
            {
                ConnectionState = state;
            };
            hubService.RecieveAllowOrDeny += (flag,test) =>
            {
                if (flag == false)
                {
                    _modalService.ShowModalWindow("Ошибка","Администратор вам запретил прохождение теста!",ModalButton.Ok);
                    return;
                }
                _modalService.HideModalWindow();
                App.Current.Dispatcher.Invoke(() =>
                {
                    _pageService.NavigateWithAnimation(new StartTest());
                });

                _= _eventBus.Publish(
                    new OnStartTest(
                        test.Questions,
                        test.Settings.TestName,
                        test.Settings.MaxQuestion,
                        test.Settings.DeadLine
                        , FullName,
                        test.TestId)
                );
            };
            
            hubService.ReceiveJoinTest += (error) =>
            {
                if (string.IsNullOrWhiteSpace(error))
                {
                    modalService.ShowModalWindow("Ожидание", "Пожалуйста подождите...\n",ModalButton.Cancel);
                    return;
                }
                modalService.ShowModalWindow("Ошибка", error);
            };
        }

        private async Task CancelHubRequest(OnCancelRequestToHub arg)
        { 
            await _hubService.CancelJoinTest();
        }


        private async Task ConnectToServer()
        {
            await _hubService.Connect();
        }
        public ICommand Connect => new DelegateCommand(async () =>
        {
            await ConnectToServer();
        }, state => ConnectionState != HubConnectionState.Connected);

        public ICommand Disconnect => new DelegateCommand(async () =>
        {
            await _hubService.Disconnect();
        }, state => ConnectionState != HubConnectionState.Disconnected);

        public ICommand GetStartTest => new DelegateCommand( async () =>
        {
            if (!IsLogging)
            {
                await RunCommandAsync(() => IsLogging, async () =>
                {
                    await _hubService.JoinTest(FullName, Code,TimeZoneInfo.Utc.Id);
                });
            }
        },state=> ConnectionState==HubConnectionState.Connected);

        public ICommand GetSignInPage => new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new SignIn());
            _ = _hubService.Disconnect();
        });
    }
}
