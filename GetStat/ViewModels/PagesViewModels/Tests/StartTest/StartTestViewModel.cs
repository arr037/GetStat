using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Dna;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Questions;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Pages.Main;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class StartTestViewModel:BaseVM
    {
        private readonly PageService _pageService;
        private readonly LoginResponseService _loginResponseService;
        public string TestName { get;  set; }
        public string QuestionCount { get;  set; }
        public TimeSpan RemarkingTime { get;  set; }
        public List<Question> Questions { get; set; }
        public bool IsStartTest { get; set; } = true;
        public string FullName { get; set; }

        private DispatcherTimer timer;
        private Guid _testId { get; set; }
        public StartTestViewModel(EventBus eventBus,PageService pageService,LoginResponseService loginResponseService)
        {
            _pageService = pageService;
            _loginResponseService = loginResponseService;
            Questions = new List<Question>();
            eventBus.Subscribe<OnStartTest>(LoadTest);
        }

       


        public ICommand EndTest=> new DelegateCommand(async () =>
        {
            var response = await WebRequests.PostAsync<ApiResponse<string>>
            ("https://localhost:5001/api/test/EndTest", content:new PassedTest
                {
                    Questions = Questions,
                    TestId = _testId
                },
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();
            if (!res.SuccessFul)
            {
                MessageBox.Show(res.Message);
                return;
            }

            MessageBox.Show(response.ServerResponse.Response);
        });

        public ICommand StartTest =>new DelegateCommand(() =>
        {
            IsStartTest = false;

            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

        });

     

        public ICommand CancelTestCommand=> new DelegateCommand(() =>
        {
            _pageService.GoToBack();
        });


        private void Timer_Tick(object sender, EventArgs e)
        {
            RemarkingTime = RemarkingTime.Subtract(TimeSpan.FromSeconds(1));

            if (RemarkingTime == TimeSpan.Zero)
            {
                timer.Stop();

            }
        }

        private Task LoadTest(OnStartTest test)
        {
            TestName = test.QuestionName;
            QuestionCount = test.QuestionCount;
            RemarkingTime = test.Time;
            Questions = test.Questions;
            FullName = test.FullName;
            _testId = test.TestId;
            return Task.CompletedTask;
        }
    }
}
