using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dna;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests.StartTest
{
    public class GetResultViewModel:BaseVM
    {
        private readonly LoginResponseService _loginResponseService;
        private readonly EventBus eventBus;
        private readonly ModalService modalService;

        public List<ResultTest> Tests { get; set; }
        public bool IsLoading { get; set; }
        public GetResultViewModel(LoginResponseService loginResponseService,EventBus eventBus,ModalService modalService)
        {
            _loginResponseService = loginResponseService;
            this.eventBus = eventBus;
            this.modalService = modalService;
            eventBus.Subscribe<OnOpenMenu>(LoadTest);
        }

        private async Task LoadTest(OnOpenMenu arg)
        {
            if (arg.MenuType != MenuType.ResultTest)
                return;

            if (!IsLoading)
            {
                await RunCommandAsync(() => IsLoading, async () =>
                {
                    var response = await WebRequests.PostAsync<ApiResponse<List<ResultTest>>>
                                 ("https://localhost:5001/api/test/GetResultTest",
                bearerToken: _loginResponseService.LoginResponse?.Token);

                    var res = response.DisplayErrorIfFailedAsync();
                    if (!res.SuccessFul)
                    {
                        modalService.ShowModalWindow("Ошибка", res.Message);
                       await eventBus.Publish(new OnCloseTab());
                        return;
                    }

                    Tests = response.ServerResponse.Response;
                });
            }
        }
    }
}
