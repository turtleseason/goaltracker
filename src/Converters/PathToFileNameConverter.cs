using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace GoalTracker.Converters
{
    // Truncates a full file path to the file name only.
    class PathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.GetFileName(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
