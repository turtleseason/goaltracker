using GoalTracker.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace GoalTracker.ViewModels
{
    class SkinEditorWindowViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private readonly App app = App.Current as App;
        
        private readonly WindowService windowService;

        private readonly Skin workingSkin;
        private readonly ResourceDictionary currentDictionary;

        public SkinEditorWindowViewModel(WindowService windowService)
        {
            this.windowService = windowService;

            currentDictionary = app.GetCurrentSkinDictionary();
            workingSkin = new Skin(currentDictionary);
            
            PopulatePropertyLists();
            
            workingSkin.BackgroundImagePath.PropertyChanged += ImagePathPropertyChanged;
            AddPropertyEventHandlers();

            SaveCommand = new DelegateCommand(SaveToFile, () => !HasErrors);
            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke());

            SkinName = app.ActiveSkin;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event Action RequestClose;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public string SkinName { get; set; } = string.Empty;

        public List<SkinProperty> GeneralColors { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> BackgroundImage { get; set; } = new List<SkinProperty>();

        public List<SkinProperty> CalendarDefault { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> Calendar0 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> Calendar50 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> Calendar100 { get; set; } = new List<SkinProperty>();
      
        public List<SkinProperty> CalendarWeek { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> CalendarWeek0 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> CalendarWeek50 { get; set; } = new List<SkinProperty>();
        public List<SkinProperty> CalendarWeek100 { get; set; } = new List<SkinProperty>();
        
        public List<SkinProperty> CalendarHover { get; set; } = new List<SkinProperty>();

        public bool HasErrors
        {
            get => hasImagePathError;
            set
            {
                if (hasImagePathError != value)
                {
                    hasImagePathError = value;
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(BackgroundImage)));
                }
            }
        }
        private bool hasImagePathError = false;


        // Todo: handle possible I/O exceptions (probably extract file handling to another class?)
        public void SaveToFile()
        {
            if (!IsValidSkinName(SkinName, out string errorMessage))
            {
                windowService.ShowMessage(errorMessage, "Invalid theme name");
                return;
            }
            
            string fileName = SkinName + SkinnedApplication.SkinDictionaryTag + ".xaml";
            string filePath = Path.Combine(app.UserSkinsFolder, fileName);
            
            if (!Directory.Exists(app.UserSkinsFolder))
            {
                Directory.CreateDirectory(app.UserSkinsFolder);
            }
            
            if (File.Exists(filePath))
            {
                MessageBoxResult result = windowService.ShowMessage($"Do you want to overwrite the existing theme \"{SkinName}\"?",
                    "Overwrite theme?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            using (FileStream file = File.Create(filePath))
            {
                XamlWriter.Save(workingSkin.ToResourceDictionary(), file);
            }

            app.FindAvailableSkins();  // Reload skins so that the new skin will show up as available
            app.ChangeSkin(SkinName);

            RequestClose?.Invoke();
        }

        public void OnClosing()
        {
            app.ChangeSkin(app.ActiveSkin);  // Clear any unsaved changes
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == nameof(BackgroundImage) && HasErrors)
            {
                return new List<string>() { "Invalid image path" };
            }
            return null;
        }


        private void PopulatePropertyLists()
        {
            GeneralColors.Add(workingSkin.BackgroundColor);
            GeneralColors.Add(workingSkin.ForegroundColor);
            GeneralColors.Add(workingSkin.MenuBorderColor);

            BackgroundImage.Add(workingSkin.BackgroundImagePath);

            CalendarDefault.Add(workingSkin.CalendarDefaultBgColor);
            CalendarDefault.Add(workingSkin.CalendarDefaultFgColor);
            CalendarDefault.Add(workingSkin.CalendarBorderColor);

            Calendar0.Add(workingSkin.Calendar0BgColor);
            Calendar0.Add(workingSkin.Calendar0FgColor);

            Calendar50.Add(workingSkin.Calendar50BgColor);
            Calendar50.Add(workingSkin.Calendar50FgColor);

            Calendar100.Add(workingSkin.Calendar100BgColor);
            Calendar100.Add(workingSkin.Calendar100FgColor);

            CalendarWeek.Add(workingSkin.CalendarWeekOpacity);
            CalendarWeek.Add(workingSkin.CalendarWeekBgColor);
            CalendarWeek.Add(workingSkin.CalendarWeekFgColor);

            CalendarWeek0.Add(workingSkin.CalendarWeek0BgColor);
            CalendarWeek0.Add(workingSkin.CalendarWeek0FgColor);

            CalendarWeek50.Add(workingSkin.CalendarWeek50BgColor);
            CalendarWeek50.Add(workingSkin.CalendarWeek50FgColor);

            CalendarWeek100.Add(workingSkin.CalendarWeek100BgColor);
            CalendarWeek100.Add(workingSkin.CalendarWeek100FgColor);

            CalendarHover.Add(workingSkin.CalendarHoverBgColor);
            CalendarHover.Add(workingSkin.CalendarHoverFgColor);
            CalendarHover.Add(workingSkin.CalendarHoverBorderColor);
        }

        // (This should be called after PopulatePropertyLists().)
        private void AddPropertyEventHandlers()
        {
            // excludes BackgroundImagePath (which has its own update handler)
            IEnumerable<SkinProperty> allProperties = GeneralColors.Concat(CalendarDefault)
                                                                   .Concat(Calendar0)
                                                                   .Concat(Calendar50)
                                                                   .Concat(Calendar100)
                                                                   .Concat(CalendarWeek)
                                                                   .Concat(CalendarWeek0)
                                                                   .Concat(CalendarWeek50)
                                                                   .Concat(CalendarWeek100)
                                                                   .Concat(CalendarHover);

            foreach (SkinProperty prop in allProperties)
            {
                prop.PropertyChanged += SkinPropertyChanged;
            }
        }

        private void SkinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SkinProperty.Value))
            {
                SkinProperty prop = (SkinProperty)sender;
                //bool fallback = (sender as OptionalSkinProperty)?.IsUsingFallback ?? false;
                UpdateProperty(prop.ResourceKey, prop.Value); // !fallback);
            }
        }

        // Updates a property's key in the ResourceDictionary so that changes to the skin can be previewed in the application.
        private void UpdateProperty(string key, object newValue, bool refresh=true)
        {
            currentDictionary[key] = newValue;
            //Console.WriteLine($"Updated {key}");


            // Todo: optimize by eliminating unnecessary reloads when multiple properties update at once
            // (when optional properties update in response to their fallback value updating)
            // 
            // UpdateProperty appears to always be called for optional properties before the fallback properties they depend on
            // (maybe because of order of event handler assignment?) - is this guaranteed/reliable enough behavior to depend on?
            // If so, simply skipping the call to ReloadSkinDictionaries() for any optional property using a fallback will work
            // 
            // (A single call to ReloadNonSkinDictionaries() actually appears to work even if other UpdateProperty calls follow immediately after;
            // does reloading ResourceDictionary.Source happen asynchronously, so all UpdateProperty calls finish first?
            if (refresh)
            {
                //Console.WriteLine("Start reload dicts");
                app.ReloadNonSkinDictionaries();
                //Console.WriteLine("Reloaded dicts");
            }
            app.SkinEdited();
        }

        private void ImagePathPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SkinProperty prop = (SkinProperty)sender;
            HasErrors = !IsValidImage((string)prop.Value);

            if (!HasErrors)
            {
                UpdateProperty(prop.ResourceKey, prop.Value);

                if (prop.Value != null)
                {
                    var uri = new Uri((string)prop.Value);
                    UpdateProperty("BackgroundImage", new BitmapImage(uri));
                }
                else
                {
                    UpdateProperty("BackgroundImage", null);
                }
            }
        }

        private bool IsValidImage(string path)
        {
            if (path != null)
            {
                Uri sourceUri = new Uri(path);

                try
                {
                    new BitmapImage(sourceUri);
                }
                catch (Exception ex) when (ex is FileNotFoundException || ex is NotSupportedException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected exception when validating image: \n  {ex.GetType()}: {ex.Message}");
                    return false;
                }
            }
            return true;
        }

        private bool IsValidSkinName(string name, out string errorMessage)
        {
            if (name == string.Empty)
            {
                errorMessage = "Please enter a theme name.";
                return false;
            }

            if (app.DefaultSkins.Contains(name))
            {
                errorMessage = $"Can't overwrite default theme \"{name}\"; please choose a different name.";
                return false;
            }

            List<char> disallowedChars = new List<char>(Path.GetInvalidFileNameChars());
            foreach (char c in name)
            {
                if (disallowedChars.Contains(c))
                {
                    errorMessage = $"Theme name cannot contain the character {c}\nPlease choose a different name.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}
