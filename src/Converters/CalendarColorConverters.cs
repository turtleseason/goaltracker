using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GoalTracker.Converters
{
    class CalendarBackgroundColorConverter : IValueConverter
    {
        // If provided, "parameter" should be a Brush which is returned as the default/zero value
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return parameter ?? Application.Current.FindResource("CalendarBgBrush");
            }

            switch ((float)value)
            {
                case float n when n >= 1:
                    return Application.Current.FindResource("Calendar100BgBrush");
                case float n when n > .5:
                    return Application.Current.FindResource("Calendar50BgBrush");
                case float n when n > 0:
                    return Application.Current.FindResource("Calendar0BgBrush");
                default:
                    return parameter ?? Application.Current.FindResource("CalendarBgBrush");
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class CalendarForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object defaultBrush = parameter ?? Application.Current.FindResource("CalendarFgBrush");
            
            if (value == null)
            {
                return defaultBrush;
            }

            object brush = null;
            switch ((float)value)
            {
                case float n when n >= 1:
                    brush = Application.Current.TryFindResource("Calendar100FgBrush");
                    break;
                case float n when n > .5:
                    brush = Application.Current.TryFindResource("Calendar50FgBrush");
                    break;
                case float n when n > 0:
                    brush = Application.Current.TryFindResource("Calendar0FgBrush");
                    break;
            };
            return brush ?? defaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class CalendarWeekBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object defaultBrush = parameter ?? Application.Current.FindResource("CalendarWeekBgBrush");

            if (value == null)
            {
                return defaultBrush;
            }

            object brush = null;
            switch ((float)value)
            {
                case float n when n >= 1:
                    brush = Application.Current.FindResource("CalendarWeek100BgBrush");
                    break;
                case float n when n > .5:
                    brush = Application.Current.FindResource("CalendarWeek50BgBrush");
                    break;
                case float n when n > 0:
                    brush = Application.Current.FindResource("CalendarWeek0BgBrush");
                    break;
            };
            return brush ?? defaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class CalendarWeekForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object defaultColor = parameter ?? Application.Current.FindResource("CalendarWeekFgBrush");

            if (value == null)
            {
                return defaultColor;
            }

            object brush = null;
            switch ((float)value)
            {
                case float n when n >= 1:
                    brush = Application.Current.TryFindResource("CalendarWeek100FgBrush");
                    break;
                case float n when n > .5:
                    brush = Application.Current.TryFindResource("CalendarWeek50FgBrush");
                    break;
                case float n when n > 0:
                    brush = Application.Current.TryFindResource("CalendarWeek0FgBrush");
                    break;
            };
            return brush ?? defaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
