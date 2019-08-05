using System;
using System.Globalization;
using Xamarin.Forms;

namespace Questonaut.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals("open"))
                return Color.FromHex("#EF4B4C");
            else if (value.Equals("closed"))
                return Color.FromHex("#52BF8B");
            else
                return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
