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
    // The inspector shows the editor of the item that is selected in the project tree, and follows what happens to the item.
    public class PanelInspectorSelectionTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        [AvaloniaFact]
        public void Show_NothingSelected_ShowsOnlyTheHint()
        {
            var screen = PanelInspectorScreen.Show(windows);
            Assert.True(screen.Placeholder.IsEffectivelyVisible);
            Assert.False(screen.Header.IsVisible);
            Assert.False(screen.Scroller.IsVisible);
        }

        [AvaloniaFact]
        public void Select_AComposition_ShowsItsSettingsAndItsEditor()
        {
            var screen = PanelInspectorScreen.Show(windows);

            screen.Select(screen.CompositionNode);

            Assert.False(screen.Placeholder.IsVisible);
            Assert.Same(screen.CompositionNode.Item, screen.Editor.Content);
            Assert.NotNull(screen.Find<CompositionView>());
            Assert.Same(screen.CompositionNode.Item, screen.Find<SettingsComposable>()!.DataContext);
            Assert.Equal("Composition", screen.HeaderIcon.IconKey);
            Assert.Equal(screen.CompositionNode.Header, screen.Title.Text);
        }

        private static Entity EntityOf(PanelInspectorScreen screen) => (Entity)screen.Layer.ModelEntityManager.ManagerData.Items[0];

        [AvaloniaFact]
        public void Select_TheModifiersGroupOfAnEntity_ShowsTheModifiersOfThatEntity()
        {
            var screen = PanelInspectorScreen.Show(windows);
            var entityNode = screen.Group<EntitiesGroup>().Children[0];

            screen.Select(entityNode.Children.Single(group => group.Group is ModifiersGroup));

            var panel = Assert.IsType<ModifiersGroup>(screen.Editor.Content);
            Assert.Same(EntityOf(screen), panel.Owner);
            var manager = screen.Find<ModifierManager>()!;
            Assert.Same(EntityOf(screen).ModifierManager, manager.DataContext);
            Assert.Equal("Add Modifier", manager.Caption);

            // The panel that adds an item lists what can be added to an entity.
            var selection = (PanelAddTypes)manager.AddContent!;
            Assert.Equal(typeof(Entity), selection.PanelOwner);
        }

        [AvaloniaFact]
        public void Select_TheFiltersGroupOfALayerOrAComposition_ShowsTheListOfFilters()
        {
            var screen = PanelInspectorScreen.Show(windows);

            screen.Select(screen.LayerNode.Children.Single(group => group.Group is FiltersGroup));
            var layerPanel = screen.Find<ModifierManager>()!;
            Assert.Same(screen.Layer.TextureModifierManager, layerPanel.DataContext);
            Assert.Equal("Add Filter", layerPanel.Caption);
            Assert.Equal(typeof(CMiX.Core.Texturing.Filters.ITextureFilter), ((PanelAddTypes)layerPanel.AddContent!).Interface);

            screen.Select(screen.CompositionNode.Children.Single(group => group.Group is FiltersGroup));
            Assert.Same(((Composition)screen.CompositionNode.Item!).TextureModifierManager, screen.Find<ModifierManager>()!.DataContext);
        }

        [AvaloniaFact]
        public void Select_TheModifiersGroupOfALayerOrAComposition_ListsWhatCanBeAdded()
        {
            var screen = PanelInspectorScreen.Show(windows);

            screen.Select(screen.LayerNode.Children.Single(group => group.Group is ModifiersGroup));
            Assert.Equal(typeof(Layer), ((PanelAddTypes)screen.Find<ModifierManager>()!.AddContent!).PanelOwner);

            screen.Select(screen.CompositionNode.Children.Single(group => group.Group is ModifiersGroup));
            Assert.Equal(typeof(Composition), ((PanelAddTypes)screen.Find<ModifierManager>()!.AddContent!).PanelOwner);
        }

        [AvaloniaFact]
        public void Select_TheTexturesGroupOfAnEntity_ShowsItsDiffuseTexture()
        {
            var screen = PanelInspectorScreen.Show(windows);
            var entityNode = screen.Group<EntitiesGroup>().Children[0];

            screen.Select(entityNode.Children.Single(group => group.Group is TexturesGroup));

            Assert.IsType<TexturesGroup>(screen.Editor.Content);
            Assert.Same(EntityOf(screen).Texture.DiffuseTexture, screen.Find<DiffuseTexture>()!.DataContext);
        }

        [AvaloniaFact]
        public void Select_AModifierRow_ShowsTheEditorOfThatModifier()
        {
            var screen = PanelInspectorScreen.Show(windows);
            EntityOf(screen).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            TestServiceProviderFactory.Pump();
            var node = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is ModifiersGroup).Children[0];

            screen.Select(node);

            Assert.Same(node.Item, screen.Editor.Content);
            Assert.Equal("Modifier", screen.HeaderIcon.IconKey);
            Assert.Equal(node.Header, screen.Title.Text);
            var editor = screen.Editor.GetVisualDescendants().OfType<Control>().First(control => control.GetType().Name == "TranslateModifier");
            Assert.Same(node.Item, editor.DataContext);
        }

        [AvaloniaFact]
        public void Select_AModifierRow_ShowsAndAddsTheModulatorsOfThatModifier()
        {
            var screen = PanelInspectorScreen.Show(windows);
            EntityOf(screen).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            var modifier = (CMiX.Core.Modulation.IModulatable)EntityOf(screen).ModifierManager.ManagerData.Items[0];
            modifier.ModulatorManager.AddItem(typeof(CMiX.Core.Modulation.Modulators.LFOModulator));
            TestServiceProviderFactory.Pump();
            var node = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is ModifiersGroup).Children[0];

            screen.Select(node);

            var list = screen.Editor.GetVisualDescendants().OfType<ModifierManager>().Single(manager => manager.Caption == "Add Modulator");
            Assert.Same(modifier.ModulatorManager, list.DataContext);
            Assert.Equal(typeof(CMiX.Core.Modulation.Modulators.IModulator), ((PanelAddTypes)list.AddContent!).Interface);
            Assert.NotNull(screen.Editor.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.GetType().Name == "LFOModulator"));
        }

        [AvaloniaFact]
        public void Select_AFilterRow_ShowsTheEditorOfThatFilter()
        {
            var screen = PanelInspectorScreen.Show(windows);
            screen.Layer.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
            TestServiceProviderFactory.Pump();
            var node = screen.LayerNode.Children.Single(group => group.Group is FiltersGroup).Children[0];

            screen.Select(node);

            Assert.Same(node.Item, screen.Editor.Content);
            Assert.Equal("Filter", screen.HeaderIcon.IconKey);
            Assert.NotNull(screen.Editor.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.GetType().Name == "Blur"));
        }

        [AvaloniaFact]
        public void Select_ATextureRow_ShowsTheEditorOfThatTexture()
        {
            var screen = PanelInspectorScreen.Show(windows);
            EntityOf(screen).Texture.DiffuseTexture.TextureManager.AddItem(typeof(CMiX.Core.Texturing.Sources.Image));
            TestServiceProviderFactory.Pump();
            var node = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is TexturesGroup).Children[0];

            screen.Select(node);

            Assert.Same(node.Item, screen.Editor.Content);
            Assert.Equal("Texture", screen.HeaderIcon.IconKey);
            Assert.NotNull(screen.Editor.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.GetType().Name == "Image"));
        }

        [AvaloniaFact]
        public void Select_AModifierInTheInspectorList_SelectsItInTheTree()
        {
            var screen = PanelInspectorScreen.Show(windows);
            EntityOf(screen).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            EntityOf(screen).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.ScaleModifier));
            TestServiceProviderFactory.Pump();
            var modifiers = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is ModifiersGroup);

            EntityOf(screen).ModifierManager.SelectAt(0);
            TestServiceProviderFactory.Pump();

            Assert.Same(modifiers.Children[0], screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Select_ALayer_ShowsItsSettingsAndEffectsButNotItsItemLists()
        {
            var screen = PanelInspectorScreen.Show(windows);

            screen.Select(screen.LayerNode);

            Assert.Same(screen.Layer, screen.Editor.Content);
            Assert.Same(screen.Layer, screen.Find<SettingsComposable>()!.DataContext);
            Assert.NotNull(screen.Find<TabLayerPostFX>());
            Assert.Null(screen.Find<TabLayerEntity>());
            Assert.Null(screen.Find<TabLayerCamera>());
            Assert.Null(screen.Find<TabLayerLight>());
            Assert.Equal("LayerScene", screen.HeaderIcon.IconKey);
        }

        [AvaloniaTheory]
        [InlineData(0, typeof(ModelEntity))]
        [InlineData(1, typeof(FullscreenQuadView))]
        [InlineData(2, typeof(TextEntityView))]
        public void Select_AnEntity_ShowsTheEditorOfItsKind(int index, Type editor)
        {
            var screen = PanelInspectorScreen.Show(windows);
            var node = screen.Group<EntitiesGroup>().Children[index];

            screen.Select(node);

            Assert.Same(node.Item, screen.Editor.Content);
            var view = screen.Editor.GetVisualDescendants().OfType<Control>().First(visual => visual.GetType() == editor);
            Assert.Same(node.Item, view.DataContext);
        }

        [AvaloniaFact]
        public void Select_ACameraOrALight_ShowsItsEditor()
        {
            var screen = PanelInspectorScreen.Show(windows);

            screen.Select(screen.Group<CamerasGroup>().Children[0]);
            Assert.NotNull(screen.Find<CameraView>());
            Assert.Equal("Camera", screen.HeaderIcon.IconKey);

            screen.Select(screen.Group<LightsGroup>().Children[0]);
            Assert.NotNull(screen.Find<LightView>());
            Assert.Null(screen.Find<CameraView>());
        }

        [AvaloniaFact]
        public void Select_AGroup_ShowsItsNameAndTheListPanelOfItsKind()
        {
            var screen = PanelInspectorScreen.Show(windows);

            screen.Select(screen.Group<CamerasGroup>());
            Assert.True(screen.Header.IsVisible);
            Assert.Equal("Cameras", screen.Title.Text);
            Assert.False(screen.Placeholder.IsVisible);
            Assert.True(screen.Scroller.IsVisible);
            Assert.Same(screen.Layer, Assert.IsType<CamerasGroup>(screen.Editor.Content).Layer);
            Assert.Same(screen.Layer, screen.Find<TabLayerCamera>()!.DataContext);
            Assert.Null(screen.Find<TabLayerEntity>());

            screen.Select(screen.Group<EntitiesGroup>());
            Assert.Equal("Entities", screen.Title.Text);
            Assert.Same(screen.Layer, screen.Find<TabLayerEntity>()!.DataContext);
            Assert.Null(screen.Find<TabLayerCamera>());

            screen.Select(screen.Group<LightsGroup>());
            Assert.Equal("Lights", screen.Title.Text);
            Assert.Same(screen.Layer, screen.Find<TabLayerLight>()!.DataContext);
            Assert.Null(screen.Find<TabLayerEntity>());
        }

        [AvaloniaFact]
        public void Select_AnotherItemOrRename_UpdatesTheInspector()
        {
            var screen = PanelInspectorScreen.Show(windows);
            var entity = screen.Group<EntitiesGroup>().Children[0];

            screen.Select(screen.LayerNode);
            Assert.NotNull(screen.Find<TabLayerPostFX>());

            screen.Select(entity);
            Assert.Null(screen.Find<TabLayerPostFX>());
            Assert.NotNull(screen.Find<ModelEntity>());

            ((IPrefab)entity.Item!).PrefabService.Name.Value = "Floor";
            TestServiceProviderFactory.Pump();
            Assert.Equal("Floor", screen.Title.Text);
        }

        [AvaloniaFact]
        public void Remove_TheSelectedItem_ClosesTheEditor()
        {
            var screen = PanelInspectorScreen.Show(windows);
            // The only camera: when it goes, the selection falls back to the group of cameras.
            var camera = screen.Group<CamerasGroup>().Children[0];
            screen.Select(camera);
            Assert.NotNull(screen.Find<CameraView>());

            screen.Layer.CameraManager.RemoveItem((IControl)camera.Item!);
            TestServiceProviderFactory.Pump();

            // The selection falls back to the group of cameras, which shows the panel of the list.
            Assert.Equal("Cameras", screen.Title.Text);
            Assert.IsType<CamerasGroup>(screen.Editor.Content);
            Assert.NotNull(screen.Find<TabLayerCamera>());
        }

        [AvaloniaFact]
        public void Select_AnItemInAList_SelectsItsRowAndLeavesTheInspector()
        {
            var screen = PanelInspectorScreen.Show(windows);
            // Selecting the first camera in its list moves the selection of the tree and not the inspector.
            screen.Layer.CameraManager.AddItem(typeof(CameraModel));
            TestServiceProviderFactory.Pump();
            var camera = screen.Group<CamerasGroup>().Children[0];

            screen.Layer.CameraManager.SelectAt(0);
            TestServiceProviderFactory.Pump();

            Assert.Same(camera, screen.Tree.SelectedNode);
            Assert.Null(screen.Tree.InspectedNode);
        }

        // Every panel of an editor is open when the item is new.
        [AvaloniaTheory]
        [InlineData("composition")]
        [InlineData("layer")]
        [InlineData("entity")]
        [InlineData("quad")]
        [InlineData("text")]
        [InlineData("camera")]
        [InlineData("light")]
        [InlineData("entities group")]
        [InlineData("cameras group")]
        [InlineData("lights group")]
        public void Show_AnEditor_OpensEveryPanel(string kind)
        {
            var screen = PanelInspectorScreen.Show(windows);
            var node = kind switch
            {
                "composition" => screen.CompositionNode,
                "layer" => screen.LayerNode,
                "entity" => screen.Group<EntitiesGroup>().Children[0],
                "quad" => screen.Group<EntitiesGroup>().Children[1],
                "text" => screen.Group<EntitiesGroup>().Children[2],
                "camera" => screen.Group<CamerasGroup>().Children[0],
                "entities group" => screen.Group<EntitiesGroup>(),
                "cameras group" => screen.Group<CamerasGroup>(),
                "lights group" => screen.Group<LightsGroup>(),
                _ => screen.Group<LightsGroup>().Children[0]
            };

            screen.Select(node);

            var expanders = screen.Editor.GetVisualDescendants().OfType<Expander>().ToList();
            Assert.NotEmpty(expanders);
            Assert.All(expanders, expander => Assert.True(expander.IsExpanded, "Closed panel: " + expander.Header));
        }

        [AvaloniaFact]
        public async Task Move_AnEntityToAnotherLayer_KeepsItInTheInspector()
        {
            var screen = PanelInspectorScreen.Show(windows);
            var composition = (Composition)screen.CompositionNode.Item!;
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            var entity = screen.Group<EntitiesGroup>().Children[0];
            var item = entity.Item;
            screen.Select(entity);
            var otherEntities = screen.CompositionNode.Children[0].Children[1].Children.Single(group => group.Group is EntitiesGroup);

            await screen.Tree.DropAsync(entity, otherEntities, DropPosition.Inside);
            TestServiceProviderFactory.Pump();

            Assert.Same(item, screen.Editor.Content);
            Assert.NotNull(screen.Find<ModelEntity>());
            Assert.Same(item, screen.Tree.SelectedNode!.Item);
        }

        [AvaloniaFact]
        public void Remove_TheCompositionOfTheSelectedLayer_ShowsTheHint()
        {
            var screen = PanelInspectorScreen.Show(windows);
            screen.Select(screen.LayerNode);
            Assert.NotNull(screen.Find<TabLayerPostFX>());

            screen.ViewModel.Project.CompositionManager.RemoveItem((IControl)screen.CompositionNode.Item!);
            TestServiceProviderFactory.Pump();

            Assert.Null(screen.Tree.SelectedNode);
            Assert.True(screen.Placeholder.IsEffectivelyVisible);
            Assert.False(screen.Header.IsVisible);
            Assert.False(screen.Scroller.IsVisible);
        }

        [AvaloniaFact]
        public void UndoAndRedo_OfARemoval_KeepTheInspectorWorking()
        {
            var screen = PanelInspectorScreen.Show(windows);
            var camera = screen.Group<CamerasGroup>().Children[0];
            screen.Select(camera);

            screen.Layer.CameraManager.RemoveItem((IControl)camera.Item!);
            TestServiceProviderFactory.Pump();
            Assert.Equal("Cameras", screen.Title.Text);

            screen.UndoManager.Undo();
            TestServiceProviderFactory.Pump();
            screen.UndoManager.Redo();
            TestServiceProviderFactory.Pump();

            Assert.Equal("Cameras", screen.Title.Text);
            Assert.IsType<CamerasGroup>(screen.Editor.Content);
        }
    }
}
