using System;
using System.Globalization;
using Xamarin.Forms;

namespace Questonaut.Converters
{
    public class ActivityImageMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val;
            val = (int)value;
            if (val % 2 == 0)
            {
                return new Thickness(5, 0, 0, 0);
            }
            return new Thickness(-28, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
