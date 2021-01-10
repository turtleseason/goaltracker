using GoalTracker.ViewModels;
using System;
using System.Windows;

namespace GoalTracker.Windows
{
    public partial class DailyGoalsWindow : Window
    {
        public DailyGoalsWindow(DateTime date, IGoalTrackerService gtService)
        {
            DailyGoalsWindowViewModel vm = new DailyGoalsWindowViewModel(date, gtService);
            vm.RequestClose += Close;
            DataContext = vm;

            InitializeComponent();
        }
    }
}
