using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.Models.Test;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class TeacherResultViewModel:BaseVM
    {
        public ObservableCollection<ResultTest> ResultTests { get; set; }

        public ICommand ShowStudentTest=> new DelegateCommand<ResultTest>(test =>
        {
            MessageBox.Show(test.FullName);
        });
    }
}
