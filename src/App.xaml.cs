using GoalTracker.ViewModels;
using GoalTracker.Windows;
using System;
using System.Diagnostics;
using System.Windows;

namespace GoalTracker
{
    public partial class App : SkinnedApplication
    {
        public App() : base()
        {
            // Ignore many "System.Windows.ResourceDictionary Warning: 9 : Resource not found" warnings when reloading resource dictionaries;
            // as far as I can tell these are harmless warnings that happen for a split second while reloading dictionaries,
            // caused by DynamicResources temporarily not having a value to reference
            PresentationTraceSources.ResourceDictionarySource.Switch.Level = SourceLevels.Off;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FindAvailableSkins();
            LoadUserSkinOrDefault();

            GoalTrackerService gtService = new GoalTrackerService();
            gtService.LoadOrCreateDataFromDefaultPath();
            if (!gtService.IsUserDataLoaded)
            {
                AlertFailedToLoadSaveData(gtService.SaveFilePath);
            }
            WindowService windowService = new WindowService();

            MainWindowViewModel mainWindowVM = new MainWindowViewModel(gtService, windowService, DateTime.Today);
            MainWindow mainWindow = new MainWindow(mainWindowVM);
            windowService.ParentWindow = mainWindow;
            mainWindow.Show();
        }

        private void AlertFailedToLoadSaveData(string saveFilePath)
        {
            string message = $"Unable to create or load save data at {saveFilePath}\n\n"
                    + "Go to \"File > New save file\" to choose a new save location, "
                    + "or go to \"File > Load save file\" to load an existing file.";
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
