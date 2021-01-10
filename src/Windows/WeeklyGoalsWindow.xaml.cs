using GoalTracker.ViewModels;
using System;
using System.Windows;

namespace GoalTracker.Windows
{
    public partial class WeeklyGoalsWindow : Window
    {
        public WeeklyGoalsWindow(DateTime startDate, IGoalTrackerService gtService)
        {
            WeeklyGoalsWindowViewModel vm = new WeeklyGoalsWindowViewModel(startDate, gtService);
            vm.RequestClose += Close;
            DataContext = vm;

            InitializeComponent();
        }
    }
}
