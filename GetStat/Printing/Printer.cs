// Copyright 2015 Michael Mairegger
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace GetStat.Printing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using GetStat.Domain.Models.Test;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;

    public class Printer : PrintProcessor
    {
        private readonly IEnumerable<ResultTest> _collToPrint;
        private readonly string testName;
        private readonly CustomPrintDimension _printDimensions = new CustomPrintDimension();
        private readonly PrintAppendixes _printingAppendix;

        public Printer(PrintAppendixes printingAppendix, IEnumerable<ResultTest> collToPrint,string testName)
        {
            _printingAppendix = printingAppendix;
            _collToPrint = collToPrint;
            this.testName = testName;
            FileName = "FileName";
            PrintDimension = _printDimensions;
        }

        public override UIElement GetHeader()
        {
            return new TextBlock
            {
                Text = "Название: " + testName + "\nДата: " + DateTime.Now,
                FontSize= 16,
                TextWrapping = TextWrapping.Wrap
            };
        }

        public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
        {
            Grid g = new Grid();
            borderBrush = Brushes.Black;
            g.Margin = new Thickness(0, 0, 0, 5);
            g.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = new GridLength(_printDimensions.WidthColumn1)
            });
            g.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = new GridLength(_printDimensions.WidthColumn2)
            });
            g.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = new GridLength(_printDimensions.WidthColumn3) 
            });
            g.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(_printDimensions.WidthColumn3)
            });
            var tb = new TextBlock
            { 
                Text = "Фио",
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            var t1 = new TextBlock
            { 
                Text = "Правильно",
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            var t2 = new TextBlock
            {
                Text = "Всего",
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            var t3 = new TextBlock
            { 
                Text = "Оценка",
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            Grid.SetColumn(tb, 0);
            Grid.SetColumn(t1, 1);
            Grid.SetColumn(t2, 2);
            Grid.SetColumn(t3, 3);

            g.Children.Add(tb);
            g.Children.Add(t1);
            g.Children.Add(t2);
            g.Children.Add(t3);
            reserveHeightOf = 30;
            return g;
        }

        public override IEnumerable<IPrintContent> ItemCollection()
        {
            return _collToPrint.Select(x => new ResultTestLineItem(x, _printDimensions));
        }

        protected override void PreparePrint()
        {
            base.PreparePrint();
            PrintDimension.Margin = new Thickness(50);

            if (_printingAppendix.HasFlag(PrintAppendixes.Summary))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Summary));
            }
            if (_printingAppendix.HasFlag(PrintAppendixes.Footer))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Footer));
            }
            if (_printingAppendix.HasFlag(PrintAppendixes.Header))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Header));
            }
        }
    }

    public static class UIElementExtensions
    {

        private static readonly Size MaxSize = new Size(double.MaxValue, double.MaxValue);

        public static Size ComputeDesiredSize(this UIElement uiElement)
        {
            uiElement.Measure(MaxSize);
            return uiElement.DesiredSize;
        }
    }

    public class GridTestContent : IPrintContent
    {
        public UIElement Content { 
            get
                {
                    var sp = new Grid();
                    sp.ColumnDefinitions.Add(new ColumnDefinition());
                    sp.ColumnDefinitions.Add(new ColumnDefinition());
                    sp.ColumnDefinitions.Add(new ColumnDefinition());
                    sp.ColumnDefinitions.Add(new ColumnDefinition());

                return sp;

            } 
        }
    }
}