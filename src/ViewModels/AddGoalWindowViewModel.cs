using GoalTracker.Util;
using System;
using System.Windows;
using static GoalTracker.Goal;

namespace GoalTracker.ViewModels
{
    public class AddGoalWindowViewModel
    {
        public static int[] TargetValues { get; } = { 1, 2, 3, 4, 5, 6 };

        private readonly IGoalTrackerService gtService;
        private readonly IWindowService windowService;

        public AddGoalWindowViewModel(IGoalTrackerService gtService, IWindowService windowService)
        {
            this.gtService = gtService;
            this.windowService = windowService;

            IsWeekly = false;
            WeeklyTarget = TargetValues[0];
            AddData = false;
            AddDataStartDate = DateTime.Today;
            AddGoalCommand = new DelegateCommand(AddGoal, AddGoal_CanExecute);
            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke());
        }

        public event Action RequestClose;

        public DelegateCommand AddGoalCommand { get; }

        public DelegateCommand CloseCommand { get; }

        public string Name { get; set; }

        public bool IsWeekly { get; set; }

        public int WeeklyTarget { get; set; }

        public bool AddData { get; set; }

        public DateTime AddDataStartDate { get; set; }


        private bool AddGoal_CanExecute()
        {
            return Name != string.Empty && Name != null;
        }

        private void AddGoal()
        {
            Console.WriteLine("AddGoal");
            if (gtService.HasRemovedGoal(Name, IsWeekly ? GoalType.Weekly : GoalType.Daily))
            {
                MessageBoxResult result = ConfirmMerge();
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if (IsWeekly)
            {
                WeeklyGoal goal = gtService.CreateWeeklyGoal(Name, WeeklyTarget);
                if (goal == null)
                {
                    AlertDuplicateGoalName();
                    return;
                }

                if (AddData)
                {
                    gtService.AddGoalToWeeks(goal, AddDataStartDate);
                }
            }
            else
            {
                DailyGoal goal = gtService.CreateDailyGoal(Name);
                Console.WriteLine("goal = " + goal + ", AddData = " + AddData);
                if (goal == null)
                {
                    AlertDuplicateGoalName();
                    return;
                }

                if (AddData)
                {
                    Console.WriteLine("Adding data");
                    gtService.AddGoalToDays(goal, AddDataStartDate);
                }
            }

            RequestClose?.Invoke();
        }

        private MessageBoxResult ConfirmMerge()
        {
            string message = $"There is existing data for a previous goal named \"{Name}\". "
                + "If you create a new goal with the same name, the old data will be merged with the new goal.";

            if (IsWeekly)
            {
                message += "\n\n(Note: If the new goal's target differs from the old target, "
                    + "existing data will continue to use the old target; new data will use the new target.)";
            }

            message += "\n\nMerge data and continue?";

            return windowService.ShowMessage(message, "Merge data?", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
        }

        private void AlertDuplicateGoalName()
        {
            string message = $"A goal with the name \"{Name}\" already exists; please choose a different name.";
            windowService.ShowMessage(message, "Goal already exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
