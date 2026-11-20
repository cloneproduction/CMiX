using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using Xunit;

namespace CMiX.UI.Tests
{
    // A tooltip opens on one side of its control, 8 pixels away, and follows the pointer along the control. Its tip points at the pointer.
    public class ToolTipDefaultsTests
    {
        private static (Window Window, Button Button) Show(double left = 40, double top = 100, double width = 80)
        {
            var button = new Button
            {
                Content = "Hover", Width = width, Height = 24,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(left, top, 0, 0),
            };
            ToolTip.SetTip(button, "A tip");
            var window = new Window { Content = button, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            ToolTipDefaults.AnchoringOf(button)!.Bounds = new Rect(0, 0, 400, 300);
            return (window, button);
        }

        private static (ToolTipCard Card, Rect Box) Open(Window window, Button button)
        {
            ToolTip.SetIsOpen(button, true);
            Dispatcher.UIThread.RunJobs();
            return Read(window);
        }

        private static (ToolTipCard Card, Rect Box) Read(Window window)
        {
            var card = window.GetVisualDescendants().OfType<ToolTipCard>().First();
            var box = card.GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_Surface");
            return (card, new Rect(box.TranslatePoint(default, window)!.Value, box.Bounds.Size));
        }

        private static void Move(Window window, Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Placement_ByDefault_IsCustomForEveryControl()
        {
            foreach (var control in new Control[] { new Button(), new TextBlock(), new Border(), new ToggleButton(), new CMiX.UI.Controls.AppIcon() })
            {
                ToolTip.SetTip(control, "A tip");
                Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(control));
                Assert.NotNull(ToolTip.GetCustomPopupPlacementCallback(control));
            }
        }

        [AvaloniaFact]
        public void Placement_SetForOneControl_StaysAsSet()
        {
            var button = new Button();
            ToolTip.SetPlacement(button, PlacementMode.Right);
            ToolTip.SetVerticalOffset(button, 3);

            ToolTip.SetTip(button, "A tip");

            Assert.Equal(PlacementMode.Right, ToolTip.GetPlacement(button));
            Assert.Equal(3, ToolTip.GetVerticalOffset(button));
            Assert.Null(ToolTip.GetCustomPopupPlacementCallback(button));
        }

        [AvaloniaFact]
        public void Placement_ATipFromABinding_IsSetToo()
        {
            var button = new Button { DataContext = new { Text = "From a binding" } };
            button.Bind(ToolTip.TipProperty, new Avalonia.Data.Binding("Text"));

            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(button));
        }

        [AvaloniaFact]
        public void Show_TheTooltipTheme_HasTheShadowRoomOfTheOtherPopups()
        {
            var (window, button) = Show();

            var (card, box) = Open(window, button);

            var origin = card.TranslatePoint(default, window)!.Value;
            Assert.Equal(PopupFrame.ShadowMargin.Left, box.X - origin.X, 0.51);
            Assert.Equal(PopupFrame.ShadowMargin.Top, box.Y - origin.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_ATooltip_IsEightPixelsBelowTheControl()
        {
            var (window, button) = Show();

            var (_, box) = Open(window, button);

            Assert.Equal(100 + 24 + ToolTipDefaults.Gap, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_ATooltip_ShowsTheTipOnTheTopEdge()
        {
            var (window, button) = Show();

            var (card, _) = Open(window, button);

            Assert.True(card.ShowsTip);
            Assert.Equal(PopupSide.Top, card.TipEdge);
        }

        [AvaloniaFact]
        public void Open_WithThePointerOnTheControl_PutsTheCardAndTheTipOnThePointer()
        {
            var (window, button) = Show(width: 300);
            Move(window, new Point(150, 110));

            var (card, box) = Open(window, button);

            Assert.Equal(150 - ToolTipDefaults.PointerAlign, box.X, 0.51);
            Assert.Equal(ToolTipDefaults.PointerAlign, card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Move_ThePointerWhileTheTooltipIsOpen_KeepsTheCardWhereItOpened()
        {
            var (window, button) = Show(width: 300);
            Move(window, new Point(150, 110));
            Open(window, button);

            Move(window, new Point(220, 110));

            var (card, box) = Read(window);
            Assert.Equal(150 - ToolTipDefaults.PointerAlign, box.X, 0.51);
            Assert.Equal(ToolTipDefaults.PointerAlign, card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Open_WithThePointerNearTheRightEdge_StopsAtTheMarginAndKeepsTheTipOnThePointer()
        {
            var (window, button) = Show(left: 20, width: 360);
            Move(window, new Point(370, 110));

            var (card, box) = Open(window, button);

            Assert.Equal(400 - 8, box.Right, 0.51);
            Assert.Equal(370 - box.X, card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Open_NoRoomBelow_OpensAboveWithTheTipOnTheBottomEdge()
        {
            var (window, button) = Show(top: 270);

            var (card, box) = Open(window, button);

            Assert.Equal(270 - ToolTipDefaults.Gap, box.Bottom, 0.51);
            Assert.Equal(PopupSide.Bottom, card.TipEdge);
        }

        [AvaloniaTheory]
        [InlineData(PopupSide.Right)]
        [InlineData(PopupSide.Left)]
        public void Open_ASideTooltip_OpensBesideTheControlAndFollowsThePointerInY(PopupSide side)
        {
            var (window, button) = Show(left: 150, top: 100, width: 80);
            ToolTipDefaults.SetSide(button, side);
            Move(window, new Point(190, 110));

            var (card, box) = Open(window, button);

            if (side == PopupSide.Right)
                Assert.Equal(150 + 80 + ToolTipDefaults.Gap, box.X, 0.51);
            else
                Assert.Equal(150 - ToolTipDefaults.Gap, box.Right, 0.51);

            Assert.Equal(110 - ToolTipDefaults.PointerAlign, box.Y, 0.51);
            Assert.Equal(side == PopupSide.Right ? PopupSide.Left : PopupSide.Right, card.TipEdge);
        }
    }
}
