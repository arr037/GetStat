using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Commands;

namespace GetStat.Domain.Models.Tabs
{
    public interface ITab
    {
        public string Name { get; set; }
        public Page Page { get; set; }
        public CancellationTokenSource cancellationToken { get; set; }
        ICommand CloseCommand { get; }
        event EventHandler CloseRequired;
    }

    public class Tab : ITab
    {
        public Tab()
        {
            CloseCommand = new DelegateCommand(() => CloseRequired?.Invoke(this,EventArgs.Empty));
        }

        public string Name { get; set; }
        public Page Page { get; set; }
        public ICommand CloseCommand { get; }
        public CancellationTokenSource cancellationToken { get; set; } = new CancellationTokenSource();

        public event EventHandler CloseRequired;
    }
}
