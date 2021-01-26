using GoalTracker;
using GoalTracker.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    class RemoveGoalWindowVMTests
    {
        private static DateTime testDate = new DateTime(2020, 9, 17);

        [Test]
        public void Goals_ContainsAllDailyAndWeeklyGoals()
        {
            List<DailyGoal> dailyGoals = new List<DailyGoal>() { new DailyGoal("goal"), new DailyGoal("goal2", true) };
            List<WeeklyGoal> weeklyGoals = new List<WeeklyGoal>() { new WeeklyGoal("goal3", 1) };

            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(mock => mock.DailyGoals).Returns(dailyGoals.AsReadOnly());
            mockGts.Setup(mock => mock.WeeklyGoals).Returns(weeklyGoals.AsReadOnly());

            RemoveGoalWindowViewModel vm = new RemoveGoalWindowViewModel(mockGts.Object, new Mock<IWindowService>().Object);

            Assert.That(dailyGoals, Is.SubsetOf(vm.Goals));
            Assert.That(weeklyGoals, Is.SubsetOf(vm.Goals));
        }

        [Test]
        public void RemoveGoals_RemovesAllSelectedGoals()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            RemoveGoalWindowViewModel vm = new RemoveGoalWindowViewModel(mockGts.Object, new Mock<IWindowService>().Object);

            List<Goal> goals = new List<Goal>() { new DailyGoal("goal") , new DailyGoal("goal2", true), new WeeklyGoal("goal3", 1) };

            vm.RemoveGoalsCommand.Execute(goals);

            foreach (Goal goal in goals)
            {
                mockGts.Verify(mock => mock.DeleteGoal(goal), Times.Once);
            }
        }

        [Test]
        public void RemoveGoals_DoesNotDeleteData_WhenDeleteDataEntriesIsFalse()
        {
            Mock <IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            RemoveGoalWindowViewModel vm = new RemoveGoalWindowViewModel(mockGts.Object, new Mock<IWindowService>().Object)
            {
                DeleteAfterDate = DateTime.MinValue,
                DeleteDataEntries = false
            };
            DailyGoal goal = new DailyGoal("goal");

            vm.RemoveGoalsCommand.Execute(new List<Goal>() { goal });

            mockGts.Verify(mock => mock.DeleteGoal(goal), Times.Once);
            mockGts.Verify(mock => mock.DeleteGoalData(goal, It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public void RemoveGoals_DeletesAllData_WhenDeleteAllIsTrue()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            RemoveGoalWindowViewModel vm = new RemoveGoalWindowViewModel(mockGts.Object, new Mock<IWindowService>().Object)
            {
                DeleteAfterDate = testDate,
                DeleteDataEntries = true,
                DeleteAll = true
            };
            DailyGoal goal = new DailyGoal("goal");

            vm.RemoveGoalsCommand.Execute(new List<Goal>() { goal });

            mockGts.Verify(mock => mock.DeleteGoalData(goal, DateTime.MinValue), Times.Once);
        }

        [Test]
        public void RemoveGoals_DeletesOnlyAfterDate_WhenDeleteAllIsFalse()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            RemoveGoalWindowViewModel vm = new RemoveGoalWindowViewModel(mockGts.Object, new Mock<IWindowService>().Object)
            {
                DeleteAfterDate = testDate,
                DeleteDataEntries = true,
                DeleteAll = false
            };
            DailyGoal goal = new DailyGoal("goal");

            vm.RemoveGoalsCommand.Execute(new List<Goal>() { goal });

            mockGts.Verify(mock => mock.DeleteGoalData(goal, testDate), Times.Once);
        }
    }
}
