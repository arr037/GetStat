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
        public event Action<Page> OnPageChanged;

        public void Navigate(BasePage page)
        {
            page.PageLoadAnimation = PageAnimation.None;
            page.PageUnloadAnimation = PageAnimation.SlideAndFadeOutToLeft;
            OnPageChanged?.Invoke(page);
            
        }

        public void NavigateWithAnimation(BasePage page,PageAnimation loadAnimation=PageAnimation.SlideAndFadeInFromRight,PageAnimation unloadAnimation= PageAnimation.SlideAndFadeOutToLeft)
        {
            page.PageLoadAnimation = loadAnimation;
            page.PageUnloadAnimation = unloadAnimation;
            OnPageChanged?.Invoke(page);
            
        }
    }

    public enum PageAnimation
    {
        None = 0,
        SlideAndFadeInFromRight = 1,
        SlideAndFadeOutToLeft = 2,
        SlideAndFadeInFromTop = 3,
        SlideAndFadeOutToBottom = 4,
    }
}