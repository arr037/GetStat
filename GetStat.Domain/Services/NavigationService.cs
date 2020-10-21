using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GetStat.Services
{
    public class PageService
    {
        public event Action<Page> OnPageChanged;
        private Stack<Page> history;
        public bool CanGoToBack => history.Skip(1).Any();

        public PageService()
        {
            history = new Stack<Page>();
        }

        public void Navigate(Page page)
        {
            OnPageChanged?.Invoke(page);
            history?.Push(page);
        }

        public void GoToBack()
        {
            history.Pop();
            var page = history.Peek();
            OnPageChanged?.Invoke(page);
        }
    }
}
