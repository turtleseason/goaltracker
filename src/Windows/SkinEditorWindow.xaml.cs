using GoalTracker.ViewModels;
using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;

namespace GoalTracker.Windows
{
    public partial class SkinEditorWindow : Window
    {
        private SkinEditorWindowViewModel viewModel;

        public SkinEditorWindow(WindowService windowService)
        {
            viewModel = new SkinEditorWindowViewModel(windowService);
            viewModel.RequestClose += Close;

            DataContext = viewModel;

            InitializeComponent();
        }
        
        private void FileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog() { Filter = GetImageFileFilter() };
            bool? result = dialog.ShowDialog();
            if (result ?? false)
            {
                SkinProperty prop = (SkinProperty)(sender as FrameworkElement).Tag;
                prop.Value = dialog.FileName;
            }
        }

        private void FileClearButton_Click(object sender, RoutedEventArgs e)
        {
            SkinProperty prop = (SkinProperty)(sender as FrameworkElement).Tag;
            prop.Value = null;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            viewModel.OnClosing();

            // Workaround for a bug(?) where closing a child window (SkinEditorWindow) minimizes the parent (MainWindow)
            // if the child window has ever opened a child window of its own: https://stackoverflow.com/q/19156373
            Owner = null;
        }

        private static string GetImageFileFilter()
        {
            // Lists each codec separately, which might be nice for saving images but is inconvenient for locating them:
            //string codecs = string.Join("|", ImageCodecInfo.GetImageEncoders().Select(x => $"{x.FormatDescription} ({x.FilenameExtension})|{x.FilenameExtension}"));

            string extensions = string.Join(";", ImageCodecInfo.GetImageEncoders().Select(x => x.FilenameExtension));
            return $"Image Files ({extensions})|{extensions}|All Files (*.*)|*.*";
        }
    }
}
