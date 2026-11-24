// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A button that opens a popup with any content, in a DropdownFrame under the button.
    public class ButtonPopupTests
    {
        private const double ShadowRoom = 8;

        private sealed record Shown(Window Window, ButtonPopup Button, ToggleButton Toggle, Popup Popup);

        private static Shown Show(Control? content = null, double width = 300, Action<ButtonPopup>? configure = null)
        {
            var button = new ButtonPopup
            {
                Caption = "Add",
                PopupContent = content ?? new TextBlock { Text = "Short" },
                Width = width,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(100, 40, 0, 0)
            };
            configure?.Invoke(button);
            var window = TestWindow.Show(button, 1200, 800);
            return new Shown(window, button, button.FindControl<ToggleButton>("popupToggle")!, button.GetLogicalDescendants().OfType<Popup>().Single());
        }

        private static Shown ShowOpen(Control? content = null, double width = 300, Action<ButtonPopup>? configure = null)
        {
            var shown = Show(content, width, configure);
            Open(shown.Toggle);
            return shown;
        }

        private static void Open(ToggleButton toggle)
        {
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
        }

        // The panel that the user sees, without the room of the shadow around it.
        private static Border PanelOf(Popup popup) => Parts.Find<Border>(popup.Child!, "PART_Surface");

        // Where a control is on the screen.
        private static (double Left, double Width, double Top) OnScreen(Control control)
        {
            var point = control.PointToScreen(new Point(0, 0));
            return (point.X, control.Bounds.Width, point.Y);
        }

        private static Point Middle(Window window, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        // A point to the right of the toggle, "away" pixels from its right edge.
        private static Point Away(Window window, Control toggle, double away)
        {
            var left = toggle.TranslatePoint(new Point(0, 0), window)!.Value;
            return new Point(left.X + toggle.Bounds.Width + away, left.Y + 5);
        }

        [AvaloniaFact]
        public void Show_ACaptionAndAContent_ShowsTheCaptionAndKeepsTheContentInTheLogicalTree()
        {
            var content = new TextBlock { Text = "choice" };

            var shown = Show(content, width: 200);

            Assert.Equal("Add", shown.Toggle.Content);
            Assert.Same(shown.Button, ((ILogical)content).LogicalParent);
        }

        [AvaloniaFact]
        public void Check_TheToggle_OpensAndClosesThePopup()
        {
            var shown = Show();
            Assert.False(shown.Popup.IsOpen);

            shown.Toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(shown.Popup.IsOpen);

            shown.Toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(shown.Popup.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ClosesOnPointerAway_EachValue_GoesToThePopup(bool closesOnPointerAway, bool expectedOpen)
        {
            var shown = ShowOpen(configure: button => button.ClosesOnPointerAway = closesOnPointerAway);
            PointerInput.Move(shown.Window, Middle(shown.Window, shown.Toggle));

            PointerInput.Move(shown.Window, Away(shown.Window, shown.Toggle, 600));

            Assert.Equal(expectedOpen, shown.Popup.IsOpen);
        }

        [AvaloniaFact]
        public void Open_ThePopup_HoldsADropdownFrame()
        {
            var shown = ShowOpen();

            Assert.IsType<DropdownFrame>(shown.Popup.Child);
        }

        [AvaloniaFact]
        public void Open_ThePanel_HasThePopupBackground()
        {
            var shown = ShowOpen();

            Assert.Equal(Tokens.Color("PopupBackgroundBrush"), ((ISolidColorBrush)PanelOf(shown.Popup).Background!).Color);
        }

        [AvaloniaFact]
        public void Open_TheFrame_CastsTheWindowShadow()
        {
            var shown = ShowOpen();

            var shadowed = shown.Popup.Child!.GetVisualDescendants().OfType<Border>().Where(border => border.Effect != null).ToList();
            Assert.Single(shadowed);
            Assert.Same(Tokens.Get<object>("DropShadowWindow"), shadowed[0].Effect);
        }

        [AvaloniaFact]
        public void Open_ThePopup_MovesUpByTheRoomOfTheShadowOnly()
        {
            var shown = ShowOpen();

            Assert.Equal(0, shown.Popup.HorizontalOffset);
            Assert.Equal(-ShadowRoom, shown.Popup.VerticalOffset);
            Assert.Equal(300, PanelOf(shown.Popup).Bounds.Width);
        }

        [AvaloniaFact]
        public void Open_ASecondTime_KeepsTheOffsets()
        {
            var shown = ShowOpen();
            shown.Toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            Open(shown.Toggle);

            Assert.Equal(0, shown.Popup.HorizontalOffset);
            Assert.Equal(-ShadowRoom, shown.Popup.VerticalOffset);
        }

        [AvaloniaFact]
        public void Open_ThePanel_StartsAtTheBottomEdgeOfTheToggle()
        {
            var shown = ShowOpen();

            var target = OnScreen(shown.Toggle);
            Assert.Equal(target.Top + shown.Toggle.Bounds.Height, OnScreen(PanelOf(shown.Popup)).Top);
        }

        [AvaloniaFact]
        public void Open_ThePanel_StartsAtTheLeftEdgeOfTheToggleAndIsAsWide()
        {
            var shown = ShowOpen();

            var panel = OnScreen(PanelOf(shown.Popup));
            var target = OnScreen(shown.Toggle);
            Assert.Equal(target.Left, panel.Left);
            Assert.Equal(target.Width, panel.Width);
        }

        [AvaloniaFact]
        public void Open_AWiderButton_MakesThePanelAsWideAsTheToggle()
        {
            var shown = ShowOpen(width: 320);

            Assert.Same(shown.Toggle, TogglePopup.GetToggle(shown.Popup));
            Assert.Equal(320, PanelOf(shown.Popup).Bounds.Width);
        }
    }
}
