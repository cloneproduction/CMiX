// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using CMiX.UI.Panels;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CameraModel = CMiX.Core.Rendering.Cameras.Camera;
using CameraView = CMiX.Studio.Avalonia.Views.Camera;
using Composition = CMiX.Core.Compositing.Composition;
using CompositionView = CMiX.Studio.Avalonia.Views.Composition;
using FullscreenQuad = CMiX.Core.Compositing.FullscreenQuad;
using FullscreenQuadView = CMiX.Studio.Avalonia.Views.FullscreenQuad;
using Layer = CMiX.Core.Compositing.Layer;
using LightModel = CMiX.Core.Rendering.Lights.LightEntity;
using LightView = CMiX.Studio.Avalonia.Views.LightEntity;
using TextEntity = CMiX.Core.Compositing.TextEntity;
using TextEntityView = CMiX.Studio.Avalonia.Views.TextEntity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The inspector is the first tab. A click in the tree keeps the tab that shows, and the inspector scrolls on its own.
    public class PanelInspectorTabTests
    {
        [AvaloniaFact]
        public void Show_TheTabs_PutTheInspectorFirstAndKeepTheOtherTabs()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var keys = TestServiceProviderFactory.MainTabControl(window).Items.OfType<TabItem>()
                .Select(item => (item.Header as AppIcon)?.IconKey).ToList();

            Assert.Equal("Inspector", keys[0]);
            Assert.Equal("Settings", keys[1]);
            Assert.Contains("Composition", keys);
            Assert.Contains("LayerScene", keys);
            Assert.Contains("Geometry", keys);
        }

        [AvaloniaFact]
        public void Show_EveryTab_HasATooltipOnItsIcon()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tabs = TestServiceProviderFactory.MainTabControl(window).Items.OfType<TabItem>().ToList();

            Assert.NotEmpty(tabs);
            Assert.All(tabs, tab =>
            {
                var header = Assert.IsAssignableFrom<Control>(tab.Header);
                var tip = ToolTip.GetTip(header) ?? ToolTip.GetTip(tab);
                Assert.False(string.IsNullOrWhiteSpace(tip as string), $"The tab {(header as AppIcon)?.IconKey} has no tooltip.");
            });
        }

        [AvaloniaFact]
        public void Show_AfterAnotherTab_ShowsTheCurrentSelection()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.LayerNode);

            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");
            screen.Select(screen.Group<CamerasGroup>().Children[0]);
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Inspector");

            var inspector = screen.Window.GetVisualDescendants().OfType<PanelInspector>().Single();
            Assert.NotNull(inspector.GetVisualDescendants().OfType<CameraView>().FirstOrDefault());
            Assert.Null(inspector.GetVisualDescendants().OfType<TabLayerPostFX>().FirstOrDefault());
        }

        private static Point CenterOfRow(PanelInspectorScreen screen, TreeNode node)
        {
            var tree = screen.Window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            var row = tree.Rows.RowOf(node)!;
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), screen.Window)!.Value;
        }


        private static string SelectedTab(PanelInspectorScreen screen) =>
            (TestServiceProviderFactory.MainTabControl(screen.Window).SelectedItem as TabItem is { Header: AppIcon icon }) ? icon.IconKey : "";

        [AvaloniaFact]
        public void MiddleClick_ARow_KeepsTheTabAndTheInspectorHoldsThatItem()
        {
            var screen = PanelInspectorScreen.Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            screen.Window.Click(CenterOfRow(screen, screen.Group<CamerasGroup>().Children[0]), MouseButton.Middle);

            Assert.Equal("Settings", SelectedTab(screen));
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Inspector");
            Assert.NotNull(screen.Find<CameraView>());
        }

        [AvaloniaFact]
        public void MiddleClick_AGroup_KeepsTheTabAndTheInspectorHoldsTheGroup()
        {
            var screen = PanelInspectorScreen.Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            screen.Window.Click(CenterOfRow(screen, screen.Group<LightsGroup>()), MouseButton.Middle);

            Assert.Equal("Settings", SelectedTab(screen));
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Inspector");
            Assert.Equal("Lights", screen.Title.Text);
        }

        [AvaloniaFact]
        public void Click_TheVisibilityButton_KeepsTheTab()
        {
            var screen = PanelInspectorScreen.Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");
            var tree = screen.Window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var toggle = tree.Rows.RowOf(screen.LayerNode)!.Toggles
                .GetVisualDescendants().OfType<ToggleButton>().First(button => CMiX.UI.AttachedProperties.ToggleDrag.GetGroup(button) == "visibility");
            var point = toggle.TranslatePoint(new Point(toggle.Bounds.Width / 2, toggle.Bounds.Height / 2), screen.Window)!.Value;

            screen.Window.Click(point);

            Assert.Equal("Settings", SelectedTab(screen));
        }

        [AvaloniaFact]
        public void Select_FromAProgramOrAList_KeepsTheTab()
        {
            var screen = PanelInspectorScreen.Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            screen.Select(screen.LayerNode);
            Assert.Equal("Settings", SelectedTab(screen));

            screen.Layer.CameraManager.AddItem(typeof(CameraModel));
            TestServiceProviderFactory.Pump();
            screen.Layer.CameraManager.SelectAt(0);
            TestServiceProviderFactory.Pump();

            Assert.Equal("Settings", SelectedTab(screen));
        }

        [AvaloniaFact]
        public void Click_OnARow_KeepsTheTab()
        {
            var screen = PanelInspectorScreen.Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            screen.Window.Click(CenterOfRow(screen, screen.LayerNode));

            Assert.Equal("Settings", SelectedTab(screen));
            Assert.Same(screen.Layer, screen.Tree.SelectedNode!.Item);
        }

        // The layer view is also a tab with its own scroll viewer. In the inspector that viewer is off, so only the inspector scrolls.
        [AvaloniaFact]
        public void HostedView_Show_DoesNotScrollOnItsOwn()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.LayerNode);
            screen.Window.Height = 300;
            TestServiceProviderFactory.Pump();
            screen.Window.UpdateLayout();

            var view = screen.Find<TabLayerPostFX>()!;
            var inner = (ScrollViewer)view.Content!;
            var innerPresenter = inner.GetVisualDescendants().OfType<global::Avalonia.Controls.Presenters.ScrollContentPresenter>().First(p => p.TemplatedParent == inner);

            Assert.Contains("hosted", view.Classes);
            Assert.Equal(ScrollBarVisibility.Disabled, inner.VerticalScrollBarVisibility);
            Assert.Equal(inner.Viewport.Height, inner.Extent.Height, 1);
            Assert.Equal(inner.Bounds.Width, innerPresenter.Bounds.Width, 1);

            Assert.True(screen.Scroller.Extent.Height > screen.Scroller.Viewport.Height, "The inspector has nothing to scroll, so the test proves nothing.");
            screen.Scroller.Offset = new Vector(0, 50);
            TestServiceProviderFactory.Pump();
            Assert.Equal(50, screen.Scroller.Offset.Y, 1);
        }

        // Each change of the kind of the selected item builds one editor view, also on a large composition.
        [AvaloniaFact]
        public void SelectedNode_OnALargeComposition_BuildsOneViewForEachSelection()
        {
            const int layers = 20;
            const int entitiesPerLayer = 100;
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < layers; i++)
            {
                composition.LayerManager.AddItem(typeof(Layer));
                var layer = (Layer)composition.LayerManager.ManagerData.Items[i];
                for (var j = 0; j < entitiesPerLayer; j++)
                    layer.ModelEntityManager.AddItem(typeof(Entity));
            }

            var tree = viewModel.ProjectTree;
            tree.SelectedNode = null;
            TestServiceProviderFactory.SelectMainTab(window, "Inspector");
            var inspector = window.GetVisualDescendants().OfType<PanelInspector>().Single();
            TestServiceProviderFactory.Pump();

            var compositionNode = tree.CompositionNodes()[0];
            TreeNode EntityOf(int layer, int index) =>
                compositionNode.Children[0].Children[layer].Children.Single(child => child.Group is EntitiesGroup).Children[index];
            var steps = new (string Name, TreeNode Node)[]
            {
                ("entity", EntityOf(0, 0)),
                ("layer", compositionNode.Children[0].Children[layers / 2]),
                ("composition", compositionNode),
                ("entity again", EntityOf(layers - 1, entitiesPerLayer - 1)),
            };

            foreach (var step in steps)
            {
                var before = inspector.EditorViewsBuilt;
                tree.SelectedNode = step.Node;
                TestServiceProviderFactory.Pump();
                window.UpdateLayout();
                TestServiceProviderFactory.Pump();

                Assert.True(inspector.EditorViewsBuilt - before <= 1, $"The {step.Name} builds more than one view.");
            }
        }
    }
}
