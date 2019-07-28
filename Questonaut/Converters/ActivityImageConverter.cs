using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace Questonaut.Converters
{
    public class ActivityImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val;
            val = (int)value;
            if (val % 2 == 0)
            {
                // Do your translation lookup here, using whatever method you require
                return ImageSource.FromResource("Questonaut.SharedImages.activity_right.png", typeof(ImageResourceExtension).GetTypeInfo().Assembly);
            }

            // Do your translation lookup here, using whatever method you require
            return ImageSource.FromResource("Questonaut.SharedImages.activity_left.png", typeof(ImageResourceExtension).GetTypeInfo().Assembly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
