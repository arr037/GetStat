using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GetStat.Animation;

namespace GetStat.Services
{
    public class PageService
    {
        private readonly Stack<BasePage> history;
        public PageService()
        {
            history = new Stack<BasePage>();
        }

        public bool CanGoToBack => history.Skip(1).Any();
        public event Action<BasePage> OnPageChanged;

        public void Navigate(BasePage page)
        {
            page.PageLoadAnimation = PageAnimation.None;
            page.PageUnloadAnimation = PageAnimation.SlideAndFadeOutToLeft;

            OnPageChanged?.Invoke(page);
            history?.Push(page);
        }

        public void NavigateWithAnimation(BasePage page,PageAnimation loadAnimation=PageAnimation.SlideAndFadeInFromRight,PageAnimation unloadAnimation= PageAnimation.SlideAndFadeOutToLeft)
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

    public enum PageAnimation
    {
        None = 0,
        SlideAndFadeInFromRight = 1,
        SlideAndFadeOutToLeft = 2
    }
}