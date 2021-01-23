using GetStat.Domain.Models.Test;
using Mairegger.Printing.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GetStat.Printing
{
    public class ResultTestLineItem : IPrintContent
    {
        private readonly ResultTest myShownObject;
        private readonly CustomPrintDimension printDimensions;

        public ResultTestLineItem(ResultTest _myShownObject, CustomPrintDimension _printDimensions)
        {
            if (_myShownObject == null)
                throw new ArgumentNullException("myShownObject");

            this.myShownObject = _myShownObject;
            this.printDimensions = _printDimensions;
        }

        public UIElement Content 
        { 
            get
            {
                StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, };
                TextBlock fullname = new TextBlock
                {
                    Text = myShownObject.FullName,
                    FontWeight = FontWeights.Medium,
                    Width = printDimensions.WidthColumn1,
                    FontSize = 16,
                    TextWrapping  = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };

                TextBlock correct = new TextBlock
                {
                    Text = myShownObject.CorrectCountQuestion.ToString(),
                    Width = printDimensions.WidthColumn2,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };
                
                TextBlock all = new TextBlock 
                { 
                    Text = myShownObject.AllCountQuestion.ToString(),
                    Width = printDimensions.WidthColumn3,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };

                var s = ((double)myShownObject.CorrectCountQuestion / myShownObject.AllCountQuestion) * 100;
                var pr =  Math.Round(s).ToString(CultureInfo.InvariantCulture);

                TextBlock price = new TextBlock
                {
                    Text = pr,
                    Width = printDimensions.WidthColumn4,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };

                sp.Children.Add(fullname);
                sp.Children.Add(correct);
                sp.Children.Add(all);
                sp.Children.Add(price);

                return sp;
            } 
        }
    }
}
