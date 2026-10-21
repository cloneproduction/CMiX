using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // The controls of the row take the corners in the order that they show on screen.
    public class DockPanelPositionedTests
    {
        private static Button Docked(Dock dock) 
        {
            var button = new Button();
            DockPanel.SetDock(button, dock);
            return button;
        }

        private static ControlPosition PositionOf(Control control) => PositionedControl.GetPosition(control);

        [AvaloniaFact]
        public void OneChild_IsRoundedOnAllCorners()
        {
            var panel = new DockPanelPositioned();
            var only = new Button();
            panel.Children.Add(only);

            Assert.Equal(ControlPosition.All, PositionOf(only));
        }

        [AvaloniaFact]
        public void LeftFillAndRight_AreLeftNoneRight()
        {
            var panel = new DockPanelPositioned();
            var left = Docked(Dock.Left);
            var right = Docked(Dock.Right);
            var fill = new Button();
            panel.Children.Add(right);
            panel.Children.Add(left);
            panel.Children.Add(fill);

            Assert.Equal(ControlPosition.Left, PositionOf(left));
            Assert.Equal(ControlPosition.None, PositionOf(fill));
            Assert.Equal(ControlPosition.Right, PositionOf(right));
        }

        [AvaloniaFact]
        public void RightSide_IsReadFromTheRightEdge()
        {
            var panel = new DockPanelPositioned();
            var outer = Docked(Dock.Right);
            var inner = Docked(Dock.Right);
            var fill = new Button();
            panel.Children.Add(outer);
            panel.Children.Add(inner);
            panel.Children.Add(fill);

            Assert.Equal(ControlPosition.Left, PositionOf(fill));
            Assert.Equal(ControlPosition.None, PositionOf(inner));
            Assert.Equal(ControlPosition.Right, PositionOf(outer));
        }

        [AvaloniaFact]
        public void APopup_IsNotPartOfTheRow()
        {
            var panel = new DockPanelPositioned();
            var left = Docked(Dock.Left);
            var popup = new Popup();
            var fill = new Button();
            panel.Children.Add(left);
            panel.Children.Add(popup);
            panel.Children.Add(fill);

            Assert.Equal(ControlPosition.Left, PositionOf(left));
            Assert.Equal(ControlPosition.Right, PositionOf(fill));
        }

        [AvaloniaFact]
        public void RemovingAChild_GivesTheNewEndTheCorner()
        {
            var panel = new DockPanelPositioned();
            var left = Docked(Dock.Left);
            var fill = new Button();
            var right = Docked(Dock.Right);
            panel.Children.Add(right);
            panel.Children.Add(left);
            panel.Children.Add(fill);

            panel.Children.Remove(right);

            Assert.Equal(ControlPosition.Right, PositionOf(fill));
        }
    }
}
