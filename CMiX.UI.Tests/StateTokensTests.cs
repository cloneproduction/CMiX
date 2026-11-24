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
    // The surface controls read their greys from the state tokens: idle, one step up under the pointer, one step under idle while pressed.
    public class StateTokensTests
    {
        private static Color Grey(int step) => Tokens.Color($"Grey{step}Brush");

        [AvaloniaFact]
        public void TheTokens_AreOneStepApart()
        {
            Assert.Equal(Grey(600), Tokens.Color("StateIdleBrush"));
            Assert.Equal(Grey(700), Tokens.Color("StateHoverBrush"));
            Assert.Equal(Grey(500), Tokens.Color("StatePressedBrush"));
            Assert.Equal(Grey(500), Tokens.Color("RowHoverBrush"));
        }

        private static (Control control, Border border) Show(Control control, string part)
        {
            TestWindow.Show(control, 300, 100);
            return (control, Parts.Find<Border>(control, part));
        }

        private static Color Background(Border border) => ((ISolidColorBrush)border.Background!).Color;

        [AvaloniaFact]
        public void AButton_GoesThroughTheThreeSteps()
        {
            var (button, border) = Show(new Button { Content = "Button", Width = 100 }, "border");
            Assert.Equal(Tokens.Color("StateIdleBrush"), Background(border));

            PseudoClassInput.Force(button, ":pointerover");
            Assert.Equal(Tokens.Color("StateHoverBrush"), Background(border));

            PseudoClassInput.Force(button, ":pressed");
            Assert.Equal(Tokens.Color("StatePressedBrush"), Background(border));
        }

        [AvaloniaFact]
        public void AToggleButton_GoesThroughTheThreeSteps_WhileOff()
        {
            var (toggle, border) = Show(new ToggleButton { Content = "Toggle", Width = 100 }, "border");
            Assert.Equal(Tokens.Color("StateIdleBrush"), Background(border));

            PseudoClassInput.Force(toggle, ":pointerover");
            Assert.Equal(Tokens.Color("StateHoverBrush"), Background(border));
        }

        [AvaloniaFact]
        public void ADragValue_StartsAtIdle_AndGoesOneStepUpUnderThePointer()
        {
            var (drag, border) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");
            Assert.Equal(Tokens.Color("StateIdleBrush"), Background(border));

            PseudoClassInput.Force(drag, ":pointerover");
            Assert.Equal(Tokens.Color("StateHoverBrush"), Background(border));

            PseudoClassInput.Force(drag, ":pressed");
            Assert.Equal(Tokens.Color("StatePressedBrush"), Background(border));
        }

        [AvaloniaFact]
        public void APressedState_IsDarkerThanIdle()
        {
            var idle = Tokens.Color("StateIdleBrush");
            var pressed = Tokens.Color("StatePressedBrush");

            Assert.True(pressed.R < idle.R);
        }

        // A pressed control must show on the window and on every panel: its grey is not the grey of a surface.
        [AvaloniaTheory]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(300)]
        [InlineData(400)]
        public void Press_AControl_IsNotTheGreyOfASurface(int surface)
        {
            Assert.NotEqual(Grey(surface), Tokens.Color("StatePressedBrush"));
        }

        // The thumb of a scroll bar is Grey500 at rest, the pressed grey of a control. A thumb that is dragged stays lit.
        [AvaloniaFact]
        public void AScrollThumb_ThatIsPressed_IsNotAtItsIdleGrey()
        {
            var viewer = new ScrollViewer { Width = 100, Height = 60, Content = new Border { Height = 400 } };
            TestWindow.Show(viewer, 300, 100);
            var thumb = viewer.GetVisualDescendants().OfType<Thumb>().First(part => part.Name == "thumb" && part.IsEffectivelyVisible);
            var idle = ((ISolidColorBrush)thumb.Background!).Color;

            PseudoClassInput.Set(thumb, ":pressed");
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Color("StateHoverBrush"), ((ISolidColorBrush)thumb.Background!).Color);
            Assert.NotEqual(idle, ((ISolidColorBrush)thumb.Background!).Color);
        }

        [AvaloniaFact]
        public void ADragValue_IsPressed_WhileTheMouseButtonIsDown()
        {
            using var spy = new CursorSpy();
            var drag = new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left };
            var window = TestWindow.ShowWithoutClips(drag, 300, 100);
            var border = Parts.Find<Border>(drag, "borderValueDisplay");
            var at = border.TranslatePoint(new Point(border.Bounds.Width / 2, border.Bounds.Height / 2), window)!.Value;

            window.MouseMove(at);
            window.MouseDown(at, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(":pressed", drag.Classes);
            Assert.Equal(Tokens.Color("StatePressedBrush"), Background(border));

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
                PseudoClassInput.Set(part, ":pointerover");
            Dispatcher.UIThread.RunJobs();

            Assert.True(Side(drag, name).IsVisible);
        }

        [AvaloniaTheory]
        [InlineData("SubButton")]
        [InlineData("AddButton")]
        public void WhileTheDragValueIsPressed_TheSideButtonsStayVisible_AndAreAsDarkAsTheRest(string name)
        {
            var (drag, border) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");

            PseudoClassInput.Set(drag, ":pressed");
            Dispatcher.UIThread.RunJobs();

            var button = Side(drag, name);
            Assert.True(button.IsVisible);
            var buttonBorder = button.GetVisualDescendants().OfType<Border>().First(part => part.Name == "border");
            Assert.Equal(Background(border), Background(buttonBorder));
            Assert.Equal(Tokens.Color("StatePressedBrush"), Background(buttonBorder));
        }

        [AvaloniaFact]
        public void TheButton_TheDragValue_AndTheSlider_HaveTheSamePressedColor()
        {
            var (button, buttonBorder) = Show(new Button { Content = "Button", Width = 100 }, "border");
            PseudoClassInput.Force(button, ":pressed");

            var (drag, dragBorder) = Show(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 }, "borderValueDisplay");
            PseudoClassInput.Force(drag, ":pressed");

            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.4, Width = 160 };
            TestWindow.Show(slider, 300, 100);
            PseudoClassInput.Set(slider, ":pressed");
            Dispatcher.UIThread.RunJobs();
            var increase = slider.GetVisualDescendants().OfType<RepeatButton>().First(part => part.Name == "PART_IncreaseButton");

            var pressed = Tokens.Color("StatePressedBrush");
            Assert.Equal(pressed, Background(buttonBorder));
            Assert.Equal(pressed, Background(dragBorder));
            Assert.Equal(pressed, ((ISolidColorBrush)increase.Background!).Color);
        }
    }
}
