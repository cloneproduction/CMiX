// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using Xunit;
using AvaloniaInteraction = Avalonia.Xaml.Interactivity.Interaction;

namespace CMiX.UI.Tests
{
    // The attached property TogglePopup: the popup is open while its toggle is checked, it closes when the pointer goes away,
    // it has a least width, and a value in edit mode keeps it open.
    public class TogglePopupTests
    {
        private static (Window Window, ToggleButton Toggle, Popup Popup) Show(Control? content = null, Action<Popup>? configure = null, double toggleWidth = 120)
        {
            var toggle = new ToggleButton { Content = "Open", Width = toggleWidth, HorizontalAlignment = HorizontalAlignment.Left };
            var popup = new Popup { Child = new Border { Child = content ?? new TextBlock { Text = "Content" } } };
            TogglePopup.SetToggle(popup, toggle);
            configure?.Invoke(popup);
            return (ShowInPanel(toggle, popup), toggle, popup);
        }

        private static Window ShowInPanel(params Control[] children)
        {
            var panel = new StackPanel { Margin = new Thickness(0, 100, 0, 0) };
            panel.Children.AddRange(children);
            return TestWindow.Show(panel, 1200, 800);
        }

        // Checks the toggle and puts the pointer on it, as a click on the toggle does.
        private static void Open(Window window, ToggleButton toggle)
        {
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            PointerInput.Move(window, Middle(window, toggle));
        }

        private static (Window Window, ToggleButton Toggle, Popup Popup) ShowOpen(Control? content = null, Action<Popup>? configure = null, double toggleWidth = 120)
        {
            var (window, toggle, popup) = Show(content, configure, toggleWidth);
            Open(window, toggle);
            Assert.True(popup.IsOpen);
            return (window, toggle, popup);
        }

