using GoalTracker.Util;
using System;
using System.ComponentModel;
using System.Windows;

namespace GoalTracker.ViewModels
{
    public abstract class CalendarButtonViewModel : ObservableObject
    {
        protected readonly IGoalTrackerService gtService;

        // Child classes should set and update GoalPeriod as needed
        protected virtual IGoalPeriod GoalPeriod
        {
            get => goalPeriod;
            set
            {
                if (goalPeriod != value)
                {
                    if (goalPeriod != null)
                    {
                        PropertyChangedEventManager.RemoveHandler(goalPeriod, GoalPeriodPropertyChanged, string.Empty);
                    }
                    goalPeriod = value;
                    if (goalPeriod != null)
                    {
                        PropertyChangedEventManager.AddHandler(goalPeriod, GoalPeriodPropertyChanged, string.Empty);
                    }
                    NotifyGoalPeriodChanged();
                }
            }
        }
        protected IGoalPeriod goalPeriod;

        public CalendarButtonViewModel(DateTime date, IGoalTrackerService gtService)
        {
            Date = date;
            this.gtService = gtService;
        }

        public DateTime Date { get; }

        public bool HasData { get => GoalPeriod != null; }

        public int? GoalsCount { get => GoalPeriod?.GoalsCount; }
        public int? CompletedGoalsCount { get => GoalPeriod?.CompletedGoalsCount; }

        public float? CompletionPercentage
        {
            get => (GoalsCount != null && CompletedGoalsCount != null) ?
                (float)CompletedGoalsCount / GoalsCount : null;
        }

        protected void NotifyGoalPeriodChanged()
        {
            NotifyPropertyChanged(nameof(HasData));
            NotifyPropertyChanged(nameof(GoalsCount));
            NotifyPropertyChanged(nameof(CompletedGoalsCount));
            NotifyPropertyChanged(nameof(CompletionPercentage));
        }

        protected void GoalPeriodPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GoalPeriod.GoalsCount))
            {
                NotifyPropertyChanged(nameof(GoalsCount));
                NotifyPropertyChanged(nameof(CompletionPercentage));
            }
            if (e.PropertyName == nameof(GoalPeriod.CompletedGoalsCount))
            {
                NotifyPropertyChanged(nameof(CompletedGoalsCount));
                NotifyPropertyChanged(nameof(CompletionPercentage));
            }
        }
    }

    public class CalendarDayButtonViewModel : CalendarButtonViewModel
    {
        public CalendarDayButtonViewModel(DateTime date, IGoalTrackerService gtService)
            : base(date, gtService)
        {
            WeakEventManager<IGoalTrackerService, EventArgs>.AddHandler(gtService,
                nameof(gtService.DaysChanged), OnDaysChanged);
            SetGoalPeriod();
        }

        public DelegateCommand<CalendarButtonViewModel> ShowDayWindowCommand { get; set; }

        protected void OnDaysChanged(object sender, EventArgs e)
        {
            SetGoalPeriod();
        }

        protected void SetGoalPeriod()
        {
            GoalPeriod = gtService.GetDayForDate(Date);
        }
    }

    public class CalendarWeekButtonViewModel : CalendarButtonViewModel
    {
        public CalendarWeekButtonViewModel(DateTime date, IGoalTrackerService gtService)
            : base(date, gtService)
        {
            WeakEventManager<IGoalTrackerService, EventArgs>.AddHandler(gtService,
                nameof(gtService.WeeksChanged), OnWeeksChanged);
            SetGoalPeriod();
        }

        public DelegateCommand<CalendarButtonViewModel> ShowWeekWindowCommand { get; set; }
        public DelegateCommand<CalendarButtonViewModel> TrackWeekCommand { get; set; }

        protected void OnWeeksChanged(object sender, EventArgs e)
        {
            SetGoalPeriod();
        }

        protected void SetGoalPeriod()
        {
            GoalPeriod = gtService.GetWeekForDate(Date);
        }
    }
}
