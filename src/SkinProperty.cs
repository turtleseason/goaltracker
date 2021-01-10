using GoalTracker.Util;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace GoalTracker
{
    // Todo: add a Type property so that the intended type is known even when the ResourceKey is null
    // (although currently only ResourceKeys of type string can be null in practice)
    class SkinProperty : ObservableObject
    {
        private object value;

        public SkinProperty(string resourceKey, object value)
        {
            ResourceKey = resourceKey;
            Value = value;
        }

        public string ResourceKey { get; }

        public string DisplayName
        {
            get => Regex.Replace(
                ResourceKey.Replace("Bg", "BG")
                           .Replace("Fg", "FG")
                           .Replace("Color", "")
                           .Replace("Path", ""),
                @"\B((?:[A-Z]+[a-z]*)|(?:[0-9]+))", " $1");  // add spaces between words
        }

        public virtual object Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }

    class OptionalSkinProperty : SkinProperty
    {
        private object localValue;
        private bool isUsingFallback;

        public OptionalSkinProperty(string resourceKey, SkinProperty fallback) : base(resourceKey, null)
        {
            Fallback = fallback;
            IsUsingFallback = true;
        }

        // Automatically sets IsUsingFallback to true if the given value is equal to the fallback's current value
        public OptionalSkinProperty(string resourceKey, object value, SkinProperty fallback) : base(resourceKey, value)
        {
            Fallback = fallback;
            Value = value;
            IsUsingFallback = value.Equals(Fallback.Value);

            PropertyChangedEventManager.AddHandler(Fallback, FallbackValueChanged, nameof(SkinProperty.Value));
        }

        public override object Value
        {
            get => IsUsingFallback ? Fallback.Value : localValue;
            set
            {
                if (localValue != value)
                {
                    localValue = value;
                    if (!IsUsingFallback)
                    {
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        public SkinProperty Fallback { get; }

        public bool IsUsingFallback
        {
            get => isUsingFallback;
            set
            {
                if (isUsingFallback != value)
                {
                    isUsingFallback = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(Value));
                }
            }
        }

        private void FallbackValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsUsingFallback)
            {
                NotifyPropertyChanged(nameof(Value));
            }
        }
    }
}
