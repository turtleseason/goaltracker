using GoalTracker;
using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Tests
{
    public class DayTests
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
        public void Date_Returns_DateWithoutTime(DateTime inputDate)
        {
            Day day = new Day(inputDate);

            Assert.That(day.Date, Is.EqualTo(inputDate.Date));
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_EmptyList()
        {
            Day day = new Day(sampleDate);

            Assert.That(day.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_AfterAddingIncompleteGoals()
        {
            Day day = new Day(sampleDate);
            DailyGoal goal = new DailyGoal("");

            day.Goals.Add(goal);

            Assert.That(day.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_AfterUpdatingGoal()
        {
            Day day = new Day(sampleDate);
            DailyGoal goal = new DailyGoal("", true);

            day.Goals.Add(goal);
            goal.SetCompleted(false);

            Assert.That(day.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsZero_AfterRemovingGoal()
        {
            Day day = new Day(sampleDate);
            DailyGoal goal = new DailyGoal("", true);

            day.Goals.Add(goal);
            day.Goals.Remove(goal);

            Assert.That(day.CompletedGoalsCount, Is.Zero);
        }

        [TestCase]
        public void CompletedGoalsCount_IsCorrect_AfterAddingCompleteGoal()
        {
            Day day = new Day(sampleDate);
            DailyGoal goal = new DailyGoal("", true);

            day.Goals.Add(goal);

            Assert.That(day.CompletedGoalsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void CompletedGoalsCount_IsCorrect_AfterUpdatingGoal()
        {
            Day day = new Day(sampleDate);
            DailyGoal goal = new DailyGoal("");

            day.Goals.Add(goal);
            goal.SetCompleted(true);

            Assert.That(day.CompletedGoalsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void Day_Deserialized_Equals_Serialized()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Day));
            Day day = new Day(sampleDate);
            DailyGoal goal = new DailyGoal("", true);
            day.Goals.Add(goal);

            using (FileStream file = File.Create(testPath))
            {
                serializer.WriteObject(file, day);
            }

            Day deserialized;
            using (FileStream file = File.OpenRead(testPath))
            {
                deserialized = (Day)serializer.ReadObject(file);
            }

            Assert.That(day.Date, Is.EqualTo(deserialized.Date));
            Assert.That(day.Goals, Is.EqualTo(deserialized.Goals));
        }
    }
}
