using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GoalTracker.Converters
{
    class SkinPropertyDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is SkinProperty)
            {
                Type type = (item as SkinProperty).Value?.GetType();
                
                if (type == typeof(Color))
                {
                    if (item is OptionalSkinProperty)
                    {
                        return element.FindResource("optionalColorPropertyTemplate") as DataTemplate;
                    }
                    else
                    {
                        return element.FindResource("colorPropertyTemplate") as DataTemplate;
                    }
                }

                if (type == typeof(double))
                {
                    return element.FindResource("doublePropertyTemplate") as DataTemplate;
                }

                // BackgroundImagePath is the only value that can be null - would be better to add a Type property to TheemProperty
                // to remove ambiguity, but in the interest of time this will work
                if (type == null || type == typeof(string))
                {
                    return element.FindResource("pathPropertyTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
