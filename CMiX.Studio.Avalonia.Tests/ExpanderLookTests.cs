using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Color = Avalonia.Media.Color;

namespace CMiX.Studio.Avalonia.Tests
{
    // The panels look the same everywhere: the expanders of the assets are the expanders of the entities, a panel inside a panel is
    // one step brighter, and the blend of a filter is closed when the filter appears.
    public class ExpanderLookTests
    {
        private static Color Header(Expander expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "mainBorder").Background!).Color;

        private static double Brightness(Color color) => color.R + color.G + color.B;

        private static ExpanderIndented ExpanderWith(Control view, string header) =>
            view.GetVisualDescendants().OfType<ExpanderIndented>().First(expander => Equals(expander.Header, header));

        [AvaloniaFact]
        public void Show_TheAssetPanel_HasTheOpenExpandersOfTheOtherPanels()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.RightPanel = CMiX.Studio.Avalonia.ViewModels.PanelChoice.Assets;
            TestServiceProviderFactory.Pump();
            var assets = window.GetVisualDescendants().OfType<AssetManager>().Single();

            var expanders = assets.GetVisualDescendants().OfType<Expander>().ToList();

            Assert.Equal(new[] { "Images", "Videos", "Geometries", "Image Sequences" }, expanders.Select(expander => expander.Header));
            Assert.All(expanders, expander => Assert.IsType<ExpanderIndented>(expander));
            Assert.All(expanders, expander => Assert.True(expander.IsExpanded));
        }

        [AvaloniaFact]
        public void Show_TheAssetExpanders_HaveTheLookOfTheEntityExpanders()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var provider = TestServiceProviderFactory.Create();
            var entity = (Entity)provider.GetRequiredService<ControlFactory>().Create(typeof(Entity));
            var entityView = new ModelEntity { DataContext = entity };
            new Window { Content = entityView, Width = 500, Height = 1600 }.Show();
            Dispatcher.UIThread.RunJobs();
            var transform = ExpanderWith(entityView, "Transform");

            viewModel.RightPanel = CMiX.Studio.Avalonia.ViewModels.PanelChoice.Assets;
            TestServiceProviderFactory.Pump();
            var images = ExpanderWith(window.GetVisualDescendants().OfType<AssetManager>().Single(), "Images");

            Assert.Same(transform.Theme, images.Theme);
            Assert.Equal(Header(transform), Header(images));
            Assert.Equal(transform.GetVisualDescendants().OfType<ToggleButton>().First().Bounds.Height, images.GetVisualDescendants().OfType<ToggleButton>().First().Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_APanelInsideAPanel_IsOneStepBrighterThanItsParent()
        {
            var provider = TestServiceProviderFactory.Create();
            var entity = (Entity)provider.GetRequiredService<ControlFactory>().Create(typeof(Entity));
            var view = new ModelEntity { DataContext = entity };
            new Window { Content = view, Width = 500, Height = 2000 }.Show();
            Dispatcher.UIThread.RunJobs();

            var geometry = Header(ExpanderWith(view, "Geometry"));
            var fx = Header(ExpanderWith(view, "Geometry FX"));
            var displacement = Header(ExpanderWith(view, "Displacement"));

            Assert.True(Brightness(fx) > Brightness(geometry), "Geometry FX is not brighter than Geometry.");
            Assert.True(Brightness(displacement) > Brightness(geometry), "Displacement is not brighter than Geometry.");
            Assert.Equal(fx, displacement);
        }

        [AvaloniaFact]
        public void Show_AFilter_OpensEveryPanelTheBlendToo()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (CMiX.Core.Texturing.Filters.Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Filters.Blur));
            var view = new CMiX.Studio.Avalonia.Views.Blur { DataContext = blur };
            new Window { Content = view, Width = 500, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.All(view.GetVisualDescendants().OfType<ExpanderIndented>(), expander => Assert.True(expander.IsExpanded, "Closed panel: " + expander.Header));
        }

        [AvaloniaFact]
        public void Select_EveryKindOfNode_ShowsOnlyOpenExpandersInTheInspector()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(CMiX.Core.Compositing.Composition));
            var composition = (CMiX.Core.Compositing.Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(CMiX.Core.Compositing.Layer));
            var layer = (CMiX.Core.Compositing.Layer)composition.LayerManager.ManagerData.Items[0];
            layer.ModelEntityManager.AddItem(typeof(CMiX.Core.Compositing.Entity));
            layer.ModelEntityManager.AddItem(typeof(CMiX.Core.Compositing.FullscreenQuad));
            layer.ModelEntityManager.AddItem(typeof(CMiX.Core.Compositing.TextEntity));
            layer.CameraManager.AddItem(typeof(CMiX.Core.Rendering.Cameras.Camera));
            layer.LightManager.AddItem(typeof(CMiX.Core.Rendering.Lights.LightEntity));
            layer.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var inspector = window.GetVisualDescendants().OfType<CMiX.Studio.Avalonia.Views.PanelInspector>().Single();

            foreach (var node in viewModel.ProjectTree.VisibleNodes().ToList())
            {
                viewModel.ProjectTree.SelectedNode = node;
                TestServiceProviderFactory.Pump();

                var closed = inspector.GetVisualDescendants().OfType<Expander>().Where(expander => expander.IsEffectivelyVisible && !expander.IsExpanded).ToList();
                Assert.True(closed.Count == 0, node.Header + " has a closed expander.");
            }
        }
    }
}
