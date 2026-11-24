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

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The main window with the inspector tab, for the inspector tests.
    internal sealed class PanelInspectorScreen
    {
        public MainWindow Window { get; init; } = null!;
        public MainViewModel ViewModel { get; init; } = null!;
        public PanelInspector Inspector { get; init; } = null!;
        public Layer Layer { get; init; } = null!;
        public UndoManager UndoManager { get; init; } = null!;

        public ProjectTree Tree => ViewModel.ProjectTree;
        public TreeNode CompositionNode => Tree.CompositionNodes()[0];
        public TreeNode LayerNode => CompositionNode.Children[0].Children[0];
        public TreeNode Group<T>() where T : TreeGroup => LayerNode.Children.Single(group => group.Group is T);

        // Found by its name: while no item is selected the scroll viewer is hidden and has no visual children yet.
        public ContentControl Editor => Inspector.FindControl<ContentControl>("editor")!;
        public ScrollViewer Scroller => Inspector.FindControl<ScrollViewer>("scroller")!;
        // The name of an item is in its name editor (textDisplay), the name of a group is in a text block called title.
        public TextBlock Title => Header.GetVisualDescendants().OfType<TextBlock>().First(t => t.Name is "title" or "textDisplay" && t.IsEffectivelyVisible);
        public ToggleButton? VisibilityToggle => Inspector.GetVisualDescendants().OfType<ToggleButton>().FirstOrDefault(t => t.Name == "visibilityToggle");
        public ToggleButton? MaskToggle => Inspector.GetVisualDescendants().OfType<ToggleButton>().FirstOrDefault(t => t.Name == "maskToggle");
        public TextBoxEditable NameEditor => Inspector.FindControl<TextBoxEditable>("nameEditor")!;
        public TextBlock Placeholder => Inspector.GetVisualDescendants().OfType<TextBlock>().First(t => t.Name == "placeholder");
        public Border Header => Inspector.GetVisualDescendants().OfType<Border>().First(b => b.Name == "header");
        public AppIcon HeaderIcon => Header.GetVisualDescendants().OfType<AppIcon>().First();

        public T? Find<T>() where T : class => Editor.GetVisualDescendants().OfType<T>().FirstOrDefault();

        // A composition with a layer. The layer has one of each kind of item. The inspector tab is shown.
        public static PanelInspectorScreen Show()
        {
            var main = ListTreeMainScreen.Show(layers: 1, fill: composition =>
            {
                var first = (Layer)composition.LayerManager.ManagerData.Items[0];
                first.ModelEntityManager.AddItem(typeof(Entity));
                first.ModelEntityManager.AddItem(typeof(FullscreenQuad));
                first.ModelEntityManager.AddItem(typeof(TextEntity));
                first.CameraManager.AddItem(typeof(CameraModel));
                first.LightManager.AddItem(typeof(LightModel));
            });
            var (window, viewModel, layer) = (main.Window, main.ViewModel, main.Layer(0));
            viewModel.ProjectTree.SelectedNode = null;
            TestServiceProviderFactory.SelectMainTab(window, "Inspector");
            var inspector = window.GetVisualDescendants().OfType<PanelInspector>().Single();
            TestServiceProviderFactory.Pump();
            return new PanelInspectorScreen { Window = window, ViewModel = viewModel, Inspector = inspector, Layer = layer, UndoManager = main.Undo };
        }

        public void Select(TreeNode node)
        {
            Tree.SelectedNode = node;
            Tree.Inspect(node);
            TestServiceProviderFactory.Pump();
        }
    }
}
