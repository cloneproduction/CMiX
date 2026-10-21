using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
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
            Assert.Equal(Grey(800), Token("StatePressedBrush"));
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
        }
    }
}