        // A slider in edit mode in an open popup.
        private static (Window Window, ToggleButton Toggle, Popup Popup, SliderCMiX Slider) ShowEditing()
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.25, Width = 200, Height = 25 };
            var (window, toggle, popup) = ShowOpen(slider);
            slider.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            return (window, toggle, popup, slider);
        }

        private static Point Middle(Window window, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        // A point to the right of the toggle, "away" pixels from its right edge.
        private static Point Away(Window window, Control toggle, double away)
        {
            var left = toggle.TranslatePoint(new Point(0, 0), window)!.Value;
            return new Point(left.X + toggle.Bounds.Width + away, left.Y + 5);
        }

        // The pointer comes back into the content of the popup: the content gets PointerEntered.
        private static void Enter(Window window, Control target)
        {
            target.RaiseEvent(new PointerEventArgs(InputElement.PointerEnteredEvent, target, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true),
                window, new Point(), 0, new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None));
            Dispatcher.UIThread.RunJobs();
        }

        private static TextBox TextBoxOf(Control editor) =>
            editor.GetVisualDescendants().OfType<TextBox>().First(candidate => candidate.Name == "PART_TextBox");

        // The panel that the user sees, without the room of the shadow around it.
        private static double PanelWidth(Popup popup) =>
            Parts.Find<Border>(popup.Child!, "PART_Surface").Bounds.Width;

        // The toggle opens and closes the popup, and the popup checks and unchecks the toggle.

        [AvaloniaFact]
        public void Check_TheToggle_OpensAndClosesThePopup()
        {
            var (_, toggle, popup) = Show();
            Assert.False(popup.IsOpen);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);

            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void Open_ThePopup_ChecksAndUnchecksTheToggle()
        {
            var (_, toggle, popup) = Show();

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(toggle.IsChecked);

            popup.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void SetToggle_APopup_TargetsTheToggleWithOneCloseBehaviorAndNoLightDismiss()
        {
            var (_, toggle, popup) = Show();

            Assert.Same(toggle, popup.PlacementTarget);
            Assert.False(popup.IsLightDismissEnabled);
            Assert.Single(AvaloniaInteraction.GetBehaviors(popup).OfType<ClosePopupBehavior>());
        }

        [AvaloniaFact]
        public void SetToggle_TheSameToggleAgain_AddsNoSecondBehavior()
        {
            var (_, toggle, popup) = Show();

            TogglePopup.SetToggle(popup, null);
            TogglePopup.SetToggle(popup, toggle);

            Assert.Single(AvaloniaInteraction.GetBehaviors(popup).OfType<ClosePopupBehavior>());
        }

        [AvaloniaFact]
        public void SetToggle_ANewToggle_TakesTheControlFromTheOldToggle()
        {
            var (_, oldToggle, popup) = Show();
            var newToggle = new ToggleButton();

            TogglePopup.SetToggle(popup, newToggle);
            oldToggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.False(popup.IsOpen);

            newToggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);
        }

        // Each popup chooses how it closes when the pointer goes away: ClosesOnPointerAway and PointerAwayDistance.

        [AvaloniaFact]
        public void PointerAway_ByDefault_ClosesThePopup()
        {
            var (window, toggle, popup) = ShowOpen();

            PointerInput.Move(window, Away(window, toggle, 200));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_WithoutClosesOnPointerAway_KeepsThePopupOpen()
        {
            var (window, toggle, popup) = ShowOpen(configure: p => TogglePopup.SetClosesOnPointerAway(p, false));

            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PressOutside_WithoutClosesOnPointerAway_ClosesThePopup()
        {
            var (window, toggle, popup) = ShowOpen(configure: p => TogglePopup.SetClosesOnPointerAway(p, false));

            PointerInput.Click(window, Away(window, toggle, 600));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_ABiggerDistance_KeepsThePopupOpenFartherAway()
        {
            var (window, toggle, popup) = ShowOpen(configure: p => TogglePopup.SetPointerAwayDistance(p, 400));

            PointerInput.Move(window, Away(window, toggle, 300));
            Assert.True(popup.IsOpen);

            PointerInput.Move(window, Away(window, toggle, 700));
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void SetClosesOnPointerAway_WhileThePopupIsOpen_TakesEffect()
        {
            var (window, toggle, popup) = ShowOpen();

            TogglePopup.SetClosesOnPointerAway(popup, false);
            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.True(popup.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void AddClosesOnPointerAway_OnAListPanelItem_GoesToItsAddPopup(bool closesOnPointerAway, bool expectedOpen)
        {
            var list = new ListPanelItem
            {
                AddCaption = "Add",
                AddContent = new TextBlock { Text = "Short" },
                AddClosesOnPointerAway = closesOnPointerAway,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var window = ShowInPanel(list);
            var toggle = list.FindControl<ToggleButton>("addToggle")!;
            Open(window, toggle);

            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.Equal(expectedOpen, list.GetLogicalDescendants().OfType<Popup>().Single().IsOpen);
        }

        [AvaloniaTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ClosesOnPointerAway_OnAButtonPopup_GoesToItsPopup(bool closesOnPointerAway, bool expectedOpen)
        {
            var button = new ButtonPopup
            {
                Caption = "Pick",
                PopupContent = new TextBlock { Text = "Short" },
                ClosesOnPointerAway = closesOnPointerAway,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var window = ShowInPanel(button);
            var toggle = button.FindControl<ToggleButton>("popupToggle")!;
            Open(window, toggle);

            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.Equal(expectedOpen, button.GetLogicalDescendants().OfType<Popup>().Single().IsOpen);
        }

        // The width: at least PopupMinWidth, or as wide as the toggle when MatchesToggleWidth is set.

        [AvaloniaFact]
        public void Open_ShortContent_IsAtLeastTheMinimumWidth()
        {
            var minWidth = Tokens.Get<double>("PopupMinWidth");

            var (_, _, popup) = ShowOpen(new TextBlock { Text = "Short" });

            Assert.Equal(minWidth, popup.Child!.MinWidth);
            Assert.True(popup.Child.Bounds.Width >= minWidth);
        }

        [AvaloniaFact]
        public void Open_WideContent_MakesThePopupWiderThanTheMinimum()
        {
            var (_, _, popup) = ShowOpen(new Border { Width = 350, Height = 20 });

            Assert.Equal(350, popup.Child!.Bounds.Width);
        }

        [AvaloniaFact]
        public void MatchesToggleWidth_ShortContent_MakesThePopupAsWideAsTheToggle()
        {
            var (_, toggle, popup) = ShowOpen(new TextBlock { Text = "Short" }, p => TogglePopup.SetMatchesToggleWidth(p, true));

            Assert.Equal(toggle.Bounds.Width, popup.Width);
            Assert.Equal(0.0, popup.Child!.MinWidth);
        }

        [AvaloniaFact]
        public void MatchesToggleWidth_ANewToggleWidth_ResizesThePopup()
        {
            var (_, toggle, popup) = ShowOpen(new TextBlock { Text = "Short" }, p => TogglePopup.SetMatchesToggleWidth(p, true), toggleWidth: 120);

            toggle.Width = 300;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(300, popup.Width);
        }

        [AvaloniaFact]
        public void Open_TheAddPanelOfAListPanelItem_IsAsWideAsItsAddToggle()
        {
            var list = new ListPanelItem { AddCaption = "Add", AddContent = new TextBlock { Text = "Short" }, Width = 400, HorizontalAlignment = HorizontalAlignment.Left };
            TestWindow.Show(list, 900, 500);

            var toggle = list.FindControl<ToggleButton>("addToggle")!;
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = (Popup)list.GetLogicalChildren().OfType<StackPanel>().Single().Children.OfType<Popup>().Single();
            Assert.Equal(400, PanelWidth(popup));
        }

        [AvaloniaFact]
        public void Open_AButtonPopup_IsAsWideAsItsToggle()
        {
            var button = new ButtonPopup { Caption = "Pick", PopupContent = new TextBlock { Text = "Short" }, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
            TestWindow.Show(button, 900, 500);

            var toggle = button.FindControl<ToggleButton>("popupToggle")!;
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = button.GetLogicalDescendants().OfType<Popup>().Single();
            Assert.Same(toggle, TogglePopup.GetToggle(popup));
            Assert.Equal(320, PanelWidth(popup));
        }

        // A value in edit mode keeps the popup open. A press outside ends the edit and the popup stays open.
        // Then only a press outside closes it, until the pointer is back in the popup.

        [AvaloniaFact]
        public void PointerAway_WhileAValueIsEdited_KeepsThePopupOpen()
        {
            var (window, toggle, popup, slider) = ShowEditing();

            PointerInput.Move(window, Away(window, toggle, 400));

            Assert.True(popup.IsOpen);
            Assert.True(slider.IsEditing);
        }

        [AvaloniaFact]
        public void PressOutside_WhileAValueIsEdited_EndsTheEditAndKeepsTheValue()
        {
            var (window, toggle, popup, slider) = ShowEditing();
            var box = TextBoxOf(slider);
            box.Text = "0.75";
            // In the application a popup is a window of its own, and the click in the main window does not reach the
            // editor. The pointer in the text box takes the click handler of the editor away, as the window does.
            Enter(window, box);

            PointerInput.Click(window, Away(window, toggle, 400));

            Assert.False(slider.IsEditing);
            Assert.Equal(0.75, slider.Value, 3);
            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_AfterAnEdit_ClosesOnlyOnAPressOutside()
        {
            var (window, toggle, popup, _) = ShowEditing();
            PointerInput.Click(window, Away(window, toggle, 400));

            PointerInput.Move(window, Away(window, toggle, 500));
            Assert.True(popup.IsOpen);

            PointerInput.Click(window, Away(window, toggle, 600));
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_AfterThePointerCameBack_ClosesThePopup()
        {
            var (window, toggle, popup, _) = ShowEditing();
            PointerInput.Click(window, Away(window, toggle, 400));

            Enter(window, popup.Child!);
            PointerInput.Move(window, Away(window, toggle, 500));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_WithoutAnEdit_ClosesThePopup()
        {
            var (window, toggle, popup, slider) = ShowEditing();
            slider.IsEditing = false;
            Dispatcher.UIThread.RunJobs();

            PointerInput.Move(window, Away(window, toggle, 400));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void EndEdit_ASlider_KeepsTheTypedValue()
        {
            var (_, _, _, slider) = ShowEditing();
            TextBoxOf(slider).Text = "0.5";

            ((IEditableValue)slider).EndEdit();

            Assert.False(slider.IsEditing);
            Assert.Equal(0.5, slider.Value, 3);
        }

        [AvaloniaFact]
        public void EndEdit_ADragValue_KeepsTheTypedValue()
        {
            var drag = new DragValue { Minimum = 0, Maximum = 10, Value = 1, Width = 200, Height = 25 };
            TestWindow.Show(drag, 400, 200);
            drag.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            TextBoxOf(drag).Text = "7";

            ((IEditableValue)drag).EndEdit();

            Assert.False(drag.IsEditing);
            Assert.Equal(7, drag.Value, 3);
        }

        [AvaloniaFact]
        public void EndEdit_WithoutAnEdit_DoesNothing()
        {
            var (_, _, _, slider) = ShowEditing();
            slider.IsEditing = false;
            Dispatcher.UIThread.RunJobs();

            ((IEditableValue)slider).EndEdit();

            Assert.Equal(0.25, slider.Value, 3);
        }
    }
}
