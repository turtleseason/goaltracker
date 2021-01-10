using GoalTracker;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.IO;
using System.Runtime.Serialization;

namespace Tests
{
    class WeeklyGoalTests
    {
        private string testPath = "test.xml";

        private WeeklyGoal GetWeeklyGoal(string name = "Weekly Goal 1", int requiredCount = 1)
        {
            return new WeeklyGoal(name, requiredCount);
        }


        [OneTimeTearDown]
        public void Cleanup()
        {
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
        }


        [Test]
        public void Count_Equals_Total_CompletedDays([Range(0, 7)] int daysCompleted)
        {
            WeeklyGoal goal = GetWeeklyGoal();

            for (int i = 0; i < daysCompleted; i++)
            {
                goal.DaysCompleted[i] = true;
            }

            Assert.That(goal.Count, Is.EqualTo(daysCompleted));
        }

        [TestCase(new int[] { })]
        [TestCase(new int[] { 3 })]
        [TestCase(new int[] { 1, 5 })]
        [TestCase(new int[] { 0, 2, 4, 6 })]
        public void DaysCompleted_Deserialized_Equals_Serialized(int[] daysCompleted)
        {
            // Arrange
            WeeklyGoal goal = GetWeeklyGoal();
            DataContractSerializer serializer = new DataContractSerializer(typeof(WeeklyGoal));

            foreach (int i in daysCompleted)
            {
                goal.DaysCompleted[i] = true;
            }

            // Act
            using (FileStream file = File.Create(testPath))
            {
                serializer.WriteObject(file, goal);
            }

            WeeklyGoal deserialized;
            using (FileStream file = File.OpenRead(testPath))
            {
                deserialized = (WeeklyGoal)serializer.ReadObject(file);
            }

            // Assert
            // ("Two arrays, collections or IEnumerables are considered equal if they have the same dimensions
            //  and if each of the corresponding elements is equal.")
            Assert.That(deserialized.DaysCompleted, Is.EqualTo(goal.DaysCompleted));
        }
    }
}