using NUnit.Framework;
using System;
using static GoalTracker.Util.DateUtil;

namespace Tests
{
    class DateUtilTests
    {
        private static DateTime[] testDates =
{
            new DateTime(2020, 10, 11),
            new DateTime(2020, 10, 12),
            new DateTime(2020, 10, 13),
            new DateTime(2020, 10, 14),
            new DateTime(2020, 10, 15),
            new DateTime(2020, 10, 16),
            new DateTime(2020, 10, 17),
            new DateTime(2020, 10, 17, 23, 59, 59, 999),
            new DateTime(2020, 10, 14, 14, 42, 42),
            new DateTime(2020, 10, 11, 0, 0, 0),
            new DateTime(2020, 10, 11, 0, 0, 0, 1),
            new DateTime(2020, 10, 10, 23, 59, 59, 999),
            new DateTime(2020, 10, 10, 23, 59, 59, 998),
            new DateTime(2020, 10, 10, 23, 59, 59, 997),
        };

        [TestCaseSource(nameof(testDates))]
        public void FirstDayOfWeek_Returns_ClosestPreviousSunday(DateTime inputDate)
        {
            DateTime firstDayOfWeek = FirstDayOfWeek(inputDate);

            Assert.That(firstDayOfWeek.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(inputDate - firstDayOfWeek, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(inputDate - firstDayOfWeek, Is.LessThan(new TimeSpan(7, 0, 0, 0)));
        }

        [TestCaseSource(nameof(testDates))]
        public void FirstDayOfWeek_Returns_DateWithoutTime(DateTime inputDate)
        {
            DateTime firstDayOfWeek = FirstDayOfWeek(inputDate);

            Assert.That(firstDayOfWeek.Date, Is.EqualTo(firstDayOfWeek));
        }

        [TestCaseSource(nameof(testDates))]
        public void LastDayOfWeek_Returns_ClosestFollowingSaturday(DateTime inputDate)
        {
            DateTime lastDayOfWeek = LastDayOfWeek(inputDate);

            Assert.That(lastDayOfWeek.DayOfWeek, Is.EqualTo(DayOfWeek.Saturday));
            Assert.That(lastDayOfWeek - inputDate, Is.GreaterThan(TimeSpan.FromDays(-1)));
            Assert.That(lastDayOfWeek - inputDate, Is.LessThanOrEqualTo(TimeSpan.FromDays(6)));
        }

        [TestCaseSource(nameof(testDates))]
        public void LastDayOfWeek_Returns_DateWithoutTime(DateTime inputDate)
        {
            DateTime lastDayOfWeek = LastDayOfWeek(inputDate);

            Assert.That(lastDayOfWeek.Date, Is.EqualTo(lastDayOfWeek));
        }

        [TestCaseSource(nameof(testDates))]
        public void LastDayOfMonth_Returns_LastDayOfMonth(DateTime inputDate)
        {
            DateTime lastDayOfMonth = LastDayOfMonth(inputDate);

            Assert.That(lastDayOfMonth.Month, Is.EqualTo(inputDate.Month));
            Assert.That(lastDayOfMonth.Year, Is.EqualTo(inputDate.Year));
            Assert.That(lastDayOfMonth.AddDays(1).Month, Is.Not.EqualTo(inputDate.Month));
        }

        [TestCaseSource(nameof(testDates))]
        public void LastDayOfMonth_Returns_DateWithoutTime(DateTime inputDate)
        {
            DateTime lastDayOfMonth = LastDayOfMonth(inputDate);

            Assert.That(lastDayOfMonth.Date, Is.EqualTo(lastDayOfMonth));
        }
    }
}
