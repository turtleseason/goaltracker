using GoalTracker.Util;
using System;
using System.Collections.ObjectModel;
using static GoalTracker.Util.DateUtil;

namespace GoalTracker.ViewModels
{
    class WeeklyGoalsWindowViewModel
    {
        public WeeklyGoalsWindowViewModel(DateTime date, IGoalTrackerService gtService)
        {
            FirstDayOfWeek = FirstDayOfWeek(date);
            LastDayOfWeek = LastDayOfWeek(date);

            CreateWeeklyGoalsList(gtService.GetWeekForDate(date));

            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke());
        }

        public event Action RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DateTime FirstDayOfWeek { get; }
        public DateTime LastDayOfWeek { get; }

        public string DateLabel { get => FirstDayOfWeek.ToString("M") + " - " + LastDayOfWeek.ToString("M"); }

        public ObservableCollection<WeeklyGoalViewModel> WeeklyGoals { get; private set; }

        private void CreateWeeklyGoalsList(Week week)
        {
            if (week != null)
            {
                WeeklyGoals = new ObservableCollection<WeeklyGoalViewModel>();
                foreach (WeeklyGoal goal in week.Goals)
                {
                    WeeklyGoals.Add(new WeeklyGoalViewModel(goal));
                }
            }
        }
    }
}
