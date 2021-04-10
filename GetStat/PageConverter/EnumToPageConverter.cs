using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GetStat.Domain.Models.Pages.Page;
using GetStat.Pages.Main.Test;

namespace GetStat.PageConverter
{
    public static class EnumToPageConverter
    {
        public static Page ConvertToPage(this MainPages page)
        {
            switch (page)
            {
                case MainPages.GoToTestPage:
                    return new JoinWithCode();
                    break;
                case MainPages.CreatePage:
                    return new CreateTestPage();
                case MainPages.MyTestPage:
                    return new MyTestsPage();
                case MainPages.ResultPage:
                    return new GetResultPage();
                case MainPages.RequestPage:
                    return new RequestPage();
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }
    }
}
