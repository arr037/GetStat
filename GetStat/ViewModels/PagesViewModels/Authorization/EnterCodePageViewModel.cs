using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using GetStat.Pages.Main;
using GetStat.Pages.Main.Test;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Authorization
{
    public class EnterCodePageViewModel:BaseVM
    {
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        private readonly EventBus _eventBus;
        public string Code { get; set; }
        public string FullName { get; set; }
        public bool IsLogging { get; set; }

        public EnterCodePageViewModel(PageService pageService,ModalService modalService,EventBus eventBus)
        {
            _pageService = pageService;
            _modalService = modalService;
            _eventBus = eventBus;
        }

        public ICommand GetStartTest => new DelegateCommand( async () =>
        {
            if (!IsLogging)
            {
                await RunCommandAsync(() => IsLogging, async () =>
                {
                    var response = await WebRequests.PostAsync<ApiResponse<Test>>
                        (Config.UrlAddress+"api/test/JoinTest", new[] { Code, FullName });

                    var res = response.DisplayErrorIfFailedAsync();

                    if (!res.SuccessFul)
                    {
                        _modalService.ShowModalWindow("Ошибка", res.Message);
                        return;
                    }

                    var result = response.ServerResponse.Response;

                    _pageService.NavigateWithAnimation(new StartTest());

                    await _eventBus.Publish(
                        new OnStartTest(
                            result.Questions,
                            result.Settings.TestName,
                            result.Settings.MaxQuestion,
                            result.Settings.DeadLine
                            , FullName,
                            result.TestId)
                    );
                });
            }
        });

        public ICommand GetSignInPage => new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new SignIn());
        });
    }
}
