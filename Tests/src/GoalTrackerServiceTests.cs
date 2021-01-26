using GoalTracker;
using NUnit.Framework;
using System;
using Tests.Util;
using static GoalTracker.Util.DateUtil;

namespace Tests
{
    class GoalTrackerServiceTests
    {
        private readonly DateTime date = new DateTime(2020, 9, 17);

        private static GoalTrackerService GetEmptyGTS()
        {
            UserData userData = new UserData();
            GoalTrackerService gts = new GoalTrackerService();
            gts.LoadData(userData);
            return gts;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            // Recreate the save directory before each test run (prevents SaveDataManager
            // from throwing exceptions if the save directory is missing).
            GoalTracker.Properties.Settings.Default.Reset();
        }

        [Test]
        public void AddGoalToDays_DoNotAddDuplicateGoals()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateDailyGoal("goal");
            Day day = gts.CreateDay(date);

            gts.AddGoalToDays(new DailyGoal("goal", true), date);

            Assert.That(day.Goals[0].Done, Is.False);
        }

        [Test]
        public void AddGoalToWeeks_AddGoalToCurrentWeek()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateWeeklyGoal("goal1", 1);  // CreateWeek only works if at least one WeeklyGoal already exists
            Week week = gts.CreateWeek(FirstDayOfWeek(date));
            
            gts.AddGoalToWeeks(new WeeklyGoal("goal2", 1), date);

