using GoalTracker.Util;
using System;
using System.Collections.ObjectModel;

namespace GoalTracker.ViewModels
{
    // PropertyChanged is raised for: CanCreateDay, DailyGoals
    class DailyGoalsWindowViewModel : ObservableObject
    {
        private readonly IGoalTrackerService gtService;

        public DailyGoalsWindowViewModel(DateTime date, IGoalTrackerService gtService)
        {
            this.gtService = gtService;

            Day day = gtService.GetDayForDate(date);
            Week week = gtService.GetWeekForDate(date);

            Date = date;
            CanCreateDay = day == null && gtService.DailyGoals.Count > 0;

            TrackDailyGoalsCommand = new DelegateCommand(TrackDailyGoals, () => CanCreateDay);
            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke());

            CreateDailyGoalsList(day);
            CreateWeeklyGoalsList(week);
        }

        public event Action RequestClose;

        public DelegateCommand TrackDailyGoalsCommand { get; }

        public DelegateCommand CloseCommand { get; }

        public DateTime Date { get; }

        public int DayOfWeek { get => (int)Date.DayOfWeek; }

        public bool CanCreateDay
        {
            get => canCreateDay;
            private set
            {
                canCreateDay = value;
                NotifyPropertyChanged();
            }
        }
        private bool canCreateDay;

        public ObservableCollection<DailyGoalViewModel> DailyGoals
        {
            get => dailyGoals;
            private set
            {
                dailyGoals = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<DailyGoalViewModel> dailyGoals;

        public ObservableCollection<WeeklyGoalDayViewModel> WeeklyGoals { get; private set; }

        private void TrackDailyGoals()
        {
            Day day = gtService.CreateDay(Date);
            CreateDailyGoalsList(day);
            CanCreateDay = false;
        }

        private void CreateDailyGoalsList(Day day)
        {
            if (day != null)
            {
                DailyGoals = new ObservableCollection<DailyGoalViewModel>();
                foreach (DailyGoal goal in day.Goals)
                {
                    DailyGoals.Add(new DailyGoalViewModel(goal));
                }
            }
        }

        private void CreateWeeklyGoalsList(Week week)
        {
            if (week != null)
            {
                WeeklyGoals = new ObservableCollection<WeeklyGoalDayViewModel>();
                foreach (WeeklyGoal goal in week.Goals)
                {
                    WeeklyGoals.Add(new WeeklyGoalDayViewModel(goal, DayOfWeek));
                }
            }
        }
    }
}
