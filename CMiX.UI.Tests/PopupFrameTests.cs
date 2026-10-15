using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Tests
{
    // The PopupFrame theme gives the frame its template.
    public class PopupFrameTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void PopupFrame_InAWindow_PresentsItsContentInTheThemedTemplate()
        {
            var text = new TextBlock { Text = "Item" };
            var frame = new PopupFrame { Content = text };
            Host(frame);
            frame.ApplyTemplate();

            var root = Assert.IsType<Border>(frame.GetVisualChildren().Single());
            Assert.NotNull(root.Effect);
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
                Theme = (ControlTheme)Application.Current!.FindResource("FlyoutPresenterPanel")!,
                Content = text
            };
            Host(presenter);

            var frame = presenter.GetVisualDescendants().OfType<PopupFrame>().Single();
            Assert.Equal(300, presenter.Width);
            Assert.Contains(text, frame.GetVisualDescendants());
        }

        [AvaloniaFact]
        public void PopupFlyoutTheme_CentersTheFlyoutOnThePointer_WhateverItsWidth()
        {
            var presenter = new FlyoutPresenter
            {
                Theme = (ControlTheme)Application.Current!.FindResource("FlyoutPresenterPanel")!,
                Content = new TextBlock { Text = "Item" },
                Width = 420
            };
            Host(presenter);

            var frame = presenter.GetVisualDescendants().OfType<PopupFrame>().Single();
            Assert.True(frame.CentersOnPointer);
            Assert.Equal(-frame.Bounds.Width / 2, frame.PointerOffset(frame.Bounds.Size).X);
            Assert.Equal(-frame.Bounds.Height / 2, frame.PointerOffset(frame.Bounds.Size).Y);
        }
    }
}
