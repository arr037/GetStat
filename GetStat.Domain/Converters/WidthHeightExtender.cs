﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GetStat.Domain.Converters
{
    public class WidthHeightExtender : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string[] str = (parameter as string).Split(',');
                if (str[0].Equals("+"))
                    return System.Convert.ToDouble(value) + System.Convert.ToDouble(str[1]);
                else if (str[0].Equals("-"))
                    return System.Convert.ToDouble(value) - System.Convert.ToDouble(str[1]);
                else
                    return System.Convert.ToDouble(value);
            }
            catch (Exception ex)
            {
                return System.Convert.ToDouble(value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
