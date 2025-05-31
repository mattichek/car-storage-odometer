using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace car_storage_odometer.Converters
{
    public class SortDirectionToArrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ListSortDirection direction)
            {
                return direction == ListSortDirection.Ascending ? "↑" : "↓";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}