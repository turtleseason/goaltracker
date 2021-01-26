using GoalTracker.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GoalTracker.ViewModels
{
    class RemoveGoalWindowViewModel
    {
        private readonly IGoalTrackerService gtService;
        private readonly IWindowService windowService;

        public RemoveGoalWindowViewModel(IGoalTrackerService gtService, IWindowService windowService)
        {
            this.gtService = gtService;
            this.windowService = windowService;

            CreateGoalsList(gtService.DailyGoals, gtService.WeeklyGoals);

            DeleteDataEntries = false;
            DeleteAll = true;
            DeleteAfterDate = DateTime.Today;

            RemoveGoalsCommand = new DelegateCommand<IList>(RemoveGoals);

            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke());
        }

        public event Action RequestClose;

        public DelegateCommand<IList> RemoveGoalsCommand { get; }

        public DelegateCommand CloseCommand { get; }

        public List<Goal> Goals { get; private set; }

        public int SelectedItemsCount { get; set; }

        public bool DeleteDataEntries { get; set; }

        public bool DeleteAll { get; set; }

        public DateTime DeleteAfterDate { get; set; }

        private void CreateGoalsList(IEnumerable<DailyGoal> dailyGoals, IEnumerable<WeeklyGoal> weeklyGoals)
        {
            Goals = new List<Goal>();
            if (dailyGoals != null)
            {
                Goals.AddRange(dailyGoals);
            }
            if (weeklyGoals != null)
            {
                Goals.AddRange(weeklyGoals); 
            }
        }

        private void RemoveGoals(IList selectedGoals)
        {
            // If deleting existing entries, show a confirmation dialog first.
            if (DeleteDataEntries && selectedGoals.Count > 0)
            {
                MessageBoxResult result = ShowConfirmationDialog(selectedGoals);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            foreach (Goal goal in selectedGoals)
            {
                gtService.DeleteGoal(goal);
                if (DeleteDataEntries)
                {
                    DateTime startDate = DeleteAll ? DateTime.MinValue : DeleteAfterDate;
                    gtService.DeleteGoalData(goal, startDate);
                }
            }

            RequestClose?.Invoke();
        }

        private MessageBoxResult ShowConfirmationDialog(IList selectedGoals)
        {
            string goalNamesString = JoinGoalNames(selectedGoals.Cast<Goal>().ToList());
            
            string dateString = DeleteAll ? string.Empty : $" on or after {DeleteAfterDate:MMMM dd, yyyy}";
            
            string message = $"Are you sure you want to delete all existing entries for {goalNamesString}{dateString}? This can't be undone.";

            return windowService.ShowMessage(message, "Confirm remove", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
        }

        private string JoinGoalNames(List<Goal> selectedGoals)
        {
            string result = string.Empty;
            for (int i = 0; i < selectedGoals.Count(); i++)
            {
                string name = selectedGoals[i].Name;

                if (i < selectedGoals.Count - 2)
                    result += $"\"{name}\", ";
                else if (i < selectedGoals.Count - 1)
                    result += $"\"{name}\"{(selectedGoals.Count > 2 ? "," : "")} and ";
                else
                    result += $"\"{name}\"";
            }
            return result;
        }
    }
}
