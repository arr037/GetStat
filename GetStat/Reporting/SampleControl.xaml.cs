using DotNetKit.Windows.Documents;
using GetStat.Domain.Models.Test;
using GetStat.Printing;
using GetStat.PrintPreviewers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetStat.Reporting
{
    /// <summary>
    /// Логика взаимодействия для SampleControl.xaml
    /// </summary>
    public partial class SampleControl : Page
    {
        public SampleControl()
        {
            InitializeComponent();
        }

        public SampleControl(OrderFormHeader header,IReadOnlyList<ResultTest> resultTests)
        {
            InitializeComponent();

            var previewer =
                new PrintPreviewer<OrderFormPage>(
                    new OrderFormPage(header,resultTests),
                    new DataGridPrintablePaginator<ResultTest>().Paginate,
                    PrinterSelector<IPrinter>.FromLocalServer<IPrinter>(q => new Printer(q))
                );
            DataContext = previewer;

            Loaded += (sender, e) =>
            {
                previewer.UpdatePreview();
            };
        }
    }
}
