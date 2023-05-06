using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RegBlaze.Presentation.Converters;

public class StringIsNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isNotString = value is not string text || string.IsNullOrEmpty(text);
        return isNotString ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}