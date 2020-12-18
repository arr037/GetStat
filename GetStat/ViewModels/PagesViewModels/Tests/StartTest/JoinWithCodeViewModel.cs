using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Dna;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests.StartTest
{
    public class JoinWithCodeViewModel:BaseVM
    {
        private readonly LoginResponseService _loginResponseService;
        private readonly ModalService _modalService;

        public JoinWithCodeViewModel(LoginResponseService loginResponseService,
            ModalService modalService)
        {
            _loginResponseService = loginResponseService;
            _modalService = modalService;
        }

        public ICommand JoinInTest=> new DelegateCommand<string>(async code =>
        {
            var response = await WebRequests.PostAsync<ApiResponse<Test>>
            ("https://localhost:5001/api/test/JoinTest", code,
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (!res.SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка",res.Message);
                return;
            }

            _modalService.ShowModalWindow("Good","Все гуд тест найден\n"+response.ServerResponse.Response.Settings.StartDay);
        },code=>!string.IsNullOrWhiteSpace(code));
    }
}
