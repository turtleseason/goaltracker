using GoalTracker;
using GoalTracker.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static GoalTracker.Util.DateUtil;

namespace Tests
{
    class DailyGoalsWindowVMTests
    {
        private DateTime date = new DateTime(2020, 9, 17);

        [Test]
        public void DailyGoals_ContainsExistingDayGoals()
        {
            List<DailyGoal> goals = new List<DailyGoal>() { new DailyGoal("goal"), new DailyGoal("goal2", true) };
            Day day = new Day(date);
            day.Goals.Add(goals[0]);
            day.Goals.Add(goals[1]);

            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(mock => mock.GetDayForDate(date)).Returns(day);
            DailyGoalsWindowViewModel vm = new DailyGoalsWindowViewModel(date, mockGts.Object);

            List<DailyGoal> vmGoals = vm.DailyGoals.Select(dailyVm => new DailyGoal(dailyVm.Name, dailyVm.Done)).ToList();
            Assert.That(goals, Is.EquivalentTo(vmGoals));
        }

        [Test]
        public void WeeklyGoals_ContainsExistingWeekGoals()
        {
            List<WeeklyGoal> goals = new List<WeeklyGoal>() { new WeeklyGoal("goal", 1), new WeeklyGoal("goal2", 2) };
            goals[0].DaysCompleted[(int)date.DayOfWeek] = true;
            Week week = new Week(FirstDayOfWeek(date));
            week.Goals.Add(goals[0]);
            week.Goals.Add(goals[1]);

            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(mock => mock.DailyGoals).Returns(new List<DailyGoal>().AsReadOnly());
            mockGts.Setup(mock => mock.GetWeekForDate(date)).Returns(week);
            DailyGoalsWindowViewModel vm = new DailyGoalsWindowViewModel(date, mockGts.Object);

            List<Tuple<string, bool>> goalsPerDay = goals.Select(goal => new Tuple<string, bool>(goal.Name, goal.DaysCompleted[(int)date.DayOfWeek])).ToList();
            List<Tuple<string, bool>> vmGoalsPerDay = vm.WeeklyGoals.Select(goalVm => new Tuple<string, bool>(goalVm.Name, goalVm.Done)).ToList();
            Assert.That(goalsPerDay, Is.EquivalentTo(vmGoalsPerDay));
        }

        [Test]
        public void TrackDailyGoalsCommand_CanExecute_ReturnsFalse_WhenDayAlreadyExists()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(mock => mock.DailyGoals).Returns(new List<DailyGoal>() { new DailyGoal("goal") }.AsReadOnly());
            mockGts.Setup(mock => mock.GetDayForDate(date)).Returns(new Day(date));

            DailyGoalsWindowViewModel vm = new DailyGoalsWindowViewModel(date, mockGts.Object);

            Assert.That(vm.TrackDailyGoalsCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void TrackDailyGoalsCommand_CanExecute_ReturnsFalse_WhenNoDailyGoalsExist()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(mock => mock.DailyGoals).Returns(new List<DailyGoal>().AsReadOnly());

            DailyGoalsWindowViewModel vm = new DailyGoalsWindowViewModel(date, mockGts.Object);

            Assert.That(vm.TrackDailyGoalsCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void TrackDailyGoalsCommand_CreatesDayForDate()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(mock => mock.DailyGoals).Returns(new List<DailyGoal>() { new DailyGoal("goal") }.AsReadOnly());

            DailyGoalsWindowViewModel vm = new DailyGoalsWindowViewModel(date, mockGts.Object);

            vm.TrackDailyGoalsCommand.Execute(null);

            mockGts.Verify(mock => mock.CreateDay(date), Times.Once);
        }
    }
}
