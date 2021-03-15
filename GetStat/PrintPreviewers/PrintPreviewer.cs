using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DotNetKit.Windows.Documents;
using GetStat.Commands;
using GetStat.Models;
using GetStat.Printing;

namespace GetStat.PrintPreviewers
{
    public interface IPrintPreviewer
        : IDisposable
    {
        ScaleSelector ScaleSelector { get; }
    }

    public class PrintPreviewer<TPrintable>
        : BaseVM
        , IPrintPreviewer
    {
        readonly TPrintable printable;
        readonly Func<TPrintable, Size, IEnumerable> paginate;

        static readonly IReadOnlyList<PrintPreviewPage> emptyPages = new PrintPreviewPage[] { };

        IReadOnlyList<PrintPreviewPage> pages = emptyPages;
        public IReadOnlyList<PrintPreviewPage> Pages
        {
            get { return pages; }
            set { pages = value;OnPropertyChanged(nameof(pages)); }
        }

        public MediaSizeSelector MediaSizeSelector { get; } =
            new MediaSizeSelector();

        bool isLandscape;
        public bool IsLandscape
        {
            get { return isLandscape; }
            set { isLandscape = value; OnPropertyChanged(nameof(isLandscape)); }
        }

        public ScaleSelector ScaleSelector { get; } =
            new ScaleSelector();

        public PrinterSelector<IPrinter> PrinterSelector { get; }

        public DelegateCommand PreviewCommand { get; }

        public DelegateCommand PrintCommand { get; }

        Size PageSize
        {
            get
            {
                var mediaSize = MediaSizeSelector.SelectedSize;
                return IsLandscape ? new Size(mediaSize.Height, mediaSize.Width) : mediaSize;
            }
        }

        public void UpdatePreview()
        {
            var pageSize = PageSize;
            Pages =
                paginate(printable, PageSize)
                .Cast<object>()
                .Select(content => new PrintPreviewPage(content, pageSize))
                .ToArray();
        }

        public void Print()
        {
            var printer = PrinterSelector.SelectedPrinterOrNull;
            if (printer == null) return;

            var pageSize = PageSize;
            printer.Print(paginate(printable, pageSize), pageSize);
        }

        public void Dispose()
        {
            PrinterSelector.Dispose();
        }

        public
            PrintPreviewer(
                TPrintable printable,
                Func<TPrintable, Size, IEnumerable> paginate,
                PrinterSelector<IPrinter> printerSelector
            )
        {
            this.printable = printable;
            this.paginate = paginate;
            PrinterSelector = printerSelector;

            PreviewCommand = new DelegateCommand(UpdatePreview);
            PrintCommand = new DelegateCommand(Print);
        }
    }
}
