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
using CMiX.UI.Mathematics;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The surface controls read their greys from the state tokens: idle, one step up under the pointer, one step
    // under idle while pressed. A control that is on and a selected row lie on the accent, with ActiveForegroundBrush text.
    public class StateTokensTests
    {
        // The surface controls read their greys from the state tokens: idle, one step up under the pointer, one step under idle while pressed.

        private static Color Grey(int step) => Tokens.Color($"Grey{step}Brush");

        [AvaloniaFact]
        public void Tokens_TheStateGreys_AreOneStepApart()
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
        public void Hover_AButton_GoesThroughTheThreeSteps()
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
        public void Press_AnyControl_IsDarkerThanIdle()
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
        public void Press_AButtonADragValueOrASlider_GivesTheSameColor()
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

        // A control that is on and a row that is selected lie on the accent: AccentInactiveBrush at rest, StateOnHoverBrush under the pointer.
        // Their text is ActiveForegroundBrush, so it can be read on the two fills.

        private static Color TextColorOf(Control control) =>
            ((ISolidColorBrush)control.GetVisualDescendants().OfType<TextBlock>().First().Foreground!).Color;

        private static Color FillOf(Control control, string part) =>
            ((ISolidColorBrush)control.GetVisualDescendants().OfType<Border>().First(border => border.Name == part).Background!).Color;

        [AvaloniaTheory]
        [InlineData("AccentInactiveBrush")]
        [InlineData("StateOnHoverBrush")]
        public void TheActiveText_OnTheFillOfAnOnState_CanBeRead(string fill)
        {
            Assert.True(ColorMix.Contrast(Tokens.Color("ActiveForegroundBrush"), Tokens.Color(fill)) >= 4.5);
        }

        [AvaloniaFact]
        public void Check_AToggleButton_MakesItsTextTheActiveText()
        {
            var toggle = new ToggleButton { Content = "Toggle", Width = 100 };
            TestWindow.Show(toggle, 300, 200);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(toggle));

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(toggle));
            Assert.Equal(Tokens.Color("AccentInactiveBrush"), FillOf(toggle, "border"));
        }

        [AvaloniaFact]
        public void Hover_AToggleButtonThatIsOn_HasTheOnHoverFill()
        {
            var toggle = new ToggleButton { Content = "Toggle", Width = 100, IsChecked = true };
            TestWindow.Show(toggle, 300, 200);

            PseudoClassInput.Force(toggle, ":pointerover");

            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(toggle, "border"));
            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(toggle));
        }

        [AvaloniaFact]
        public void Check_ARadioButton_MakesItsTextTheActiveText()
        {
            var radio = new RadioButton { Content = "Radio", Width = 100 };
            TestWindow.Show(radio, 300, 200);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(radio));

            radio.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(radio));

            PseudoClassInput.Force(radio, ":pointerover");
            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(radio, "radioButtonBorder"));
        }

        [AvaloniaFact]
        public void Select_ARowOfAList_MakesItsTextTheActiveText()
        {
            var list = new ListBox { ItemsSource = new[] { "First", "Second" }, Width = 140, Height = 80 };
            TestWindow.Show(list, 300, 200);
            list.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();
            var rows = list.GetVisualDescendants().OfType<ListBoxItem>().ToList();

            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(rows[0]));
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(rows[1]));

            PseudoClassInput.Force(rows[0], ":pointerover");
            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(rows[0], "Bd"));
        }

        [AvaloniaFact]
        public void Select_ARowOfAComboBox_MakesItsTextTheActiveText()
        {
            var item = new ComboBoxItem { Content = "Item", Width = 140 };
            TestWindow.Show(item, 300, 200);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(item));

            item.IsSelected = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(item));

            PseudoClassInput.Force(item, ":pointerover");
            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(item, "border"));
        }

        // The header of an expander is a toggle that is checked while the expander is open. It is not on the accent: its text does not change.
        [AvaloniaFact]
        public void Open_AnExpander_KeepsTheTextOfItsHeader()
        {
            var expander = new Expander { Header = "Header", Content = "Content", IsExpanded = true, Width = 200 };
            TestWindow.Show(expander, 300, 200);

            var header = expander.GetVisualDescendants().OfType<ToggleButton>().First();

            Assert.True(header.IsChecked);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(header));
        }
    }
}
