using System.Windows;

namespace CMiX.Core.Themes.AttachedProperties

{
    public static class ExpanderIndentation
    {
        public static readonly DependencyProperty IndentationProperty =
            DependencyProperty.RegisterAttached(
                "Indentation",
                typeof(Thickness),
                typeof(ExpanderIndentation),
                new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));

        public static Thickness GetIndentation(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(IndentationProperty);
        }

        public static void SetIndentation(DependencyObject obj, Thickness value)
        {
            obj.SetValue(IndentationProperty, value);
        }
    }
}
