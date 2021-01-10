using GoalTracker.Windows;
using Microsoft.Win32;
using System;
using System.Windows;

namespace GoalTracker
{
    public interface IWindowService
    {
        // Shows a save file dialog prompt.
        // Returns a valid file path, or null if the user canceled the prompt.
        string PromptForSaveFilePath();

        // Shows an open file dialog prompt.
        // Returns an existing file path, or null if the user canceled the prompt.
        string PromptForOpenFilePath();

        MessageBoxResult ShowMessage(string messageText,
                                     string titleText="",
                                     MessageBoxButton buttons=MessageBoxButton.OK,
                                     MessageBoxImage icon=MessageBoxImage.None);

        void ShowDailyGoalsWindow(DateTime date, IGoalTrackerService gtService);
        void ShowWeeklyGoalsWindow(DateTime date, IGoalTrackerService gtService);
        void ShowAddGoalWindow(IGoalTrackerService gtService);
        void ShowRemoveGoalWindow(IGoalTrackerService gtService);

        void ShowSkinEditorWindow(EventHandler onClosed);
    }

    public class WindowService : IWindowService
    {
        public WindowService() { }

        // Any Windows created through the WindowService will have this window set as their parent.
        public Window ParentWindow { get; set; }

        public void ShowDailyGoalsWindow(DateTime date, IGoalTrackerService gtService)
        {
            var popup = new DailyGoalsWindow(date, gtService) { Owner = ParentWindow };
            popup.ShowDialog();
        }

        public void ShowWeeklyGoalsWindow(DateTime date, IGoalTrackerService gtService)
        {
            var popup = new WeeklyGoalsWindow(date, gtService) { Owner = ParentWindow };
            popup.ShowDialog();
        }

        public void ShowAddGoalWindow(IGoalTrackerService gtService)
        {
            // should WindowService be a parameter?
            // in case you need to use a different windowService for some reason?
            var popup = new AddGoalWindow(gtService, this) { Owner = ParentWindow };
            popup.ShowDialog();
        }

        public void ShowRemoveGoalWindow(IGoalTrackerService gtService)
        {
            var popup = new RemoveGoalWindow(gtService, this) { Owner = ParentWindow };
            popup.ShowDialog();
        }

        public void ShowSkinEditorWindow(EventHandler onClosed=null)
        {
            var window = new SkinEditorWindow(this) { Owner = ParentWindow };
            if (onClosed != null)
            {
                window.Closed += onClosed;
            }
            window.Show();
        }

        public MessageBoxResult ShowMessage(string messageText,
                                            string titleText="",
                                            MessageBoxButton buttons=MessageBoxButton.OK,
                                            MessageBoxImage icon=MessageBoxImage.None)
        {
            return MessageBox.Show(messageText, titleText, buttons, icon);
        }

        public string PromptForSaveFilePath()
        {
            return FilePrompt(new SaveFileDialog(), "New file");
        }

        public string PromptForOpenFilePath()
        {
            return FilePrompt(new OpenFileDialog(), "Load file");
        }

        private string FilePrompt(FileDialog dialog, string title)
        {
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML Files|*.xml|All files|*.*";
            dialog.InitialDirectory = Properties.Settings.Default.SaveDirectory;
            dialog.Title = title;

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
