using System;

namespace GoalTracker.Util
{
    public static class DateUtil
    {
        /// <summary>
        /// Returns a DateTime where the date component is the first Sunday equal to
        /// or before the given date and the time component is zero.
        /// </summary>
        public static DateTime FirstDayOfWeek(DateTime date)
        {
            return date.Date.AddDays(-(int)date.DayOfWeek);
        }

        /// <summary>
        /// Returns a DateTime where the date component is the first Saturday equal to 
        /// or after the given date and the time component is zero.
        /// Note: If the given DateTime is a Saturday and the time component is not zero,
        /// the resulting DateTime will actually be earlier than the input DateTime.
        /// </summary>
        public static DateTime LastDayOfWeek(DateTime date)
        {
            return date.Date.AddDays(6 - (int)date.DayOfWeek);
        }

        /// <summary>
        /// Returns a DateTime where the date component is the last day
        /// in the same month + year of the given date and the time component is zero.
        /// </summary>
        public static DateTime LastDayOfMonth(DateTime date)
        {
            return date.AddDays(DateTime.DaysInMonth(date.Year, date.Month) - date.Day).Date;
        }
    }
}
