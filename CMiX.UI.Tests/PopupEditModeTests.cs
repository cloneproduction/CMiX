using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Number;
using Xunit;

namespace CMiX.UI.Tests
{
    // A value of a popup that is in edit mode keeps the popup open. A press outside ends the edit and the popup
    // stays open. Then only a press outside closes it, until the pointer is back in the popup.
    public class PopupEditModeTests
    {
        private static (Window Window, Popup Popup, SliderCMiX Slider, ToggleButton Toggle) Show()
        {
            var toggle = new ToggleButton { Content = "Open", Width = 120, HorizontalAlignment = HorizontalAlignment.Left };
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.25, Width = 200, Height = 25 };
            var popup = new Popup { Child = new Border { Child = slider } };
            TogglePopup.SetToggle(popup, toggle);
            var window = new Window { Content = new StackPanel { Margin = new Thickness(0, 100, 0, 0), Children = { toggle, popup } }, Width = 1200, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            window.MouseMove(Middle(window, toggle));
            Dispatcher.UIThread.RunJobs();

            slider.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            return (window, popup, slider, toggle);
        }

        private static Point Middle(Window window, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        private static Point Away(Window window, Control toggle, double away)
        {
            var left = toggle.TranslatePoint(new Point(0, 0), window)!.Value;
            return new Point(left.X + toggle.Bounds.Width + away, left.Y + 5);
        }

        private static void MoveTo(Window window, Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        private static void PressAt(Window window, Point point)
        {
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        // The pointer comes back into the content of the popup: the content gets PointerEntered.
        private static void EnterThePopup(Window window, Popup popup)
        {
            var child = popup.Child!;
            child.RaiseEvent(new PointerEventArgs(InputElement.PointerEnteredEvent, child, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true),
                window, new Point(), 0, new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void WhileAValueIsEdited_ThePointerGoingAway_KeepsThePopupOpen()
        {
            var (window, popup, slider, toggle) = Show();

            MoveTo(window, Away(window, toggle, 400));

            Assert.True(popup.IsOpen);
            Assert.True(slider.IsEditing);
        }

        [AvaloniaFact]
        public void APressOutside_EndsTheEdit_AndTheCommitIsKept()
        {
            var (window, popup, slider, toggle) = Show();
            var box = slider.GetVisualDescendants().OfType<TextBox>().First(candidate => candidate.Name == "PART_TextBox");
            box.Text = "0.75";

            PressAt(window, Away(window, toggle, 400));

            Assert.False(slider.IsEditing);
            Assert.Equal(0.75, slider.Value, 3);
            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void AfterTheEdit_ThePointerGoingAway_DoesNotClose_ButAPressOutsideDoes()
        {
            var (window, popup, _, toggle) = Show();
            PressAt(window, Away(window, toggle, 400));

            MoveTo(window, Away(window, toggle, 500));
            Assert.True(popup.IsOpen);

            PressAt(window, Away(window, toggle, 600));
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WhenThePointerIsBackInThePopup_GoingAwayClosesItAgain()
        {
            var (window, popup, _, toggle) = Show();
            PressAt(window, Away(window, toggle, 400));

            EnterThePopup(window, popup);
            MoveTo(window, Away(window, toggle, 500));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WithoutAnEdit_ThePointerGoingAway_StillCloses()
        {
            var (window, popup, slider, toggle) = Show();
            slider.IsEditing = false;
            Dispatcher.UIThread.RunJobs();

            MoveTo(window, Away(window, toggle, 400));

            Assert.False(popup.IsOpen);
        }
    }
}
