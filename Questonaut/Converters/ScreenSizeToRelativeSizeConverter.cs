using System;
using System.Globalization;
using Questonaut.DependencyServices;
using Xamarin.Forms;

namespace Questonaut.Converters
{
    public class ScreenSizeToRelativeSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = DependencyService.Get<IScreenDimensions>().GetScreenHeight();
            return (double)height * (double)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //we do not need to convert anything back
            throw new NotImplementedException();
        }
    }
}
