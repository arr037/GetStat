using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Domain.Web;
using GetStat.Models;
using GetStat.Reporting;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class TeacherResultViewModel:BaseVM
    {
        private readonly LoginResponseService loginResponseService;
        private readonly ModalService modalService;
        private readonly EventBus eventBus;

        public ObservableCollection<ResultTest> ResultTests { get; set; }

        public TeacherResultViewModel(LoginResponseService loginResponseService,ModalService modalService,EventBus eventBus)
        {
            this.loginResponseService = loginResponseService;
            this.modalService = modalService;
            this.eventBus = eventBus;
        }

        public ICommand PrintCommand => new DelegateCommand(async() =>
        {
            if (ResultTests.Count > 0)
            {
                var response = await WebRequests.PostAsync<ApiResponse<Setting>>
                  (Config.UrlAddress + "api/test/GetTestHeader", ResultTests.First().TestId,
                      bearerToken: loginResponseService.LoginResponse?.Token);

                var res = response.DisplayErrorIfFailedAsync();

                if (!res.SuccessFul)
                {
                    modalService.ShowModalWindow("Ошибка", res.Message);
                    return;
                }

                IReadOnlyList<ResultTest> s = ResultTests.ToList().AsReadOnly();
                var st = response.ServerResponse.Response;
                var header = new OrderFormHeader(st.TestName, 1, 1)
                {
                    MaxQuestion = st.MaxQuestion,
                    StartTime = st.StartTime,
                    EndTime = st.EndTime,
                    DeadLine = st.DeadLine,
                    StartDay = st.StartDay
                };
                await eventBus.Publish(new OnPrintResultTest(header, s));
            }
            
        });


        public ICommand ShowStudentTest=> new DelegateCommand<ResultTest>(async test =>
        {
            var response = await WebRequests.PostAsync<ApiResponse<ResultTest>>
               (Config.UrlAddress+"api/test/GetResultQuestions", test.ResultTestId,
                   bearerToken: loginResponseService.LoginResponse?.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (res.SuccessFul==false)
            {
                modalService.ShowModalWindow("Ошибка", res.Message);
                return;
            }

            var s = response.ServerResponse.Response;

            await eventBus.Publish(new OnUserResult(s.ResultQuestons, s.FullName,s.CorrectCountQuestion,s.AllCountQuestion));
        });
    }
}
