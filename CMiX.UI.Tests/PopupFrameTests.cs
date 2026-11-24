using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;

namespace CMiX.UI.Tests
{
    // The PopupFrame theme gives the frame its template.
    public class PopupFrameTests
    {
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
    }
}
