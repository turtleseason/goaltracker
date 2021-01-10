// Adapted from the source code from this article: https://mcguirev10.com/2019/02/10/skin-wpf-apps-using-only-xaml-declarations.html
// and modified to support loading loose XAML skins, to support adding & editing skins at runtime,
// and to avoid false positive errors from SkinnedResourceDictionary at the cost of making the code less elegant
// (Visual Studio doesn't seem to like when you derive from ResourceDictionary and set the child class's properties in XAML:)
// https://stackoverflow.com/questions/54616993/xaml-designer-error-object-does-not-match-target-type-for-subclassed-resourced

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace GoalTracker
{
    public delegate void SkinChangedEventHandler(object sender, object eventArgs);
 
    public abstract class SkinnedApplication : Application
    {
        // If a ResourceDictionary's filename contains this string (case-sensitive), it is treated as a skin file;
        // everything before the last occurrence of this string in the filename is used as the skin name.
        public const string SkinDictionaryTag = "Theme";

        public readonly string UserSkinsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes");

        private readonly Dictionary<string, Uri> skinUris = new Dictionary<string, Uri>();

        public SkinnedApplication() : base()
        { }

        public event SkinChangedEventHandler SkinChangedEvent;

        // This should be updated whenever a key is added to/removed from skinUris
        // (ideally it would be synced to skinUris.Keys through something like a CollectionChanged event on skinUris)
        public ObservableCollection<string> AvailableSkins { get; } = new ObservableCollection<string>();

        public List<string> DefaultSkins { get; } = new List<string>();
        
        public string DefaultSkinName { get; set; } = string.Empty;  // set in App.xaml

        public string ActiveSkin
        {
            get => activeSkin;
            protected set
            {
                activeSkin = value;
                GoalTracker.Properties.Settings.Default.UserSkin = value;
                GoalTracker.Properties.Settings.Default.Save();
                SkinChangedEvent?.Invoke(this, ActiveSkin);
            }
        }
        private string activeSkin = string.Empty;

        // Updates the list of available skins.
        public void FindAvailableSkins()
        {
            LoadDefaultSkins();
            LoadUserSkins();
        }

        public void LoadUserSkinOrDefault()
        {
            string skinName = GoalTracker.Properties.Settings.Default.UserSkin;
            ChangeSkin(skinName);
            if (ActiveSkin != skinName)  // skin failed to load
            {
                ChangeSkin(DefaultSkinName);
            }

            ConsolidateDictionaries();
        }

        public ResourceDictionary GetCurrentSkinDictionary()
        {
            foreach (ResourceDictionary dict in Application.Current.Resources.MergedDictionaries)
            {
                if (IsSkinDictionary(dict.Source.OriginalString, out string skinName) && skinName == ActiveSkin)
                {
                    return dict;
                }
            }
            return null;
        }

        public void ChangeSkin(string skinName)
        {
            if (!skinUris.ContainsKey(skinName))
            {
                Console.WriteLine($"Theme not found: {skinName}");
                return;
            }

            // Currently assumes one XAML file per skin; if this changed, would need to implement some sort of rollback
            // so we don't end up halfway between two skins if a file fails to load
            foreach (ResourceDictionary dict in Application.Current.Resources.MergedDictionaries)
            {
                if (IsSkinDictionary(dict.Source.OriginalString, out string _))
                {
                    Uri originalSource = dict.Source;
                    Uri source = skinUris[skinName];
                    try
                    {
                        dict.Source = source;
                    }
                    // Docs don't specify which exceptions can be thrown (seems like WebException & XamlParseException are possibilities?),
                    // so I've narrowed down the try block as much as possible to minimize the disadvantages of catching Exception
                    catch (Exception)
                    {
                        Console.WriteLine($"Failed to load theme: {skinName}");
                        dict.Source = originalSource;
                        return;
                    }
                }
                else
                {
                    // Reload non-skin dictionaries in case they reference skin resources
                    dict.Source = dict.Source;
                }
            }
            ActiveSkin = skinName;
        }

        public void ReloadNonSkinDictionaries()
        {
            foreach (ResourceDictionary dict in Application.Current.Resources.MergedDictionaries)
            {
                if (!IsSkinDictionary(dict.Source.OriginalString, out string _))
                {
                    dict.Source = dict.Source;
                }
            }
        }

        // Provides a way to notify when the current skin has been edited elsewhere (like in the theme editor).
        public void SkinEdited()
        {
            SkinChangedEvent?.Invoke(this, ActiveSkin);
        }


        private void LoadDefaultSkins()
        {
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                string name = dict.Source.OriginalString;
                if (IsSkinDictionary(name, out string skinName) && !skinUris.ContainsKey(skinName))
                { 
                    skinUris.Add(skinName, dict.Source);
                    AvailableSkins.Add(skinName);
                    DefaultSkins.Add(skinName);
                }
            }
        }

        private void LoadUserSkins()
        {
            if (!Directory.Exists(UserSkinsFolder))
            {
                return;
            }

            foreach (string fileName in Directory.EnumerateFiles(UserSkinsFolder, "*.xaml"))
            {
                if (IsSkinDictionary(fileName, out string skinName)
                    && !skinUris.ContainsKey(skinName)
                    && IsValidResourceDictionary(fileName))
                {
                    skinUris.Add(skinName, new Uri(fileName));
                    AvailableSkins.Add(skinName);
                }
            }
        }

        // Checks whether a file's name follows the convention for skin resource dictionaries, and if so returns the skin name
        // (this doesn't guarantee that the file's contents are valid, though).
        private bool IsSkinDictionary(string fileName, out string skinName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            int delimiterPos = name.LastIndexOf(SkinDictionaryTag);

            if (delimiterPos < 0)
            {
                skinName = null;
                return false;
            }
            else
            {
                skinName = name.Substring(0, delimiterPos);
                return true;
            }
        }

        // Checks that the file can be read and contains a ResourceDictionary.
        // (This doesn't guarantee that the file contains all the required skin resources,
        // but it at least makes sure it can be parsed without throwing an exception.)
        private bool IsValidResourceDictionary(string fileName)
        {
            try
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    var _ = (ResourceDictionary)XamlReader.Load(fs);
                }
            }
            catch (XamlParseException ex) when (ex.InnerException is FileNotFoundException fileEx && File.Exists(fileName))
            {
                // This is caused by trying to load a moved or deleted image; it might be nice to find a better way of handling missing images
                // than skipping the theme file altogether (would probably need to edit the file to remove the missing path?)
                Console.WriteLine($"Can't load ResourceDictionary from theme file {Path.GetFileName(fileName)} because a referenced file is missing: {fileEx.FileName}");
                return false;

            }
            catch (Exception ex)
            {
                // Catching Exception is generally bad, but for the scope of this project, the time spent to implement checks for every possible file/parsing exception
                // probably outweighs the time to debug if an exception gets caught that should have been thrown
                Console.WriteLine($"Can't load ResourceDictionary from theme file {Path.GetFileName(fileName)}; threw {ex.GetType()}:");
                Console.WriteLine($"    {ex.Message}");
                return false;
            }
            return true;
        }
        
        // When multiple default themes are declared in XAML, we end up with redundant ResourceDictionaries in the application's MergedDictionaries;
        // this deletes the extras (to avoid any ambiguity or possible conflicts when editing the current skin dictionary).
        // (This method shouldn't be called before ActiveSkin is set for the first time.)
        private void ConsolidateDictionaries()
        {
            List<ResourceDictionary> skinDictionaries = new List<ResourceDictionary>();
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                string name = dict.Source.OriginalString;
                if (IsSkinDictionary(name, out string _))
                {
                    skinDictionaries.Add(dict);
                }
            }

            if (skinDictionaries.Count > 0)
            {
                skinDictionaries[0].Source = skinUris[ActiveSkin];
            }
            
            for (int i = 1; i < skinDictionaries.Count; i++)
            {
                Application.Current.Resources.MergedDictionaries.Remove(skinDictionaries[i]);
            }
        }
    }
}
