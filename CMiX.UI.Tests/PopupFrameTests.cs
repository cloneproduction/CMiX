using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // The three frames: the PopupFrame template and its shadow, the DropdownFrame of a combo box, the FlyoutPresenterPanel,
    // and the least width of a menu.
    public class PopupFrameTests
    {
        // The PopupFrame theme gives the frame its template.

        [AvaloniaFact]
        public void PopupFrame_InAWindow_PresentsItsContentInTheThemedTemplate()
        {
            var text = new TextBlock { Text = "Item" };
            var frame = new PopupFrame { Content = text };
            TestWindow.Show(frame, 400, 300);
            frame.ApplyTemplate();

            var surface = Assert.IsType<PopupSurface>(frame.GetVisualChildren().Single());
            Assert.NotNull(Assert.IsType<Border>(surface.GetVisualChildren().Single()).Effect);
            var presenter = frame.GetVisualDescendants().OfType<ContentPresenter>()
                .Single(p => p.Name == "PART_ContentPresenter" && p.TemplatedParent == frame);
            Assert.Same(text, presenter.Content);
        }

        [AvaloniaFact]
        public void PopupFlyoutTheme_InAWindow_WrapsItsContentInAPopupFrame()
        {
            var text = new TextBlock { Text = "Item" };
            var presenter = new FlyoutPresenter
            {
                Theme = Tokens.Theme("FlyoutPresenterPanel"),
                Content = text
            };
            TestWindow.Show(presenter, 400, 300);

            var frame = presenter.GetVisualDescendants().OfType<PopupFrame>().Single();
            Assert.Equal(300, presenter.Width);
            Assert.Contains(text, frame.GetVisualDescendants());
        }

        [AvaloniaFact]
        public void PopupFlyoutTheme_CentersTheFlyoutOnThePointer_WhateverItsWidth()
        {
            var presenter = new FlyoutPresenter
            {
                Theme = Tokens.Theme("FlyoutPresenterPanel"),
                Content = new TextBlock { Text = "Item" },
                Width = 420
            };
            TestWindow.Show(presenter, 400, 300);

            var frame = presenter.GetVisualDescendants().OfType<PopupFrame>().Single();
            Assert.True(frame.CentersOnPointer);
            Assert.Equal(-frame.Bounds.Width / 2, frame.PointerOffset(frame.Bounds.Size).X);
            Assert.Equal(-frame.Bounds.Height / 2, frame.PointerOffset(frame.Bounds.Size).Y);
        }

        // The dropdown panels share one frame: the same color, the same shadow, and the position that they had without the shadow.

        private const double ShadowRoom = 8;

        private static Border PanelOf(Popup popup) =>
            popup.Child!.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "PART_Surface");

        [AvaloniaFact]
        public void Open_AnEdgeAlignedComboBox_StartsThePanelAtItsLeftEdge()
        {
            var theme = Tokens.Theme("ComboBoxWithButton");
            var combo = new ComboBox { Theme = theme, ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0, Width = 120, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            TestWindow.Show(combo, 900, 500);

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var panel = OnScreen(PanelOf(combo.GetVisualDescendants().OfType<Popup>().Single()));
            Assert.Equal(OnScreen(combo).Left, panel.Left);
            Assert.Equal(250, panel.Width);
        }

        // Where the panel is on the screen, against the control that it hangs under.
        private static (double Left, double Width, double Top) OnScreen(Control control)
        {
            var point = control.PointToScreen(new Point(0, 0));
            return (point.X, control.Bounds.Width, point.Y);
        }

        [AvaloniaFact]
        public void Open_AComboBox_ShowsADropdownFrameAsWideAsTheComboBox()
        {
            var combo = new ComboBox { ItemsSource = new[] { "One", "Two", "Three" }, SelectedIndex = 0, Width = 220, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            TestWindow.Show(combo, 900, 500);

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
            Assert.IsType<DropdownFrame>(popup.Child);
            Assert.Equal(220, PanelOf(popup).Bounds.Width);
            Assert.Equal(OnScreen(combo).Left, OnScreen(PanelOf(popup)).Left);
            Assert.Equal(2 - ShadowRoom, popup.VerticalOffset);
        }

        // Every menu has the same least width. The frame adds a margin of 8 on each side for the shadow.

        private const double ShadowMargin = 16;

        private static double OpenFrameWidth(string caption)
        {
            var box = new DragValue { Caption = caption, ResetCommand = new RelayCommand(() => { }) };
            new Window { Content = box, Width = 800, Height = 300, FontSize = 12.5 }.Show();
            Dispatcher.UIThread.RunJobs();

            box.ContextMenu!.Open(box);
            Dispatcher.UIThread.RunJobs();

            return box.ContextMenu.GetVisualDescendants().OfType<PopupFrame>().Single().Bounds.Width;
        }

        [AvaloniaFact]
        public void Open_AShortMenu_HasTheLeastWidth()
        {
            Assert.Equal(Tokens.Get<double>("PopupMinWidth") + ShadowMargin, OpenFrameWidth("X"));
        }

        [AvaloniaFact]
        public void Open_ALongRow_MakesTheMenuWider()
        {
            Assert.True(OpenFrameWidth(new string('W', 60)) > Tokens.Get<double>("PopupMinWidth") + ShadowMargin);
        }
    }
}
