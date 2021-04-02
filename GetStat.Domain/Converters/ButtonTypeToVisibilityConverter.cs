using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GetStat.Services;

namespace GetStat.Domain.Converters
{
    public class ButtonTypeToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter!=null)
            {
                var elem = (ModalButton) value;
                var param = (ModalButton) parameter;

                if (elem == param)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
