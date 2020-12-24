using System;
using System.Globalization;
using System.Windows.Data;

namespace GetStat.Domain.Converters
{
    public class MultiValueConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int cor && values[1] is int all)
            {
               var s= ((double) cor / all)*100;
               return Math.Round(s).ToString(CultureInfo.InvariantCulture);
            }

            return 111.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}