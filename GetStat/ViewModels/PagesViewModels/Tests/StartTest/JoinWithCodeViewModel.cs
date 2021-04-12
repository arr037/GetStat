using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using GetStat.Commands;
using GetStat.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Domain.Web;
using GetStat.Models;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class JoinWithCodeViewModel:BaseVM
    {
        private readonly LoginResponseService _loginResponseService;
        private readonly ModalService _modalService;
        private readonly PageService _pageService;
        private readonly EventBus _eventBus;
        private readonly HubService _hubService;


        public string Code { get; set; }
        public string FullName { get; set; }

        public JoinWithCodeViewModel(
            LoginResponseService loginResponseService,
            ModalService modalService,
            PageService pageService,
            EventBus eventBus, HubService hubService)
        {
            _loginResponseService = loginResponseService;
            _modalService = modalService;
            _pageService = pageService;
            _eventBus = eventBus;
            _hubService = hubService;
            eventBus.Subscribe<OnCancelRequestToHub>(CancelHubRequest);
            hubService.RecieveAllowOrDeny += (flag, test) =>
            {
                if (flag == false)
                {
                    _modalService.ShowModalWindow("Ошибка", "Администратор вам запретил прохождение теста!", ModalButton.Ok);
                    return;
                }
                _modalService.HideModalWindow();
                App.Current.Dispatcher.Invoke(() =>
                {
                    _pageService.NavigateWithAnimation(new Pages.Main.Test.StartTest());
                });

                _ = _eventBus.Publish(
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
                    modalService.ShowModalWindow("Ожидание", "Пожалуйста подождите...\n", ModalButton.Cancel);
                    return;
                }
                modalService.ShowModalWindow("Ошибка", error);
            };
        }

        private async Task CancelHubRequest(OnCancelRequestToHub arg)
        {
            await _hubService.CancelJoinTest();
        }

        public ICommand JoinInTest=> new DelegateCommand(async () =>
        {
            var log = _loginResponseService.LoginResponse;
            string fullName = log.Surname + " " + log.Name + " " + log.MiddleName;

            await _hubService.JoinTest(fullName, Code,TimeZoneInfo.Utc.Id);
            

        },code=>!string.IsNullOrWhiteSpace(Code));
    }

    
}
