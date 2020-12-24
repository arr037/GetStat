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
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main;
using GetStat.Pages.Main.Test;
using GetStat.Services;
using GetStat.ViewModels.PagesViewModels.Tests.StartTest;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class StartTestViewModel:BaseVM
    {
        private readonly EventBus _eventBus;
        private readonly PageService _pageService;
        private readonly LoginResponseService _loginResponseService;
        public string TestName { get;  set; }
        public string QuestionCount { get;  set; }
        public TimeSpan RemarkingTime { get;  set; }
        public List<Question> Questions { get; set; }
        public bool IsStartTest { get; set; } = true;
        public string FullName { get; set; }

        private DispatcherTimer timer;
        private int _testId { get; set; }
        public StartTestViewModel(
            EventBus eventBus,
            PageService pageService,
            LoginResponseService loginResponseService)
        {
            _eventBus = eventBus;
            _pageService = pageService;
            _loginResponseService = loginResponseService;
            Questions = new List<Question>();
            eventBus.Subscribe<OnStartTest>(LoadTest);
        }

       


        public ICommand EndTest=> new DelegateCommand(async () =>
        {
            await SendResult();
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
            if (_loginResponseService.LoginResponse==null)
            {
                _pageService.NavigateWithAnimation(new SignIn());
            }
            else
            {
                _pageService.NavigateWithAnimation(new MainPage());
            }
        });


        private async void Timer_Tick(object sender, EventArgs e)
        {
            RemarkingTime = RemarkingTime.Subtract(TimeSpan.FromSeconds(1));

            if (RemarkingTime == TimeSpan.Zero)
            {
                timer.Stop();
                await SendResult();
            }
        }

        private async Task SendResult()
        {
            var answers = new List<ResultQA>(Questions.Select(x => new ResultQA
            {
                AnswerId = x.Answers.FirstOrDefault(a => a.IsSelected).AnswerId,
                QuestionId = x.QuestionId
            }));


            var response = await WebRequests.PostAsync<ApiResponse<ResultTest>>
            ("https://localhost:5001/api/test/EndTest", content: new BaseResultQA
                {
                    ResultQas = answers,
                    TestId = _testId,
                    FullName = FullName
                },
                bearerToken: _loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();
            if (!res.SuccessFul)
            {
                MessageBox.Show(res.Message);
                return;
            }

            _pageService.NavigateWithAnimation(new ResultTestPage());

            await _eventBus.Publish(new OnResultTest(response.ServerResponse.Response));
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
