using GoalTracker.Util;
using GoalTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace GoalTracker
{
    // Contains static objects with mock data for use in the designer preview.
    static class DesignerData
    {
        public static DateTime Date = new DateTime(2020, 9, 17);

        public static IGoalTrackerService DummyGTS
        {
            get
            {
                if (gts == null)
                {
                    gts = new GoalTrackerService();
                    UserData userData = new UserData()
                    {
                        DailyGoals = DesignerData.DailyGoals,
                        WeeklyGoals = DesignerData.WeeklyGoals,
                        Days = DesignerData.Days,
                        Weeks = DesignerData.Weeks
                    };
                    gts.LoadData(userData);
                }
                return gts;
            }
        }
        private static IGoalTrackerService gts;

        public static Day Day
        {
            get
            {
                if (day == null)
                {
                    day = new Day(Date);
                    day.Goals.Add(new DailyGoal("Daily Goal 1", true));
                    day.Goals.Add(new DailyGoal("Daily Goal Two"));
                    day.Goals.Add(new DailyGoal("daily threeee"));
                }
                return day;
            }
        }
        private static Day day;

        public static Week Week
        {
            get
            {
                if (week == null)
                {
                    week = new Week(Date);
                    week.Goals.Add(WeeklyGoal1);
                    week.Goals.Add(WeeklyGoal2);
                }
                return week;
            }
        }
        private static Week week;

        public static List<DailyGoal> DailyGoals
        {
            get
            {
                if (dailyGoals == null)
                {
                    dailyGoals = new List<DailyGoal>
                    {
                        new DailyGoal("Daily Goal 1", true),
                        new DailyGoal("Daily Goal Two"),
                        new DailyGoal("daily threeee")
                    };
                }
                return dailyGoals;
            }
        }
        private static List<DailyGoal> dailyGoals;

        public static List<WeeklyGoal> WeeklyGoals
        {
            get
            {
                if (weeklyGoals == null)
                {
                    weeklyGoals = new List<WeeklyGoal>();
                    weeklyGoals.Add(WeeklyGoal1);
                    weeklyGoals.Add(WeeklyGoal2);
                }
                return weeklyGoals;
            }
        }
        private static List<WeeklyGoal> weeklyGoals;

        public static Dictionary<DateTime, Day> Days
        {
            get
            {
                if (cachedDays == null)
                {
                    DateTime start = new DateTime(2020, 8, 30);
                    cachedDays = new Dictionary<DateTime, Day>();
                    for (int i = 0; i < 35; i++)
                    {
                        cachedDays.Add(start.AddDays(i), null);
                    }

                    Day day = new Day(new DateTime(2020, 9, 17));
                    day.Goals.Add(new DailyGoal("goal"));
                    cachedDays[new DateTime(2020, 9, 17)] = day;

                    day = new Day(new DateTime(2020, 9, 18));
                    day.Goals.Add(new DailyGoal("goal"));
                    day.Goals.Add(new DailyGoal("goal2", true));
                    cachedDays[new DateTime(2020, 9, 18)] = day;

                    day = new Day(new DateTime(2020, 9, 19));
                    day.Goals.Add(new DailyGoal("goal", true));
                    day.Goals.Add(new DailyGoal("goal2", true));
                    cachedDays[new DateTime(2020, 9, 19)] = day;
                }
                return cachedDays;
            }
        }
        private static Dictionary<DateTime, Day> cachedDays;

        public static Dictionary<DateTime, Week> Weeks
        {
            get
            {
                if (cachedWeeks == null)
                {
                    DateTime start = new DateTime(2020, 8, 30);
                    cachedWeeks = new Dictionary<DateTime, Week>();
                    for (int i = 0; i < 5; i++)
                    {
                        cachedWeeks.Add(start.AddDays(7 * i), null);
                    }

                    WeeklyGoal goal = new WeeklyGoal("goal", 1);
                    goal.DaysCompleted[0] = true;
                    WeeklyGoal goal2 = new WeeklyGoal("goal2", 1);
                    goal2.DaysCompleted[1] = true;

                    Week week = new Week(new DateTime(2020, 9, 13));
                    week.Goals.Add(goal);
                    week.Goals.Add(new WeeklyGoal("goal2", 2));
                    cachedWeeks[new DateTime(2020, 9, 13)] = week;

                    week = new Week(new DateTime(2020, 9, 20));
                    week.Goals.Add(new WeeklyGoal("goal", 2));
                    cachedWeeks[new DateTime(2020, 9, 20)] = week;

                    week = new Week(new DateTime(2020, 9, 27));
                    week.Goals.Add(goal);
                    week.Goals.Add(goal2);
                    week.Goals.Add(new WeeklyGoal("goal2", 1));
                    cachedWeeks[new DateTime(2020, 9, 27)] = week;
                }
                return cachedWeeks;
            }
        }
        private static Dictionary<DateTime, Week> cachedWeeks;

        public static WeeklyGoal WeeklyGoal1
        {
            get
            {
                WeeklyGoal goal = new WeeklyGoal("Weekly Goal 1", 1);
                goal.DaysCompleted[(int)Date.DayOfWeek] = true;
                return goal;
            }
        }

        public static WeeklyGoal WeeklyGoal2
        {
            get
            {
                return new WeeklyGoal("Weekly Goal Two", 3);
            }
        }

        public static DailyGoalsWindowViewModel DailyGoalsViewModel = new DailyGoalsWindowViewModel(Date, DummyGTS);

        public static WeeklyGoalsWindowViewModel WeeklyGoalsWindowViewModel = new WeeklyGoalsWindowViewModel(Date, DummyGTS);

        public static RemoveGoalWindowViewModel RemoveGoalWindowViewModel = new RemoveGoalWindowViewModel(DummyGTS, null);

        public static MainWindowViewModel MainWindowViewModel = new MainWindowViewModel(DummyGTS, null, Date);

        public static DesignerSkinEditorWindowViewModel SkinEditorWindowViewModel = new DesignerSkinEditorWindowViewModel();
    }

    // Calling SkinEditorWindowViewModel's constructor at design time throws exceptions, so rather than modifying the actual class
    // for the sake of design-time mock data, this mock version provides all the needed properties without actually deriving from it.
    class DesignerSkinEditorWindowViewModel
    {
        public DesignerSkinEditorWindowViewModel()
        {
            SkinProperty testProp = new SkinProperty("TestProperty", Colors.Black);
            SkinProperty optionalTestProp = new OptionalSkinProperty("OptionalProperty", testProp);
            SkinProperty stringProp = new SkinProperty("TestImagePath", null);
            SkinProperty doubleProp = new SkinProperty("TestPercentage", .42);

            GeneralColors = new List<SkinProperty>() { testProp, testProp, testProp };
            BackgroundImage = new List<SkinProperty>() { stringProp };

            CalendarDefault = new List<SkinProperty>() { testProp, testProp, testProp };
            Calendar0 = new List<SkinProperty>() { testProp, optionalTestProp };
            Calendar50 = new List<SkinProperty>() { testProp, optionalTestProp };
            Calendar100 = new List<SkinProperty>() { testProp, optionalTestProp };

            CalendarWeek = new List<SkinProperty>() { doubleProp, testProp, optionalTestProp };
            CalendarWeek0 = new List<SkinProperty>() { optionalTestProp, optionalTestProp };
            CalendarWeek50 = new List<SkinProperty>() { optionalTestProp, optionalTestProp };
            CalendarWeek100 = new List<SkinProperty>() { optionalTestProp, optionalTestProp };

            CalendarHover = new List<SkinProperty>() { testProp, testProp, testProp };
        }

        public DelegateCommand SaveCommand { get; protected set; }

        public List<SkinProperty> GeneralColors { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> BackgroundImage { get; set; } = new List<SkinProperty>();

        public List<SkinProperty> CalendarDefault { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> Calendar0 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> Calendar50 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> Calendar100 { get; set; } = new List<SkinProperty>();

        public List<SkinProperty> CalendarWeek { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> CalendarWeek0 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> CalendarWeek50 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> CalendarWeek100 { get; set; } = new List<SkinProperty>();

        public List<SkinProperty> CalendarHover { get; set; } = new List<SkinProperty>();
    }
}
