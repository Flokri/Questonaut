using System;
using System.Globalization;
using Xamarin.Forms;

namespace Questonaut.Converters
{
    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date;
            DateTime.TryParse($"{value}", out date);

            DateTime date2 = DateTime.Today;

            int daysDiff = date2.Day - date.Day;


            if (daysDiff == 0)
                return "today";

            return $"{daysDiff} days ago";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
