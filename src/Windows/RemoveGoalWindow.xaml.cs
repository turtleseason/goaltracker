using GoalTracker.ViewModels;
using System.Windows;

namespace GoalTracker.Windows
{
    public partial class RemoveGoalWindow : Window
    {
        public RemoveGoalWindow(IGoalTrackerService gtService, IWindowService windowService)
        {
            RemoveGoalWindowViewModel vm = new RemoveGoalWindowViewModel(gtService, windowService);
            vm.RequestClose += Close;
            DataContext = vm;

            InitializeComponent();
        }
    }
}
