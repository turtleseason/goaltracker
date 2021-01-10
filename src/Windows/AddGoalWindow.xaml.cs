using GoalTracker.ViewModels;
using System.Windows;

namespace GoalTracker.Windows
{
    public partial class AddGoalWindow : Window
    {
        public AddGoalWindow(IGoalTrackerService gtService, IWindowService windowService)
        {
            AddGoalWindowViewModel vm = new AddGoalWindowViewModel(gtService, windowService);
            vm.RequestClose += Close;
            DataContext = vm;

            InitializeComponent();
        }
    }
}
