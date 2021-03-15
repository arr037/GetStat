using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GetStat.Animation
{
    public static class PageAnimationHelper
    {
        public static async Task SlideAndFadeInFromRight(this Page page, float seconds)
        {
            var sb = new Storyboard();
            sb.AddSlideFromRight(seconds,page.WindowWidth);
            sb.AddFadeIn(seconds);
            sb.Begin(page);
            page.Visibility = Visibility.Visible;

            await Task.Delay((int) seconds * 1000);

        }

        public static async Task SlideAndFadeInFromBottom(this Page page, float seconds)
        {
            var sb = new Storyboard();
            sb.AddSlideFromBottom(seconds, page.WindowWidth);
            sb.AddFadeIn(seconds);
            sb.Begin(page);
            page.Visibility = Visibility.Visible;

            await Task.Delay((int)seconds * 1000);

        }

        public static async Task SlideAndFadeOutToLeft(this Page page, float seconds)
        {
            // Create the storyboard
            var sb = new Storyboard();
            // Add fade in animation
            //await Task.Delay((int)(seconds * 1000));
            // Add slide from right animation
            sb.AddSlideToLeft(seconds*2, page.WindowWidth);
            sb.AddFadeOut(seconds);


            // Start animating
            sb.Begin(page);


            // Make page visible
            
            page.Visibility = Visibility.Visible;


            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        public static async Task SlideAndFadeOutToBottom(this Page page, float seconds)
        {
            var sb = new Storyboard(); await Task.Delay((int)seconds * 1000);
            sb.AddSlideToBottom(seconds*2, page.WindowWidth);
            sb.AddFadeIn(seconds);
            sb.Begin(page);
            page.Visibility = Visibility.Visible;

            

            //// Create the storyboard
            //var sb = new Storyboard();
            //// Add fade in animation
            ////await Task.Delay((int)(seconds * 1000));
            //// Add slide from right animation
            //sb.AddSlideToBottom(seconds * 2, page.WindowWidth);
            //sb.AddFadeOut(seconds);


            //// Start animating
            //sb.Begin(page);


            //// Make page visible

            //page.Visibility = Visibility.Visible;


            //// Wait for it to finish
            //await Task.Delay((int)(seconds * 1000));
        }

    }
}
