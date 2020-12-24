using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Test;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class TeacherResultViewModel:BaseVM
    {
        public ObservableCollection<ResultTest> ResultTests { get; set; }


        public TeacherResultViewModel()
        {
            
        }
    }
}
