using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models;
using GetStat.Models;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class AllowDenyViewModel:BaseVM
    {
       

        public ObservableCollection<BaseTestHub> TestHubs { get; set; }

        private void TestService_ReciveGetPermissionToEnterTheTest(List<BaseTestHub> obj)
        {
            TestHubs = new ObservableCollection<BaseTestHub>(obj);   
        }
    }
}
