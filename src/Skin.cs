using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GoalTracker
{
    class Skin
    {
        public Skin(ResourceDictionary source)
        {
            try
            {
                ImportFromResourceDictionary(source);
            }
            catch (MissingResourceException ex)
            {
                throw new ArgumentException($"The required resource {ex.ResourceName} is missing.", nameof(source), ex);
            }
        }

        public SkinProperty CalendarWeekOpacity { get; private set; }

        public SkinProperty BackgroundImagePath { get; private set; }

        public SkinProperty BackgroundColor { get; private set; }
        public SkinProperty ForegroundColor { get; private set; }
        public SkinProperty MenuBorderColor { get; private set; }

        public SkinProperty CalendarBorderColor { get; private set; }

        public SkinProperty CalendarDefaultBgColor { get; private set; }
        public SkinProperty Calendar0BgColor { get; private set; }
        public SkinProperty Calendar50BgColor { get; private set; }
        public SkinProperty Calendar100BgColor { get; private set; }

        public SkinProperty CalendarDefaultFgColor { get; private set; }
        public OptionalSkinProperty Calendar0FgColor { get; private set; }
        public OptionalSkinProperty Calendar50FgColor { get; private set; }
        public OptionalSkinProperty Calendar100FgColor { get; private set; }

        public SkinProperty CalendarWeekBgColor { get; private set; }
        public OptionalSkinProperty CalendarWeek0BgColor { get; private set; } 
        public OptionalSkinProperty CalendarWeek50BgColor { get; private set; }
        public OptionalSkinProperty CalendarWeek100BgColor { get; private set; }

        public OptionalSkinProperty CalendarWeekFgColor { get; private set; }
        public OptionalSkinProperty CalendarWeek0FgColor { get; private set; }
        public OptionalSkinProperty CalendarWeek50FgColor { get; private set; }
        public OptionalSkinProperty CalendarWeek100FgColor { get; private set; }

        public SkinProperty CalendarHoverBgColor { get; private set; }
        public SkinProperty CalendarHoverFgColor { get; private set; }
        public SkinProperty CalendarHoverBorderColor { get; private set; }

        public ResourceDictionary ToResourceDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            ExportProperties(dict);
            return dict;
        }

        private void ImportFromResourceDictionary(ResourceDictionary source)
        {
            CalendarWeekOpacity = ImportProperty(nameof(CalendarWeekOpacity), source);

            // The background image property is unique in that it's optional with no direct fallback value;
            // if it doesn't exist, it should just be null.
            string backgroundPathValue = source.Contains(nameof(BackgroundImagePath)) ? (string)source[nameof(BackgroundImagePath)] : null;
            BackgroundImagePath = new SkinProperty(nameof(BackgroundImagePath), backgroundPathValue);

            BackgroundColor = ImportProperty(nameof(BackgroundColor), source);
            ForegroundColor = ImportProperty(nameof(ForegroundColor), source);
            MenuBorderColor = ImportProperty(nameof(MenuBorderColor), source);

            CalendarBorderColor = ImportProperty(nameof(CalendarBorderColor), source);

            CalendarDefaultBgColor = ImportProperty(nameof(CalendarDefaultBgColor), source);
            Calendar0BgColor = ImportProperty(nameof(Calendar0BgColor), source);
            Calendar50BgColor = ImportProperty(nameof(Calendar50BgColor), source);
            Calendar100BgColor = ImportProperty(nameof(Calendar100BgColor), source);

            CalendarDefaultFgColor = ImportProperty(nameof(CalendarDefaultFgColor), source);
            Calendar0FgColor = ImportOptionalProperty(nameof(Calendar0FgColor), source, CalendarDefaultFgColor);
            Calendar50FgColor = ImportOptionalProperty(nameof(Calendar50FgColor), source, CalendarDefaultFgColor);
            Calendar100FgColor = ImportOptionalProperty(nameof(Calendar100FgColor), source, CalendarDefaultFgColor);

            CalendarWeekBgColor = ImportProperty(nameof(CalendarWeekBgColor), source);
            CalendarWeek0BgColor = ImportOptionalProperty(nameof(CalendarWeek0BgColor), source, Calendar0BgColor);
            CalendarWeek50BgColor = ImportOptionalProperty(nameof(CalendarWeek50BgColor), source, Calendar50BgColor);
            CalendarWeek100BgColor = ImportOptionalProperty(nameof(CalendarWeek100BgColor), source, Calendar100BgColor);

            CalendarWeekFgColor = ImportOptionalProperty(nameof(CalendarWeekFgColor), source, CalendarDefaultFgColor);
            CalendarWeek0FgColor = ImportOptionalProperty(nameof(CalendarWeek0FgColor), source, Calendar0FgColor);
            CalendarWeek50FgColor = ImportOptionalProperty(nameof(CalendarWeek50FgColor), source, Calendar50FgColor);
            CalendarWeek100FgColor = ImportOptionalProperty(nameof(CalendarWeek100FgColor), source, Calendar100FgColor);

            CalendarHoverBgColor = ImportProperty(nameof(CalendarHoverBgColor), source);
            CalendarHoverFgColor = ImportProperty(nameof(CalendarHoverFgColor), source);
            CalendarHoverBorderColor = ImportProperty(nameof(CalendarHoverBorderColor), source);
        }
        
        private void ExportProperties(ResourceDictionary dict)
        {
            dict[nameof(CalendarWeekOpacity)] = CalendarWeekOpacity.Value;

            if (BackgroundImagePath.Value != null)
            {
                BitmapImage backgroundImage = new BitmapImage() { UriSource = new Uri((string)BackgroundImagePath.Value) };
                dict["BackgroundImage"] = backgroundImage;

                // The path is stored separately alongside the actual BitmapImage so that we can read it back later from a loaded ResourceDictionary
                // (once loaded, the image can no longer be cast to a BitmapImage to check its UriSource)
                dict[nameof(BackgroundImagePath)] = BackgroundImagePath.Value;
            }

            dict[nameof(BackgroundColor)] = BackgroundColor.Value;
            dict[nameof(ForegroundColor)] = ForegroundColor.Value;
            dict[nameof(MenuBorderColor)] = MenuBorderColor.Value;

            dict[nameof(CalendarBorderColor)] = CalendarBorderColor.Value;

            dict[nameof(CalendarDefaultBgColor)] = CalendarDefaultBgColor.Value;
            dict[nameof(Calendar0BgColor)] = Calendar0BgColor.Value;
            dict[nameof(Calendar50BgColor)] = Calendar50BgColor.Value;
            dict[nameof(Calendar100BgColor)] = Calendar100BgColor.Value;

            dict[nameof(CalendarDefaultFgColor)] = CalendarDefaultFgColor.Value;
            dict[nameof(Calendar0FgColor)] = Calendar0FgColor.Value;
            dict[nameof(Calendar50FgColor)] = Calendar50FgColor.Value;
            dict[nameof(Calendar100FgColor)] = Calendar100FgColor.Value;

            dict[nameof(CalendarWeekBgColor)] = CalendarWeekBgColor.Value;
            dict[nameof(CalendarWeek0BgColor)] = CalendarWeek0BgColor.Value;
            dict[nameof(CalendarWeek50BgColor)] = CalendarWeek50BgColor.Value;
            dict[nameof(CalendarWeek100BgColor)] = CalendarWeek100BgColor.Value;

            dict[nameof(CalendarWeekFgColor)] = CalendarWeekFgColor.Value;
            dict[nameof(CalendarWeek0FgColor)] = CalendarWeek0FgColor.Value;
            dict[nameof(CalendarWeek50FgColor)] = CalendarWeek50FgColor.Value;
            dict[nameof(CalendarWeek100FgColor)] = CalendarWeek100FgColor.Value;

            dict[nameof(CalendarHoverBgColor)] = CalendarHoverBgColor.Value;
            dict[nameof(CalendarHoverFgColor)] = CalendarHoverFgColor.Value;
            dict[nameof(CalendarHoverBorderColor)] = CalendarHoverBorderColor.Value;
        }

        private SkinProperty ImportProperty(string key, ResourceDictionary source)
        {
            try
            {
                return new SkinProperty(key, source[key]);
            }
            catch (NullReferenceException)
            {
                throw new MissingResourceException(key);
            }
        }

        private OptionalSkinProperty ImportOptionalProperty(string key, ResourceDictionary source, SkinProperty fallback)
        {
            if (source.Contains(key))
            {
                object value = source[key];
                return new OptionalSkinProperty(key, value, fallback);
            }
            else
            {
                return new OptionalSkinProperty(key, fallback);
            }
        }


        private sealed class MissingResourceException : Exception
        {
            public MissingResourceException(string resourceName)
            {
                ResourceName = resourceName;
            }

            public string ResourceName { get; private set; }
        }
    }
}
