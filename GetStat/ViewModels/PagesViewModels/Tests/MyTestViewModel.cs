using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
    public class MyTestViewModel:BaseVM
    {
        private readonly EventBus _eventBus;
        private readonly LoginResponseService _loginResponseService;
        private readonly ModalService _modalService;
        public ObservableCollection<Test> Tests { get; set; }

        public MyTestViewModel(EventBus eventBus,LoginResponseService loginResponseService,ModalService modalService)
        {
            _eventBus = eventBus;
            _loginResponseService = loginResponseService;
            _modalService = modalService;
            eventBus.Subscribe<OnOpenMenu>(LoadTest);
        }

        public ICommand DeleteTest => new DelegateCommand<Test>( async test =>
        {
           var val=  MessageBox.Show("Вы точно хотите удалить тест?", "Условие", MessageBoxButton.YesNo, MessageBoxImage.Warning,
                MessageBoxResult.No);

           if (val == MessageBoxResult.Yes)
           {
               var response = await WebRequests.PostAsync(
                   "https://localhost:5001/api/test/RemoveTest",test.TestId,bearerToken:_loginResponseService.LoginResponse.Token);

               if (response.StatusCode == HttpStatusCode.OK)
               {
                   Tests.Remove(test);
                   return;
               }

               MessageBox.Show("Произошла ошибка при удалении теста");

           }
           
        });

        public ICommand EditTest=> new DelegateCommand<Test>(async
            test =>
        {
            var response = await WebRequests.PostAsync<ApiResponse<Test>>
            ("https://localhost:5001/api/test/EditTest", test.TestId,
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (!res.SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка", res.Message);
                return;
            }

            await _eventBus.Publish(new OnEditTest(response.ServerResponse.Response));
        });

        public ICommand GetResultTest => new DelegateCommand<Test>(async test =>
        {
            var response = await WebRequests.PostAsync<ApiResponse<List<ResultTest>>>
            ("https://localhost:5001/api/test/GetResult", test.TestId,
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (!res.SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка", res.Message);
                return;
            }

            await _eventBus.Publish(new OnTeacherResult(response.ServerResponse.Response));

        });

        public ICommand CopyCode=> new DelegateCommand<string>(Clipboard.SetText);

        private async Task LoadTest(OnOpenMenu arg)
        {
            if(arg.MenuType!=MenuType.MyTest)
                return;

            var res = await WebRequests.PostAsync<ApiResponse<List<Test>>>
            ("https://localhost:5001/api/test/GetMyTests",
                bearerToken:_loginResponseService.LoginResponse.Token
                
            );

            if (!res.DisplayErrorIfFailedAsync().SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка",res.ErrorMessage);
                return;
            }

            Tests = new ObservableCollection<Test>(res.ServerResponse.Response);
        }

    }
}
