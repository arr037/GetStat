using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using Dna;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
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


        public string Code { get; set; }
        public string FullName { get; set; }

        public JoinWithCodeViewModel(
            LoginResponseService loginResponseService,
            ModalService modalService,
            PageService pageService,
            EventBus eventBus)
        {
            _loginResponseService = loginResponseService;
            _modalService = modalService;
            _pageService = pageService;
            _eventBus = eventBus;
        }

        public ICommand JoinInTest=> new DelegateCommand(async () =>
        {
            var log = _loginResponseService.LoginResponse;
            string fullName = log.Surname + " " + log.Name + " " + log.MiddleName;

            var response = await WebRequests.PostAsync<ApiResponse<Test>>
            ("https://localhost:5001/api/test/JoinTest", new[]{Code,fullName},
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (!res.SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка",res.Message);
                return;
            }

            var result = response.ServerResponse.Response;

            _pageService.NavigateWithAnimation(new Pages.Main.Test.StartTest());
          
            
            await _eventBus.Publish(
                new OnStartTest(
                    result.Questions, 
                    result.Settings.TestName,
                    result.Settings.MaxQuestion,
                    result.Settings.DeadLine,
                    fullName,
                    result.TestId)
                );


        },code=>!string.IsNullOrWhiteSpace(Code));
    }

    
}
