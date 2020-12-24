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
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using Microsoft.AspNetCore.Mvc;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class MyTestViewModel:BaseVM
    {
        private readonly LoginResponseService _loginResponseService;
        public ObservableCollection<Test> Tests { get; set; }

        public MyTestViewModel(EventBus eventBus,LoginResponseService loginResponseService)
        {
            _loginResponseService = loginResponseService;
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

        public ICommand EditTest=> new DelegateCommand<Test>(test =>
        {

        });

        public ICommand GetResultTest => new DelegateCommand<Test>(test =>
        {

        });

        public ICommand CopyCode=> new DelegateCommand<string>(Clipboard.SetText);

        private async Task LoadTest(OnOpenMenu arg)
        {
            var res = await WebRequests.PostAsync<List<Test>>
            ("https://localhost:5001/api/test/GetMyTests",
                bearerToken:_loginResponseService.LoginResponse.Token
                
            );
            Tests = new ObservableCollection<Test>(res.ServerResponse);
        }

    }
}
