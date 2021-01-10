using GoalTracker.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace GoalTracker.Windows
{
    public partial class MainWindow : Window
    {
        private readonly App app = App.Current as App;

        public MainWindow(MainWindowViewModel vm)
        {
            vm.NoDayDataToShow += AlertNoDayData;
            vm.NoWeekDataToShow += AlertNoWeekData;
            vm.ShowSaveFilePath += ShowSaveFilePath;

            vm.RequestChangeSkin += app.ChangeSkin;
            vm.RequestClose += Close;

            DataContext = vm;

            app.SkinChangedEvent += SkinChanged;

            InitializeComponent();

            themesList.Collection = app.AvailableSkins;
        }

        private void AlertNoDayData(bool hasUntrackedWeeklyGoals)
        {
            string message;
            if (hasUntrackedWeeklyGoals)
            {
                message = "Go to Goals > Add Goal to create a new daily goal,"
                    + "or click the button to the right to start tracking weekly goals for this week.";
            }
            else
            {
                message = "Start by going to Goals > Add Goal and adding a new goal to keep track of.";
            }

            MessageBox.Show(message, "No goals to show", MessageBoxButton.OK);
        }

        private void AlertNoWeekData()
        {
            string message = "Start by going to Goals > Add Goal and adding a new weekly goal to keep track of.";
            MessageBox.Show(message, "No goals to add", MessageBoxButton.OK);
        }

        private void ShowSaveFilePath(string filePath)
        {
            MessageBox.Show(filePath);
        }

        private void SkinChanged(object sender, object eventArgs)
        {
            // Bit of a hacky way to force-redraw child controls
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            DataContext = null;
            DataContext = vm;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Clears IsKeyboardFocused styling when a control loses focus via mouse click.
            // Todo: Preferably button keyboard focus styles would behave more like FocusVisualStyle
            // (applies whenever focused, but only if the most recent input device was the keyboard)
            Keyboard.ClearFocus();
        }
    }
}
