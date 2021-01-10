using System;
using System.Globalization;
using System.Windows.Data;

namespace GoalTracker.Converters
{
    // Escapes underscores in strings where they would otherwise be interpreted as designating an access key, such as in MenuItem headers.
    class EscapeUnderscoresConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Replace("_", "__");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
