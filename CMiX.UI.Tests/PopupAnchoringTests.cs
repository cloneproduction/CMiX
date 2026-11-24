using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Popups;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A tooltip, a popup and a context menu open where the rule of PopupPlacement says. The tests open real popups in a window.
    public class PopupAnchoringTests
    {
        private static readonly Rect WindowBounds = new(0, 0, 300, 200);

        private static (Window Window, Button Button) Show(double top = 20, Control? extra = null)
        {
            var button = new Button
            {
                Content = "Hover", Width = 80, Height = 24,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(40, top, 0, 0),
            };
            var panel = new Panel { Children = { button } };
            if (extra != null)
                panel.Children.Add(extra);

            return (TestWindow.Show(panel, 300, 200), button);
        }

        // The box of the tooltip is the first border of its template. It has the room of the shadow around it.
        private static (Rect Box, Border Border) OpenTip(Window window, Button button)
        {
            ToolTip.SetIsOpen(button, true);
            Dispatcher.UIThread.RunJobs();
            var tip = window.GetVisualDescendants().OfType<ToolTip>().First();
            var border = tip.GetVisualDescendants().OfType<Border>().First(part => part.Name == "PART_Surface");
            var at = border.TranslatePoint(default, window)!.Value;
            return (new Rect(at, border.Bounds.Size), border);
        }

        private static PopupAnchoring UseOnTip(Button button, PopupSide side)
        {
            ToolTip.SetTip(button, "A tip");
            var anchoring = PopupAnchoring.Use(button);
            anchoring.Side = side;
            anchoring.Bounds = WindowBounds;
            return anchoring;
        }

        [AvaloniaFact]
        public void Use_AControl_GivesItsTooltipTheCustomPlacement()
        {
            var button = new Button();
            var anchoring = PopupAnchoring.Use(button);

            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(button));
            Assert.NotNull(ToolTip.GetCustomPopupPlacementCallback(button));
            Assert.NotNull(anchoring);
        }

        [AvaloniaFact]
        public void Use_APopupAMenuOrAFlyout_GivesThemTheCustomPlacement()
        {
            var popup = new Popup();
            var menu = new ContextMenu();
            PopupAnchoring.Use(popup);
            PopupAnchoring.Use(menu);
            var flyout = new Flyout();
            PopupAnchoring.Use(flyout);

            Assert.Equal(PlacementMode.Custom, popup.Placement);
            Assert.NotNull(popup.CustomPopupPlacementCallback);
            Assert.Equal(PlacementMode.Custom, menu.Placement);
            Assert.NotNull(menu.CustomPopupPlacementCallback);
            Assert.Equal(PlacementMode.Custom, flyout.Placement);
            Assert.NotNull(flyout.CustomPopupPlacementCallback);
        }

        [AvaloniaFact]
        public void Open_ABottomTooltip_PutsTheCardAtTheGapBelowTheControlAndCentersIt()
        {
            var (window, button) = Show();
            UseOnTip(button, PopupSide.Bottom);

            var (box, _) = OpenTip(window, button);

            Assert.Equal(20 + 24 + 8, box.Y, 0.51);
            Assert.Equal(80 - box.Width / 2, box.X, 0.51);
        }

        [AvaloniaFact]
        public void Open_ARightTooltip_PutsTheCardAtTheGapAtTheRightOfTheControl()
        {
            var (window, button) = Show();
            UseOnTip(button, PopupSide.Right);

            var (box, _) = OpenTip(window, button);

            Assert.Equal(40 + 80 + 8, box.X, 0.51);
            Assert.Equal(32 - box.Height / 2, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_ALeftTooltip_PutsTheCardAtTheGapAtTheLeftOfTheControl()
        {
            var (window, button) = Show();
            var anchoring = UseOnTip(button, PopupSide.Left);
            anchoring.Bounds = new Rect(-100, 0, 400, 200);

            var (box, _) = OpenTip(window, button);

            Assert.Equal(40 - 8 - box.Width, box.X, 0.51);
            Assert.Equal(32 - box.Height / 2, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_ATopTooltip_PutsTheCardAtTheGapAboveTheControl()
        {
            var (window, button) = Show(top: 120);
            UseOnTip(button, PopupSide.Top);

            var (box, _) = OpenTip(window, button);

            Assert.Equal(120 - 8 - box.Height, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_NoRoomBelow_FlipsTheCardAboveTheControl()
        {
            var (window, button) = Show(top: 150);
            var anchoring = UseOnTip(button, PopupSide.Bottom);

            var (box, _) = OpenTip(window, button);

            Assert.Equal(PopupSide.Top, anchoring.Last!.Value.Side);
            Assert.Equal(150 - 8 - box.Height, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_WithAPointAlong_PutsTheCardAndTheTipOnThePoint()
        {
            var (window, button) = Show();
            var anchoring = UseOnTip(button, PopupSide.Bottom);
            anchoring.Along = 100;
            anchoring.AlignOffset = 28;

            var (box, _) = OpenTip(window, button);

            Assert.Equal(72, box.X, 0.51);
            Assert.Equal(28, anchoring.Last!.Value.TipOffset, 0.01);
        }

        [AvaloniaFact]
        public void Open_ATooltip_RaisesPlacedWithTheResult()
        {
            var (window, button) = Show();
            var anchoring = UseOnTip(button, PopupSide.Bottom);
            PlacementResult? seen = null;
            anchoring.Placed += result => seen = result;

            OpenTip(window, button);

            Assert.NotNull(seen);
            Assert.Equal(anchoring.Last, seen);
        }

        [AvaloniaFact]
        public void Open_APopupWithoutShadow_PutsItsChildAtTheCorner()
        {
            var child = new Border { Width = 100, Height = 40 };
            var popup = new Popup { Child = child, IsLightDismissEnabled = false };
            var (window, button) = Show(extra: popup);
            popup.PlacementTarget = button;
            var anchoring = PopupAnchoring.Use(popup);
            anchoring.Shadow = default;
            anchoring.Bounds = WindowBounds;

            popup.Open();
            Dispatcher.UIThread.RunJobs();

            var at = child.TranslatePoint(default, window)!.Value;
            Assert.Equal(52, at.Y, 0.51);
            Assert.Equal(80 - 50, at.X, 0.51);
        }

        [AvaloniaFact]
        public void RectOf_AControlInAWindow_IsItsBoundsInTheWindow()
        {
            var (_, button) = Show();

            Assert.Equal(new Rect(40, 20, 80, 24), PopupAnchoring.RectOf(button));
        }

        [AvaloniaFact]
        public void RectOf_AControlOutsideAWindow_IsEmpty()
        {
            Assert.Equal(default, PopupAnchoring.RectOf(new Button()));
        }

        [AvaloniaFact]
        public void WorkingAreaOf_AControl_HoldsTheControl()
        {
            var (_, button) = Show();

            var area = PopupAnchoring.WorkingAreaOf(button);

            Assert.True(area.Contains(PopupAnchoring.RectOf(button).TopLeft));
            Assert.True(area.Width > 0 && area.Height > 0);
        }
    }
}
