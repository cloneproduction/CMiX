using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
using Xunit;

namespace CMiX.UI.Tests
{
    // Hovering the leading or the trailing content of a value editor must not look like hovering the editor.
    // The pointer over the content makes the whole control hovered, so the hover styles of the editors
    // must look at their own body, not at the whole control.
    public class HoverTargetTests
    {
        private static Border Content(Color color) => new() { Width = 40, Height = 25, Background = new SolidColorBrush(color) };

        private static Window Host(Control control)
        {
            control.Width = 400;
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;
            var window = new Window { Content = control, Width = 600, Height = 200, FontSize = 12.5 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.MouseMove(new Point(500, 150));
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static void HoverOn(Window window, Control target)
        {
            window.MouseMove(target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value);
            Dispatcher.UIThread.RunJobs();
        }

        private static T Part<T>(Control control, string name) where T : Control =>
            control.GetVisualDescendants().OfType<T>().First(part => part.Name == name);

        private static Color ColorOf(RepeatButton button) => ((ISolidColorBrush)button.Background!).Color;

        private static Color Resource(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        // The test pointer does not reach the body of the slider, so the hover state is set by hand,
        // on the body and on the control, as a real pointer over the body would do.
        private static void SetHover(Control control, bool on) =>
            ((IPseudoClasses)control.Classes).Set(":pointerover", on);

        [AvaloniaFact]
        public void Slider_HoveringTheTrailingContent_DoesNotLightTheTrack()
        {
            var trailing = Content(Colors.Red);
            var slider = new SliderCMiX { Minimum = 0, Maximum = 10, Value = 5, TrailingContent = trailing };
            var window = Host(slider);
            var resting = (ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton")), ColorOf(Part<RepeatButton>(slider, "PART_IncreaseButton")));

            HoverOn(window, trailing);

            Assert.True(slider.IsPointerOver);
            Assert.Equal(resting.Item1, ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton")));
            Assert.Equal(resting.Item2, ColorOf(Part<RepeatButton>(slider, "PART_IncreaseButton")));
        }

        [AvaloniaFact]
        public void Slider_HoveringTheBody_LightsTheTrack()
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 10, Value = 5, TrailingContent = Content(Colors.Red) };
            Host(slider);

            SetHover(slider, true);
            SetHover(Part<Border>(slider, "mainBorder"), true);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Resource("AccentBrush"), ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton")));
            Assert.Equal(Resource("Grey600Brush"), ColorOf(Part<RepeatButton>(slider, "PART_IncreaseButton")));
        }

        [AvaloniaFact]
        public void Slider_WithOnlyTheControlHovered_DoesNotLightTheTrack()
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 10, Value = 5, TrailingContent = Content(Colors.Red) };
            Host(slider);
            var resting = ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton"));

            SetHover(slider, true);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(resting, ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton")));
        }

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DragValue_HoveringTheLeadingOrTrailingContent_ShowsNoHoverOfTheValue(bool leading)
        {
            var side = Content(leading ? Colors.Blue : Colors.Red);
            var drag = new DragValue { Minimum = 0, Maximum = 10, Value = 5 };
            if (leading) drag.LeadingContent = side; else drag.TrailingContent = side;
            var window = Host(drag);
            var valueArea = Part<Border>(drag, "borderValueDisplay");
            var resting = (valueArea.Background as ISolidColorBrush)?.Color;

            HoverOn(window, side);

            Assert.True(drag.IsPointerOver);
            Assert.False(Part<Button>(drag, "SubButton").IsVisible);
            Assert.False(Part<Button>(drag, "AddButton").IsVisible);
            Assert.False(valueArea.IsPointerOver);
            Assert.Equal(resting, (valueArea.Background as ISolidColorBrush)?.Color);
        }
    }
}
