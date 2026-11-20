using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using CompositionView = CMiX.Studio.Avalonia.Views.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Every scrolling panel shows the overlay bar of the tree and the lists. The bar lies in the gutter on the right of the content.
    public class ScrollBarLookTests
    {
        public static TheoryData<string> Panels => new() { "Inspector", "Settings", "Entities", "Composition", "LayerPostFX", "LayerLight" };

        // The panel in a window that is lower than its content, and the scroll viewer of the panel.
        private static (Window Window, ScrollViewer Viewer) ShowPanel(string name)
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            mainWindow.Close();

            UserControl panel = name switch
            {
                "Inspector" => new Inspector { DataContext = viewModel.ProjectTree },
                "Settings" => new TabSettings { DataContext = viewModel },
                "Entities" => new TabEntities { DataContext = viewModel },
                "Composition" => new CompositionView { DataContext = composition },
                "LayerPostFX" => new LayerPostFXTab { DataContext = layer },
                "LayerLight" => new LayerLightTab { DataContext = layer },
                _ => throw new ArgumentOutOfRangeException(nameof(name)),
            };
            if (panel is Inspector)
                viewModel.ProjectTree.Inspect(viewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0]);

            var window = new Window { Content = panel, Width = 300, Height = 60 };
            window.Show();
            TestServiceProviderFactory.Pump();
            window.UpdateLayout();

            var viewer = panel is Inspector inspector ? inspector.FindControl<ScrollViewer>("scroller")! : (ScrollViewer)panel.Content!;
            return (window, viewer);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Panels))]
        public void Panel_ContentHigherThanTheView_ShowsTheOverlayBarInTheGutter(string name)
        {
            var (window, viewer) = ShowPanel(name);

            var bar = viewer.GetVisualDescendants().OfType<ScrollBar>()
                .First(b => b.TemplatedParent == viewer && b.Orientation == Orientation.Vertical);
            var presenter = viewer.GetVisualDescendants().OfType<ScrollContentPresenter>().First(p => p.TemplatedParent == viewer);
            var gutter = Assert.IsType<Thickness>(Application.Current!.FindResource("ScrollBarGutter"));
            var barBox = new Rect(bar.TranslatePoint(new Point(0, 0), window)!.Value, bar.Bounds.Size);
            var presenterBox = new Rect(presenter.TranslatePoint(new Point(0, 0), window)!.Value, presenter.Bounds.Size);
            var viewerBox = new Rect(viewer.TranslatePoint(new Point(0, 0), window)!.Value, viewer.Bounds.Size);

            Assert.Same(Application.Current!.FindResource("ScrollViewerOverlayKeepOffset"), viewer.Theme);
            Assert.Same(Application.Current!.FindResource("ScrollViewerOverlay"), viewer.Theme.BasedOn);
            Assert.True(viewer.Extent.Height > viewer.Viewport.Height, "The content is not higher than the view, so the test proves nothing.");
            Assert.True(bar.IsVisible, "The bar is not shown.");
            Assert.True(bar.Bounds.Width > 0);
            Assert.Equal(viewerBox.Width - gutter.Right, presenterBox.Width, 1);
            Assert.True(barBox.Left >= presenterBox.Right - 0.5, "The bar is over the content.");
            Assert.True(barBox.Right <= viewerBox.Right + 0.5, "The bar is outside the scroll viewer.");
            window.Close();
        }
    }
}
