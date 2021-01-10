using GoalTracker;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    class GoalPeriodTests
    {
        // Converts an array of ints into a Day object; each 0 in the input array represents an incomplete goal,
        // and each 1 (or other nonzero int) represents a completed goal.
        private static Day GetDayWithGoalsFromInts(int[] goalsListAsInts)
        {
            Day day = new Day(new DateTime(2020, 9, 17));

            for (int i = 0; i < goalsListAsInts.Length; i++)
            {
                bool isComplete = goalsListAsInts[i] != 0;
                DailyGoal g = new DailyGoal("goal", isComplete);
                day.Goals.Add(g);
            }

            return day;
        }

        [TestCase(new int[] { })]
        [TestCase(new int[] { 0 })]
        [TestCase(new int[] { 1 })]
        [TestCase(new int[] { 0, 0, 1 })]
        [TestCase(new int[] { 1, 1, 0 })]
        public void CompletedGoalsCount_Matches_Completed_Goals(int[] goalsListAsInts)
        {
            Day day = GetDayWithGoalsFromInts(goalsListAsInts);
            int numCompletedGoals = goalsListAsInts.Where(x => x != 0).Count();

            Assert.That(day.CompletedGoalsCount, Is.EqualTo(numCompletedGoals));
        }

        [TestCase(new int[] { })]
        [TestCase(new int[] { 0 })]
        [TestCase(new int[] { 1 })]
        [TestCase(new int[] { 0, 0, 1 })]
        [TestCase(new int[] { 1, 1, 0 })]
        public void GoalsCount_Matches_Total_Goals(int[] goalsListAsInts)
        {
            Day day = GetDayWithGoalsFromInts(goalsListAsInts);
            int numGoals = goalsListAsInts.Length;

            Assert.That(day.GoalsCount, Is.EqualTo(numGoals));
        }
    }
}
