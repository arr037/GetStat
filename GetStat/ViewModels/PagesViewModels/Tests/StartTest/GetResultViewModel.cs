using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Test;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests.StartTest
{
    public class GetResultViewModel:BaseVM
    {
        public List<Test> Tests { get; set; }

        public GetResultViewModel()
        {
            Tests = new List<Test>();
        }

        private async void LoadTests()
        {
            await Task.Run(async () =>
            {

            });
        }
    }
}
