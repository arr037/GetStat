using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests.StartTest
{
    public class GetResultPageViewModel:BaseVM
    {
        public List<ResultQueston> ResultQuestons { get; set; }
        public GetResultPageViewModel(EventBus eventBus)
        {
            eventBus.Subscribe<OnResultTest>(LoadResultTests);
        }

        private Task LoadResultTests(OnResultTest arg)
        {
            ResultQuestons = arg.List.ResultQuestons;

            return Task.CompletedTask;
        }

    }

    public class OnResultTest:IEvent{
        public ResultTest List { get; }

        public OnResultTest(ResultTest list)
        {
            List = list;
        }
    }
}