            Assert.That(week.GoalsCount == 2);
        }

        [Test]
        public void AddGoalToWeeks_DoNotAddDuplicateGoals()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateWeeklyGoal("goal", 3);
            Week week = gts.CreateWeek(FirstDayOfWeek(date));

            gts.AddGoalToWeeks(new WeeklyGoal("goal", 4), date);

            Assert.That(week.Goals[0].Target == 3);
        }

        [Test]
        public void DeleteGoal_AddToRemovedGoals_IfDataRemains()
        {
            GoalTrackerService gts = GetEmptyGTS();
            DailyGoal goal = gts.CreateDailyGoal("goal");
            gts.CreateDay(date);

            gts.DeleteGoal(goal);

            Assert.That(gts.HasRemovedGoal(goal.Name, Goal.GoalType.Daily));
        }

        [Test]
        public void DeleteGoal_DoNotAddToRemovedGoals_IfNoDataRemains()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateDay(date);
            DailyGoal goal = gts.CreateDailyGoal("goal");

            gts.DeleteGoal(goal);

            Assert.That(!gts.HasRemovedGoal(goal.Name, Goal.GoalType.Daily));
        }

        [Test]
        public void DeleteGoalData_DailyGoal_DoDeleteOnStartDate()
        {
            GoalTrackerService gts = GetEmptyGTS();
            DailyGoal goal = gts.CreateDailyGoal("goal");
            Day day = gts.CreateDay(date);

            gts.DeleteGoalData(goal, date);

            Assert.That(day.GoalsCount, Is.Zero);
        }

        [Test]
        public void DeleteGoalData_DailyGoal_DoNotDeleteBeforeStartDate()
        {
            GoalTrackerService gts = GetEmptyGTS();
            DailyGoal goal = gts.CreateDailyGoal("goal");
            Day day = gts.CreateDay(date);
            gts.CreateDay(date.AddDays(1));

            gts.DeleteGoalData(goal, date.AddDays(1));

            Assert.That(day.GoalsCount, Is.GreaterThan(0));
        }

        [Test]
        public void DeleteGoalData_DailyGoal_RemoveDaysWithNoRemainingGoalData()
        {
            GoalTrackerService gts = GetEmptyGTS();
            DailyGoal goal = gts.CreateDailyGoal("goal");
            gts.CreateDay(date);
            EventRaisedTester daysChangedRaised = new EventRaisedTester(handler => gts.DaysChanged += handler);

            gts.DeleteGoalData(goal, date);

            Assert.That(gts.GetDayForDate(date), Is.Null);
            Assert.That(daysChangedRaised.TimesRaised, Is.EqualTo(1));
        }

        [Test]
        public void DeleteGoalData_DailyGoal_RemoveGoalFromRemovedGoals_WhenDeletedFromAllDays()
        {
            GoalTrackerService gts = GetEmptyGTS();
            DailyGoal goal = gts.CreateDailyGoal("goal");
            gts.CreateDailyGoal("goal2");
            gts.CreateDay(date);
            gts.DeleteGoal(goal);

            gts.DeleteGoalData(goal, date);

            Assert.That(gts.HasRemovedGoal(goal.Name, Goal.GoalType.Daily), Is.False);
        }

        [Test]
        public void DeleteGoalData_WeeklyGoal_DoNotDeleteFromCurrentWeek_WhenStartDateIsMidweek()
        {
            GoalTrackerService gts = GetEmptyGTS();
            WeeklyGoal goal = gts.CreateWeeklyGoal("goal", 1);
            Week week = gts.CreateWeek(FirstDayOfWeek(date)); 

            gts.DeleteGoalData(goal, date);   // Sept 17 2020 is a Thursday

            Assert.That(week.GoalsCount, Is.GreaterThan(0));
        }

        [Test]
        public void DeleteGoalData_WeeklyGoal_RemoveWeeksWithNoRemainingGoalData()
        {
            DateTime startDate = FirstDayOfWeek(date);
            GoalTrackerService gts = GetEmptyGTS();
            WeeklyGoal goal = gts.CreateWeeklyGoal("goal", 1);
            gts.CreateWeek(startDate);
            EventRaisedTester weeksChangedRaised = new EventRaisedTester(handler => gts.WeeksChanged += handler);

            gts.DeleteGoalData(goal, startDate);

            Assert.That(gts.GetWeekForDate(startDate), Is.Null);
            Assert.That(weeksChangedRaised.TimesRaised, Is.EqualTo(1));
        }

        [Test]
        public void DeleteGoalData_WeeklyGoal_RemoveGoalFromRemovedGoals_WhenDeletedFromAllWeeks()
        {
            DateTime startDate = FirstDayOfWeek(date);
            GoalTrackerService gts = GetEmptyGTS();
            WeeklyGoal goal = gts.CreateWeeklyGoal("goal", 1);
            gts.CreateWeeklyGoal("goal2", 1);
            gts.CreateWeek(startDate);
            gts.DeleteGoal(goal);

            gts.DeleteGoalData(goal, startDate);

            Assert.That(gts.HasRemovedGoal(goal.Name, Goal.GoalType.Weekly), Is.False);
        }

        [Test]
        public void CreateDay_ReturnsNull_IfDayAlreadyExists()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateDailyGoal("goal");
            gts.CreateDay(date);

            Assert.That(gts.CreateDay(date), Is.Null);
        }

        [Test]
        public void CreateDay_ReturnsNull_IfNoDailyGoalsExist()
        {
            GoalTrackerService gts = GetEmptyGTS();

            Assert.That(gts.CreateDay(date), Is.Null);
        }

        [Test]
        public void CreateDay_DayHasAllCurrentDailyGoals()
        {
            GoalTrackerService gts = GetEmptyGTS();
            DailyGoal goal1 = gts.CreateDailyGoal("goal");
            DailyGoal goal2 = gts.CreateDailyGoal("goal2");
            DailyGoal removedGoal = gts.CreateDailyGoal("goal3");
            gts.DeleteGoal(removedGoal);

            Day day = gts.CreateDay(date);

            Assert.That(day.Goals.Contains(goal1));
            Assert.That(day.Goals.Contains(goal2));
            Assert.That(!day.Goals.Contains(removedGoal));
        }

        [Test]
        public void CreateDay_RaisesDaysChangedOnSuccess()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateDailyGoal("goal");
            EventRaisedTester daysChangedRaised = new EventRaisedTester(handler => gts.DaysChanged += handler);

            gts.CreateDay(date);

            Assert.That(daysChangedRaised.TimesRaised, Is.EqualTo(1));
        }

        [Test]
        public void CreateWeek_ReturnsNull_IfWeekAlreadyExists()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateWeeklyGoal("goal", 1);
            gts.CreateWeek(FirstDayOfWeek(date));

            Assert.That(gts.CreateWeek(FirstDayOfWeek(date)), Is.Null);

        }

        [Test]
        public void CreateWeek_ReturnsNull_IfNoWeeklyGoalsExist()
        {
            GoalTrackerService gts = GetEmptyGTS();

            Assert.That(gts.CreateWeek(FirstDayOfWeek(date)), Is.Null);
        }

        [Test]
        public void CreateWeek_WeekHasAllCurrentWeeklyGoals()
        {
            GoalTrackerService gts = GetEmptyGTS();
            WeeklyGoal goal1 = gts.CreateWeeklyGoal("goal", 1);
            WeeklyGoal goal2 = gts.CreateWeeklyGoal("goal2", 2);
            WeeklyGoal removedGoal = gts.CreateWeeklyGoal("goal3", 3);
            gts.DeleteGoal(removedGoal);

            Week week = gts.CreateWeek(FirstDayOfWeek(date));

            Assert.That(week.Goals.Contains(goal1));
            Assert.That(week.Goals.Contains(goal2));
            Assert.That(!week.Goals.Contains(removedGoal));
        }

        [Test]
        public void CreateWeek_RaisesWeeksChangedOnSuccess()
        {
            GoalTrackerService gts = GetEmptyGTS();
            gts.CreateWeeklyGoal("goal", 1);
            EventRaisedTester weeksChangedRaised = new EventRaisedTester(handler => gts.WeeksChanged += handler);

            gts.CreateWeek(FirstDayOfWeek(date));

            Assert.That(weeksChangedRaised.TimesRaised, Is.EqualTo(1));
        }
    }
}
