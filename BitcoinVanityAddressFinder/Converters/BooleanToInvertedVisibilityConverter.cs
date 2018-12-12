using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BitcoinVanityAddressFinder.Converters
{
    public class BooleanToInvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Hidden : Visibility.Visible;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}