using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The surface controls read their greys from the state tokens: idle, one step up under the pointer, two steps up while pressed.
    public class StateTokensTests
    {
        private static Color Token(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        private static Color Grey(int step) => Token($"Grey{step}Brush");

        [AvaloniaFact]
        public void TheTokens_AreOneStepApart()
        {
            Assert.Equal(Grey(600), Token("StateIdleBrush"));
            Assert.Equal(Grey(700), Token("StateHoverBrush"));
            Assert.Equal(Grey(300), Token("StatePressedBrush"));
            Assert.Equal(Grey(500), Token("RowHoverBrush"));
        }

        private static (Control control, Border border) Show(Control control, string part)
        {
            new Window { Content = control, Width = 300, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (control, control.GetVisualDescendants().OfType<Border>().First(border => border.Name == part));
        }

        private static Color Background(Border border) => ((ISolidColorBrush)border.Background!).Color;

        private static void Force(Control control, string pseudoClass)
        {
            foreach (var part in control.GetSelfAndVisualDescendants().OfType<Control>())
                ((IPseudoClasses)part.Classes).Set(pseudoClass, true);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void AButton_GoesThroughTheThreeSteps()
        {
            var (button, border) = Show(new Button { Content = "Button", Width = 100 }, "border");
            Assert.Equal(Token("StateIdleBrush"), Background(border));

            Force(button, ":pointerover");
            Assert.Equal(Token("StateHoverBrush"), Background(border));

            Force(button, ":pressed");
            Assert.Equal(Token("StatePressedBrush"), Background(border));
        }

        [AvaloniaFact]
        public void AToggleButton_GoesThroughTheThreeSteps_WhileOff()
        {
            var (toggle, border) = Show(new ToggleButton { Content = "Toggle", Width = 100 }, "border");
            Assert.Equal(Token("StateIdleBrush"), Background(border));

            Force(toggle, ":pointerover");
            Assert.Equal(Token("StateHoverBrush"), Background(border));
        }

        [AvaloniaFact]
        public void ADragValue_StartsAtIdle_AndGoesOneStepUpUnderThePointer()
        {
            var (drag, border) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");
            Assert.Equal(Token("StateIdleBrush"), Background(border));

            Force(drag, ":pointerover");
            Assert.Equal(Token("StateHoverBrush"), Background(border));

            Force(drag, ":pressed");
            Assert.Equal(Token("StatePressedBrush"), Background(border));
        }

        [AvaloniaFact]
        public void APressedState_IsDarkerThanIdle()
        {
            var idle = Token("StateIdleBrush");
            var pressed = Token("StatePressedBrush");

            Assert.True(pressed.R < idle.R);
        }

        [AvaloniaFact]
        public void ADragValue_IsPressed_WhileTheMouseButtonIsDown()
        {
            using var spy = new CursorSpy();
            var drag = new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left };
            var window = new Window { Content = drag, Width = 300, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            var border = drag.GetVisualDescendants().OfType<Border>().First(part => part.Name == "borderValueDisplay");
            var at = border.TranslatePoint(new Point(border.Bounds.Width / 2, border.Bounds.Height / 2), window)!.Value;

            window.MouseMove(at);
            window.MouseDown(at, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(":pressed", drag.Classes);
            Assert.Equal(Token("StatePressedBrush"), Background(border));

            window.MouseUp(at, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.DoesNotContain(":pressed", drag.Classes);
        }

        private static Button Side(Control drag, string name) =>
            drag.GetVisualDescendants().OfType<Button>().First(button => button.Name == name);

        [AvaloniaTheory]
        [InlineData("SubButton")]
        [InlineData("AddButton")]
        public void TheSideButtons_AreHidden_WhenTheDragValueIsNeitherHoveredNorPressed(string name)
        {
            var (drag, _) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");

            Assert.False(Side(drag, name).IsVisible);
        }

        [AvaloniaTheory]
        [InlineData("SubButton")]
        [InlineData("AddButton")]
        public void TheSideButtons_Show_UnderThePointer(string name)
        {
            var (drag, _) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");

            foreach (var part in drag.GetVisualDescendants().OfType<Border>().Where(border => border.Name == "mainBorder"))
                ((IPseudoClasses)part.Classes).Set(":pointerover", true);
            Dispatcher.UIThread.RunJobs();

            Assert.True(Side(drag, name).IsVisible);
        }

        [AvaloniaTheory]
        [InlineData("SubButton")]
        [InlineData("AddButton")]
        public void WhileTheDragValueIsPressed_TheSideButtonsStayVisible_AndAreAsDarkAsTheRest(string name)
        {
            var (drag, border) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");

            ((IPseudoClasses)drag.Classes).Set(":pressed", true);
            Dispatcher.UIThread.RunJobs();

            var button = Side(drag, name);
            Assert.True(button.IsVisible);
            var buttonBorder = button.GetVisualDescendants().OfType<Border>().First(part => part.Name == "border");
            Assert.Equal(Background(border), Background(buttonBorder));
            Assert.Equal(Token("StatePressedBrush"), Background(buttonBorder));
        }
    }
}
