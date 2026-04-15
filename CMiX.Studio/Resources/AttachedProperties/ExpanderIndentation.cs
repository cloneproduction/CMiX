using System.Windows;

namespace CMiX.Studio.AttachedProperties
{
    public static class ExpanderIndentation
    {
        public static readonly DependencyProperty IndentationProperty =
            DependencyProperty.RegisterAttached(
                "Indentation",
                typeof(int),
                typeof(ExpanderIndentation),
                new FrameworkPropertyMetadata(0, OnIndentationChanged));

        public static int GetIndentation(DependencyObject obj)
            => (int)obj.GetValue(IndentationProperty);

        public static void SetIndentation(DependencyObject obj, int value)
            => obj.SetValue(IndentationProperty, value);

        private static void OnIndentationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
                element.Margin = new Thickness((int)e.NewValue, 0, 0, 0);
        }
    }
}
