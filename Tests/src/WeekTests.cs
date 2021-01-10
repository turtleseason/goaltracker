using GoalTracker;
using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.Serialization;
using static GoalTracker.Util.DateUtil;

namespace Tests
{
    public class WeekTests
    {
        private static DateTime sampleDate = new DateTime(2020, 10, 14);

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

        private static string testPath = "test.xml";


        [OneTimeTearDown]
        public void Cleanup()
        {
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
        }


        [TestCaseSource(nameof(testDates))]
        public void Date_Returns_FirstDayOfWeek(DateTime inputDate)
        {
            Week week = new Week(inputDate);

            Assert.That(week.Date, Is.EqualTo(FirstDayOfWeek(inputDate)));
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_EmptyList()
        {
            Week week = new Week(sampleDate);

            Assert.That(week.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_AfterAddingIncompleteGoals()
        {
            Week week = new Week(sampleDate);
            WeeklyGoal goal = new WeeklyGoal("", 1);

            week.Goals.Add(goal);

            Assert.That(week.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_AfterUpdatingGoal()
        {
            Week week = new Week(sampleDate);
            WeeklyGoal goal = new WeeklyGoal("", 1);
            goal.DaysCompleted[0] = true;

            week.Goals.Add(goal);
            goal.DaysCompleted[0] = false;

            Assert.That(week.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_AfterRemovingGoal()
        {
            Week week = new Week(sampleDate);
            WeeklyGoal goal = new WeeklyGoal("", 1);
            goal.DaysCompleted[0] = true;

            week.Goals.Add(goal);
            week.Goals.Remove(goal);

            Assert.That(week.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsCorrect_AfterAddingCompleteGoal()
        {
            Week week = new Week(sampleDate);
            WeeklyGoal goal = new WeeklyGoal("", 1);
            goal.DaysCompleted[0] = true;

            week.Goals.Add(goal);

            Assert.That(week.CompletedGoalsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void CompletedGoalsCount_IsCorrect_AfterUpdatingGoal()
        {
            Week week = new Week(sampleDate);
            WeeklyGoal goal = new WeeklyGoal("", 1);

            week.Goals.Add(goal);
            goal.DaysCompleted[0] = true;

            Assert.That(week.CompletedGoalsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void Week_Deserialized_Equals_Serialized()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Week));
            Week week = new Week(sampleDate);
            WeeklyGoal goal = new WeeklyGoal("", 1);
            goal.DaysCompleted[1] = true;

            using (FileStream file = File.Create(testPath))
            {
                serializer.WriteObject(file, week);
            }

            Week deserialized;
            using (FileStream file = File.OpenRead(testPath))
            {
                deserialized = (Week)serializer.ReadObject(file);
            }

            Assert.That(week.Date, Is.EqualTo(deserialized.Date));
            Assert.That(week.Goals, Is.EqualTo(deserialized.Goals));
        }
    }
}
