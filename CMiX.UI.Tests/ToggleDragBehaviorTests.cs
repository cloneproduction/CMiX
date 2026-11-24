using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A drag over toggle buttons paints them with the state of the first button.
    public class ToggleDragBehaviorTests
    {
        private sealed class Rows
        {
            public Window Window { get; init; } = null!;
            public ToggleDragBehavior Behavior { get; init; } = null!;
            public List<ToggleButton> Visibility { get; } = new();
            public List<ToggleButton> Mask { get; } = new();
            public List<ToggleButton> Plain { get; } = new();

            public Point CenterOf(ToggleButton button)
            {
                Window.UpdateLayout();
                return button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), Window)!.Value;
            }

            public void Down(ToggleButton button) => Window.MouseDown(CenterOf(button), MouseButton.Left);

            public void MoveTo(ToggleButton button, RawInputModifiers modifiers = RawInputModifiers.LeftMouseButton) =>
                Window.MouseMove(CenterOf(button), modifiers);

            public void Up(ToggleButton button) => Window.MouseUp(CenterOf(button), MouseButton.Left);
        }

        // Five rows. Each row has a visibility button, a mask button and a button without a group.
        private static Rows Show(int count = 5)
        {
            var behavior = new ToggleDragBehavior();
            var panel = new StackPanel();
            var window = new Window { Content = panel, Width = 200, Height = 400 };
            Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window).Add(behavior);
            var rows = new Rows { Window = window, Behavior = behavior };

            for (var i = 0; i < count; i++)
            {
                var visibility = Button("v", "visibility");
                var mask = Button("m", "mask");
                var plain = Button("p", null);
                rows.Visibility.Add(visibility);
                rows.Mask.Add(mask);
                rows.Plain.Add(plain);
                panel.Children.Add(new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Children = { visibility, mask, plain } });
            }

            window.Show();
            Dispatcher.UIThread.RunJobs();
            TestWindow.Render();
            return rows;
        }

        private static ToggleButton Button(string text, string? group)
        {
            var button = new ToggleButton { Content = text, Width = 40, Height = 24 };
            if (group != null)
                ToggleDrag.SetGroup(button, group);
            return button;
        }

        [AvaloniaFact]
        public void Press_AButton_TogglesItOnceAtOnce()
        {
            var rows = Show();

            rows.Down(rows.Visibility[0]);
            Assert.True(rows.Visibility[0].IsChecked);

            rows.Up(rows.Visibility[0]);
            Assert.True(rows.Visibility[0].IsChecked);
        }

        [AvaloniaFact]
        public void Drag_Down_PaintsEveryButtonWithTheNewStateOfTheFirst()
        {
            var rows = Show();

            rows.Down(rows.Visibility[0]);
            for (var i = 1; i < 4; i++)
                rows.MoveTo(rows.Visibility[i]);
            rows.Up(rows.Visibility[3]);

            Assert.All(rows.Visibility.Take(4), button => Assert.True(button.IsChecked));
            Assert.False(rows.Visibility[4].IsChecked);
        }

        [AvaloniaFact]
        public void Drag_FromAnOnButton_SwitchesTheOthersOff()
        {
            var rows = Show();
            foreach (var index in new[] { 0, 1, 3 })
                rows.Visibility[index].IsChecked = true;

            rows.Down(rows.Visibility[0]);
            for (var i = 1; i < 5; i++)
                rows.MoveTo(rows.Visibility[i]);
            rows.Up(rows.Visibility[4]);

            Assert.All(rows.Visibility, button => Assert.False(button.IsChecked));
        }

        [AvaloniaFact]
        public void Drag_Fast_SkipsNoButton()
        {
            var rows = Show();

            rows.Down(rows.Visibility[0]);
            rows.MoveTo(rows.Visibility[4]);
            rows.Up(rows.Visibility[4]);

            Assert.All(rows.Visibility, button => Assert.True(button.IsChecked));
        }

        [AvaloniaFact]
        public void Drag_Up_PaintsTheButtons()
        {
            var rows = Show();

            rows.Down(rows.Visibility[4]);
            rows.MoveTo(rows.Visibility[0]);
            rows.Up(rows.Visibility[0]);

            Assert.All(rows.Visibility, button => Assert.True(button.IsChecked));
        }

        [AvaloniaFact]
        public void Drag_OnTheVisibilityColumn_KeepsTheMaskButtons()
        {
            var rows = Show();

            rows.Down(rows.Visibility[0]);
            rows.MoveTo(rows.Mask[2]);
            rows.MoveTo(rows.Mask[4]);
            rows.Up(rows.Mask[4]);

            Assert.All(rows.Mask, button => Assert.False(button.IsChecked));
            Assert.True(rows.Visibility[0].IsChecked);
        }

        [AvaloniaFact]
        public void Drag_OnTheMaskColumn_PaintsTheMaskButtonsOnly()
        {
            var rows = Show();

            rows.Down(rows.Mask[0]);
            rows.MoveTo(rows.Mask[3]);
            rows.Up(rows.Mask[3]);

            Assert.All(rows.Mask.Take(4), button => Assert.True(button.IsChecked));
            Assert.All(rows.Visibility, button => Assert.False(button.IsChecked));
        }

        [AvaloniaFact]
        public void Press_AButtonWithoutAGroup_TogglesOnReleaseWithNoStroke()
        {
            var rows = Show();

            rows.Down(rows.Plain[0]);
            Assert.False(rows.Plain[0].IsChecked);
            Assert.False(rows.Behavior.IsStroking);
            rows.MoveTo(rows.Plain[2]);
            rows.Up(rows.Plain[2]);

            Assert.All(rows.Plain, button => Assert.False(button.IsChecked));

            rows.Down(rows.Plain[1]);
            rows.Up(rows.Plain[1]);
            Assert.True(rows.Plain[1].IsChecked);
        }

        [AvaloniaFact]
        public void Move_AfterTheRelease_PaintsNothing()
        {
            var rows = Show();

            rows.Down(rows.Visibility[0]);
            rows.Up(rows.Visibility[0]);
            rows.MoveTo(rows.Visibility[3], RawInputModifiers.None);
            rows.MoveTo(rows.Visibility[4]);

            Assert.True(rows.Visibility[0].IsChecked);
            Assert.All(rows.Visibility.Skip(1), button => Assert.False(button.IsChecked));
        }

        [AvaloniaFact]
        public void Move_WithoutThePressedMouseButton_EndsTheStroke()
        {
            var rows = Show();

            rows.Down(rows.Visibility[0]);
            rows.MoveTo(rows.Visibility[1], RawInputModifiers.None);
            rows.MoveTo(rows.Visibility[3]);

            Assert.False(rows.Behavior.IsStroking);
            Assert.False(rows.Visibility[2].IsChecked);
            Assert.False(rows.Visibility[3].IsChecked);
        }

        [AvaloniaFact]
        public void Drag_OverADisabledButton_LeavesItAndStartsNoStroke()
        {
            var rows = Show();
            rows.Visibility[2].IsEnabled = false;

            rows.Down(rows.Visibility[0]);
            rows.MoveTo(rows.Visibility[4]);
            rows.Up(rows.Visibility[4]);

            Assert.False(rows.Visibility[2].IsChecked);
            Assert.True(rows.Visibility[3].IsChecked);

            var other = Show();
            other.Visibility[0].IsEnabled = false;
            other.Down(other.Visibility[0]);
            Assert.False(other.Behavior.IsStroking);
        }

        [AvaloniaFact]
        public void Press_OutsideTheButtons_StartsNoStroke()
        {
            var rows = Show();

            rows.Window.MouseDown(new Point(190, 390), MouseButton.Left);

            Assert.False(rows.Behavior.IsStroking);
        }

        [AvaloniaFact]
        public void Drag_AStroke_RaisesStartedFirstAndEndedLast()
        {
            var rows = Show();
            var log = new List<string>();
            rows.Behavior.StrokeStarted += (_, _) => log.Add("started:" + rows.Visibility[0].IsChecked);
            rows.Behavior.StrokeEnded += (_, _) => log.Add("ended:" + rows.Visibility[2].IsChecked);

            rows.Down(rows.Visibility[0]);
            rows.MoveTo(rows.Visibility[2]);
            rows.Up(rows.Visibility[2]);

            Assert.Equal(new[] { "started:False", "ended:True" }, log);
        }

        [AvaloniaFact]
        public void Drag_AnyStrokeOrClick_RaisesOneStartAndOneEnd()
        {
            var rows = Show();
            var started = 0;
            var ended = 0;
            rows.Behavior.StrokeStarted += (_, _) => started++;
            rows.Behavior.StrokeEnded += (_, _) => ended++;

            rows.Down(rows.Visibility[0]);
            rows.Up(rows.Visibility[0]);
            rows.Down(rows.Visibility[1]);
            rows.MoveTo(rows.Visibility[2]);
            rows.Up(rows.Visibility[2]);

            Assert.Equal(2, started);
            Assert.Equal(2, ended);
        }

        [AvaloniaFact]
        public void Group_SetAndCleared_SwitchesThePressModeOnAndOff()
        {
            var button = new ToggleButton();
            Assert.Equal(ClickMode.Release, button.ClickMode);

            ToggleDrag.SetGroup(button, "visibility");
            Assert.Equal(ClickMode.Press, button.ClickMode);

            ToggleDrag.SetGroup(button, null);
            Assert.Equal(ClickMode.Release, button.ClickMode);
        }

        [AvaloniaFact]
        public void Detach_DuringAStroke_EndsItAndStopsTheInput()
        {
            var rows = Show();
            var ended = 0;
            rows.Behavior.StrokeEnded += (_, _) => ended++;

            rows.Down(rows.Visibility[0]);
            Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(rows.Window).Remove(rows.Behavior);
            rows.MoveTo(rows.Visibility[3]);

            Assert.Equal(1, ended);
            Assert.False(rows.Visibility[3].IsChecked);
        }
    }
}
