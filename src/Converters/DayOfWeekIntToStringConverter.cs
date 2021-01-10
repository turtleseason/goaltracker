using System;
using System.Globalization;
using System.Windows.Data;

namespace GoalTracker.Converters
{
    class DayOfWeekIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
