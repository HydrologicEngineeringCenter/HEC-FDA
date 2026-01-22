using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HEC.FDA.View.Utilities
{
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                return collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            if (value is IEnumerable enumerable)
            {
                return enumerable.GetEnumerator().MoveNext() ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
