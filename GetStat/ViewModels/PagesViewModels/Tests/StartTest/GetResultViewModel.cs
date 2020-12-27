using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dna;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests.StartTest
{
    public class GetResultViewModel:BaseVM
    {
        private readonly LoginResponseService _loginResponseService;
        public List<ResultTest> Tests { get; set; }

        public GetResultViewModel(LoginResponseService loginResponseService,EventBus eventBus)
        {
            _loginResponseService = loginResponseService;
            eventBus.Subscribe<OnOpenMenu>(LoadTest);
        }

        private async Task LoadTest(OnOpenMenu arg)
        {
            if (arg.MenuType != MenuType.ResultTest)
                return;

            var response = await WebRequests.PostAsync<ApiResponse<List<ResultTest>>>
            ("https://localhost:5001/api/test/GetResultTest",
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();
            if (!res.SuccessFul)
            {
                MessageBox.Show(res.Message);
                return;
            }

            Tests = response.ServerResponse.Response;
        }
    }
}
