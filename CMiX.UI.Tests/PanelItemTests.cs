using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // PanelItem is an item of a list. It reads only its own properties, and its default theme
    // is the collapsible look with an eye, a close button and a drag handle.
    public class PanelItemTests
    {
        private static (Window window, PanelItem item) Show(PanelItem? item = null)
        {
            item ??= new PanelItem { Title = "Grid", Content = new TextBlock { Text = "body" } };
            var window = new Window { Content = new StackPanel { Children = { item } }, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, item);
        }

        private static Expander ExpanderOf(PanelItem item) => item.GetVisualDescendants().OfType<Expander>().Single();

        private static Button ButtonWithIcon(PanelItem item, string iconKey) =>
            item.GetVisualDescendants().OfType<AppIcon>().Single(i => i.IconKey == iconKey).GetVisualAncestors().OfType<Button>().First();

        private static ToggleButton EyeOf(PanelItem item) =>
            item.GetVisualDescendants().OfType<AppIcon>().Single(i => i.IconKey is "Visible" or "NotVisible")
                .GetVisualAncestors().OfType<ToggleButton>().First();

        private static double LeftOf(Control control, Control relativeTo) => control.TranslatePoint(new Point(0, 0), relativeTo)!.Value.X;

        [AvaloniaFact]
        public void ShowsItsTitle()
        {
            var (_, item) = Show();

            Assert.Contains(item.GetVisualDescendants().OfType<TextBlock>(), t => t.Text == "Grid");
        }

        [AvaloniaFact]
        public void TheBody_IsShownOnlyWhileExpanded()
        {
            var (_, item) = Show();
            bool BodyIsVisible() => item.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "body" && t.IsEffectivelyVisible);

            Assert.False(BodyIsVisible());

            item.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(BodyIsVisible());
        }

        [AvaloniaFact]
        public void TheExpander_AndThePanel_FollowEachOther()
        {
            var (_, item) = Show();

            ExpanderOf(item).IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(item.IsExpanded);

            item.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(ExpanderOf(item).IsExpanded);
        }

        [AvaloniaFact]
        public void TheEye_FollowsIsChecked_AndWritesIt()
        {
            var (window, item) = Show();
            var eye = EyeOf(item);

            item.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(eye.IsChecked);

            PointerInput.Click(window, eye, MouseButton.Left);
            Assert.False(item.IsChecked);
        }

        [AvaloniaFact]
        public void TheCloseButton_RunsTheCommandWithItsParameter()
        {
            object? received = null;
            var item = new PanelItem
            {
                Title = "Grid",
                CloseCommand = new RelayCommand<object?>(p => received = p),
                CloseCommandParameter = "the item"
            };
            var (window, _) = Show(item);

            PointerInput.Click(window, ButtonWithIcon(item, "Close"), MouseButton.Left);

            Assert.Equal("the item", received);
        }

        [AvaloniaFact]
        public void TheDragHandle_ShareItsPressedStateWithThePanel()
        {
            var (_, item) = Show();
            var handle = ButtonWithIcon(item, "DragHandle");

            item.DragHandleIsPressed = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(DragHandle.GetIsPressed(handle));

            DragHandle.SetIsPressed(handle, false);
            Dispatcher.UIThread.RunJobs();
            Assert.False(item.DragHandleIsPressed);
        }

        [AvaloniaFact]
        public void TheHeaderButtons_AreInTheOldOrder_EyeThenCloseThenHandle()
        {
            var (_, item) = Show();

            var eye = LeftOf(EyeOf(item), item);
            var close = LeftOf(ButtonWithIcon(item, "Close"), item);
            var handle = LeftOf(ButtonWithIcon(item, "DragHandle"), item);

            Assert.True(eye < close && close < handle, $"eye {eye}, close {close}, handle {handle}");
        }

        [AvaloniaFact]
        public void WithoutAPanelBackground_TheThemeBackgroundShows()
        {
            var (_, item) = Show();
            var theme = ((ISolidColorBrush)Application.Current!.FindResource("Grey300Brush")!).Color;

            Assert.Equal(theme, ((ISolidColorBrush)ExpanderOf(item).Background!).Color);
        }

        [AvaloniaFact]
        public void APanelBackground_ReplacesTheThemeBackground()
        {
            var (_, item) = Show();

            item.PanelBackground = Brushes.Crimson;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Colors.Crimson, ((ISolidColorBrush)ExpanderOf(item).Background!).Color);
        }

        [AvaloniaFact]
        public void IsHovered_FollowsThePointer()
        {
            var (window, item) = Show();
            var inside = item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), window)!.Value;

            window.MouseMove(inside);
            Dispatcher.UIThread.RunJobs();
            Assert.True(item.IsHovered);

            window.MouseMove(new Point(390, 290));
            Dispatcher.UIThread.RunJobs();
            Assert.False(item.IsHovered);
        }
    }
}
