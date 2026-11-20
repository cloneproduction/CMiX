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

namespace CMiX.Studio.Avalonia.Tests
{
    // The inspector shows the editor of the item that is selected in the project tree. The other tabs stay as they were.
    public class PanelInspectorTests
    {
        private sealed class Screen
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
            // The name of an item is in its name editor (TextDisplay), the name of a group is in a text block called title.
            public TextBlock Title => Header.GetVisualDescendants().OfType<TextBlock>().First(t => t.Name is "title" or "TextDisplay" && t.IsEffectivelyVisible);
            public ToggleButton? VisibilityToggle => Inspector.GetVisualDescendants().OfType<ToggleButton>().FirstOrDefault(t => t.Name == "visibilityToggle");
            public ToggleButton? MaskToggle => Inspector.GetVisualDescendants().OfType<ToggleButton>().FirstOrDefault(t => t.Name == "maskToggle");
            public TextBoxEditable NameEditor => Inspector.FindControl<TextBoxEditable>("nameEditor")!;
            public TextBlock Placeholder => Inspector.GetVisualDescendants().OfType<TextBlock>().First(t => t.Name == "placeholder");
            public Border Header => Inspector.GetVisualDescendants().OfType<Border>().First(b => b.Name == "header");
            public AppIcon HeaderIcon => Header.GetVisualDescendants().OfType<AppIcon>().First();

            public T? Find<T>() where T : class => Editor.GetVisualDescendants().OfType<T>().FirstOrDefault();
        }

        // A composition with a layer. The layer has one of each kind of item. The inspector tab is shown.
        private static Screen Show()
        {
            var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            layer.ModelEntityManager.AddItem(typeof(TextEntity));
            layer.CameraManager.AddItem(typeof(CameraModel));
            layer.LightManager.AddItem(typeof(LightModel));
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            TestServiceProviderFactory.SelectMainTab(window, "Inspector");
            var inspector = window.GetVisualDescendants().OfType<PanelInspector>().Single();
            TestServiceProviderFactory.Pump();
            return new Screen { Window = window, ViewModel = viewModel, Inspector = inspector, Layer = layer, UndoManager = provider.GetRequiredService<UndoManager>() };
        }

        private static void Select(Screen screen, TreeNode node)
        {
            screen.Tree.SelectedNode = node;
            screen.Tree.Inspect(node);
            TestServiceProviderFactory.Pump();
        }

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

        // The title bar has the height of the headers of the panels under it (the headers of the expanders).
        [AvaloniaFact]
        public void Show_TheHeader_HasTheHeightOfThePanelHeaders()
        {
            var screen = Show();
            Select(screen, screen.CompositionNode);
            var expander = screen.Editor.GetVisualDescendants().OfType<ExpanderIndented>().First();
            var expanderHeader = expander.GetVisualDescendants().OfType<ToggleButton>().First();

            var height = Convert.ToDouble(Application.Current!.FindResource("ControlHeightXL"));
            Assert.Equal(height, screen.Header.Bounds.Height);
            Assert.Equal(expanderHeader.Bounds.Height, screen.Header.Bounds.Height);

            // The icon and the name are in the middle of the bar.
            var icon = screen.HeaderIcon;
            var iconMiddle = icon.TranslatePoint(new Point(0, icon.Bounds.Height / 2), screen.Header)!.Value.Y;
            Assert.Equal(height / 2, iconMiddle, 1);
        }

        // The whole title bar has the color of the kind of the selected row. The badge has no fill of its own.
        [AvaloniaFact]
        public void Select_AnItem_ColorsTheWholeHeaderWithTheKindColor()
        {
            var screen = Show();
            var palette = CMiX.Studio.Avalonia.Themes.CategoryPalette.FromApplication();
            foreach (var node in new[] { screen.CompositionNode, screen.LayerNode, screen.Group<EntitiesGroup>(), screen.Group<LightsGroup>() })
            {
                Select(screen, node);
                var square = screen.Header.GetVisualDescendants().OfType<BadgeCategory>().Single().GetVisualDescendants().OfType<Border>().First();

                var expected = palette.For(node.Category!.Value, node.ComposableDepth, node.KindDepth).FillSelected;
                Assert.Equal(expected, ((global::Avalonia.Media.ISolidColorBrush)screen.Header.Background!).Color);
                Assert.Null(square.Background);
                Assert.Equal(node.IconKey, screen.HeaderIcon.IconKey);
            }
        }

        [AvaloniaFact]
        public void Show_TheBadgeOfTheHeader_IsARowHighSquareWithAWhiteIconInTheBar()
        {
            var screen = Show();
            Select(screen, screen.CompositionNode);
            var square = screen.Header.GetVisualDescendants().OfType<BadgeCategory>().Single().GetVisualDescendants().OfType<Border>().First();

            var row = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));
            Assert.Equal(row, square.Bounds.Width);
            Assert.Equal(row, square.Bounds.Height);
            Assert.Equal((CornerRadius)Application.Current!.FindResource("CornerRadiusAll")!, square.CornerRadius);

            var top = square.TranslatePoint(new Point(0, 0), screen.Header)!.Value.Y;
            Assert.Equal(top, screen.Header.Bounds.Height - (top + square.Bounds.Height), 1);

            var white = ((global::Avalonia.Media.ISolidColorBrush)Application.Current!.FindResource("ActiveForegroundBrush")!).Color;
            Assert.Equal(white, ((global::Avalonia.Media.ISolidColorBrush)screen.HeaderIcon.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Show_TheHeaderOfAnItem_HasTheEditableNameAndTheTogglesAtTheRight()
        {
            var screen = Show();
            Select(screen, screen.LayerNode);

            var visibility = screen.VisibilityToggle!;
            var mask = screen.MaskToggle!;
            Assert.True(visibility.IsEffectivelyVisible);
            Assert.True(mask.IsEffectivelyVisible);
            Assert.True(screen.NameEditor.IsEffectivelyVisible);

            var headerWidth = screen.Header.Bounds.Width;
            // The mask is first and the visibility is on the far right.
            var visibilitySlot = (Control)visibility.Parent!;
            var visibilityRight = visibilitySlot.TranslatePoint(new Point(visibilitySlot.Bounds.Width, 0), screen.Header)!.Value.X;
            var maskLeft = mask.TranslatePoint(new Point(0, 0), screen.Header)!.Value.X;
            var visibilityLeft = visibility.TranslatePoint(new Point(0, 0), screen.Header)!.Value.X;
            var nameRight = screen.NameEditor.TranslatePoint(new Point(screen.NameEditor.Bounds.Width, 0), screen.Header)!.Value.X;
            Assert.Equal(headerWidth - 8, visibilityRight, 1);
            Assert.True(maskLeft < visibilityLeft);
            Assert.True(nameRight <= maskLeft + 0.5, "The name goes under the toggles.");
        }

        [AvaloniaFact]
        public void Show_TheToggles_HaveTheSquaresOfTheToggleColumnOfTheTree()
        {
            var screen = Show();
            Select(screen, screen.LayerNode);

            var tree = screen.Window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var treeItem = tree.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, screen.LayerNode));
            var treeToggles = treeItem.GetVisualDescendants().OfType<TogglesRow>().First();
            var treeSlots = treeToggles.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot") && border.IsEffectivelyVisible).ToList();
            var treeButtons = treeToggles.GetVisualDescendants().OfType<ToggleButton>().Where(button => button.IsEffectivelyVisible).ToList();

            var slots = screen.Header.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot")).ToList();
            var buttons = screen.Header.GetVisualDescendants().OfType<ToggleButton>().ToList();

            Assert.Equal(treeSlots.Select(slot => slot.Bounds.Size), slots.Select(slot => slot.Bounds.Size));
            Assert.Equal(treeButtons.Select(button => button.Bounds.Size), buttons.Select(button => button.Bounds.Size));
            Assert.Equal(2, slots.Count);
        }

        [AvaloniaFact]
        public void Edit_TheTogglesAndTheName_ChangeTheItem()
        {
            var screen = Show();
            Select(screen, screen.LayerNode);
            var layer = (Layer)screen.LayerNode.Item!;

            var visible = layer.PrefabService.Visibility.Value;
            screen.VisibilityToggle!.IsChecked = !visible;
            Assert.Equal(!visible, layer.PrefabService.Visibility.Value);

            screen.MaskToggle!.IsChecked = true;
            Assert.True(layer.Mask.IsMask.Value);

            screen.NameEditor.Text = "Renamed";
            Assert.Equal("Renamed", layer.PrefabService.Name.Value);
        }

        [AvaloniaFact]
        public void Show_EachKindOfItem_GivesTheMaskToggleToACompositionAndALayerOnly()
        {
            var screen = Show();
            var entity = screen.Group<EntitiesGroup>().Children[0];

            Select(screen, screen.CompositionNode);
            Assert.True(screen.MaskToggle!.IsEffectivelyVisible);

            Select(screen, entity);
            Assert.True(screen.VisibilityToggle!.IsEffectivelyVisible);
            Assert.True(screen.MaskToggle == null || !screen.MaskToggle.IsEffectivelyVisible);

            Select(screen, screen.Group<EntitiesGroup>());
            Assert.False(screen.VisibilityToggle!.IsEffectivelyVisible);
            Assert.False(screen.NameEditor.IsEffectivelyVisible);
            Assert.Equal("Entities", screen.Title.Text);
        }

        [AvaloniaFact]
        public void Show_NothingSelected_ShowsOnlyTheHint()
        {
            var screen = Show();
            Assert.True(screen.Placeholder.IsEffectivelyVisible);
            Assert.False(screen.Header.IsVisible);
            Assert.False(screen.Scroller.IsVisible);
        }

        [AvaloniaFact]
        public void Select_AComposition_ShowsItsSettingsAndItsEditor()
        {
            var screen = Show();

            Select(screen, screen.CompositionNode);

            Assert.False(screen.Placeholder.IsVisible);
            Assert.Same(screen.CompositionNode.Item, screen.Editor.Content);
            Assert.NotNull(screen.Find<CompositionView>());
            Assert.Same(screen.CompositionNode.Item, screen.Find<SettingsComposable>()!.DataContext);
            Assert.Equal("Composition", screen.HeaderIcon.IconKey);
            Assert.Equal(screen.CompositionNode.Header, screen.Title.Text);
        }

        // ---- the groups of textures, filters and modifiers, and the items in them ----

        private static Entity EntityOf(Screen screen) => (Entity)screen.Layer.ModelEntityManager.ManagerData.Items[0];

        [AvaloniaFact]
        public void Select_TheModifiersGroupOfAnEntity_ShowsTheModifiersOfThatEntity()
        {
            var screen = Show();
            var entityNode = screen.Group<EntitiesGroup>().Children[0];

            Select(screen, entityNode.Children.Single(group => group.Group is ModifiersGroup));

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
            var screen = Show();

            Select(screen, screen.LayerNode.Children.Single(group => group.Group is FiltersGroup));
            var layerPanel = screen.Find<ModifierManager>()!;
            Assert.Same(screen.Layer.TextureModifierManager, layerPanel.DataContext);
            Assert.Equal("Add Filter", layerPanel.Caption);
            Assert.Equal(typeof(CMiX.Core.Texturing.Filters.ITextureFilter), ((PanelAddTypes)layerPanel.AddContent!).Interface);

            Select(screen, screen.CompositionNode.Children.Single(group => group.Group is FiltersGroup));
            Assert.Same(((Composition)screen.CompositionNode.Item!).TextureModifierManager, screen.Find<ModifierManager>()!.DataContext);
        }

        [AvaloniaFact]
        public void Select_TheModifiersGroupOfALayerOrAComposition_ListsWhatCanBeAdded()
        {
            var screen = Show();

            Select(screen, screen.LayerNode.Children.Single(group => group.Group is ModifiersGroup));
            Assert.Equal(typeof(Layer), ((PanelAddTypes)screen.Find<ModifierManager>()!.AddContent!).PanelOwner);

            Select(screen, screen.CompositionNode.Children.Single(group => group.Group is ModifiersGroup));
            Assert.Equal(typeof(Composition), ((PanelAddTypes)screen.Find<ModifierManager>()!.AddContent!).PanelOwner);
        }

        [AvaloniaFact]
        public void Select_TheTexturesGroupOfAnEntity_ShowsItsDiffuseTexture()
        {
            var screen = Show();
            var entityNode = screen.Group<EntitiesGroup>().Children[0];

            Select(screen, entityNode.Children.Single(group => group.Group is TexturesGroup));

            Assert.IsType<TexturesGroup>(screen.Editor.Content);
            Assert.Same(EntityOf(screen).Texture.DiffuseTexture, screen.Find<DiffuseTexture>()!.DataContext);
        }

        [AvaloniaFact]
        public void Select_AModifierRow_ShowsTheEditorOfThatModifier()
        {
            var screen = Show();
            EntityOf(screen).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            TestServiceProviderFactory.Pump();
            var node = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is ModifiersGroup).Children[0];

            Select(screen, node);

            Assert.Same(node.Item, screen.Editor.Content);
            Assert.Equal("Modifier", screen.HeaderIcon.IconKey);
            Assert.Equal(node.Header, screen.Title.Text);
            var editor = screen.Editor.GetVisualDescendants().OfType<Control>().First(control => control.GetType().Name == "TranslateModifier");
            Assert.Same(node.Item, editor.DataContext);
        }

        [AvaloniaFact]
        public void Select_TheModulatorsGroupOrAModulatorRow_ShowsTheListOrTheEditor()
        {
            var screen = Show();
            EntityOf(screen).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            var modifier = (CMiX.Core.Modulation.IModulatable)EntityOf(screen).ModifierManager.ManagerData.Items[0];
            modifier.ModulatorManager.AddItem(typeof(CMiX.Core.Modulation.Modulators.LFOModulator));
            TestServiceProviderFactory.Pump();
            var modifierNode = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is ModifiersGroup).Children[0];
            var groupNode = modifierNode.Children.Single(group => group.Group is ModulatorsGroup);

            Select(screen, groupNode);
            Assert.IsType<ModulatorsGroup>(screen.Editor.Content);
            var list = screen.Find<ModifierManager>()!;
            Assert.Same(modifier.ModulatorManager, list.DataContext);
            Assert.Equal("Add Modulator", list.Caption);
            Assert.Equal(typeof(CMiX.Core.Modulation.Modulators.IModulator), ((PanelAddTypes)list.AddContent!).Interface);

            Select(screen, groupNode.Children[0]);
            Assert.Same(groupNode.Children[0].Item, screen.Editor.Content);
            Assert.Equal("Modulator", screen.HeaderIcon.IconKey);
            Assert.NotNull(screen.Editor.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.GetType().Name == "LFOModulator"));
        }

        [AvaloniaFact]
        public void Select_AFilterRow_ShowsTheEditorOfThatFilter()
        {
            var screen = Show();
            screen.Layer.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
            TestServiceProviderFactory.Pump();
            var node = screen.LayerNode.Children.Single(group => group.Group is FiltersGroup).Children[0];

            Select(screen, node);

            Assert.Same(node.Item, screen.Editor.Content);
            Assert.Equal("Filter", screen.HeaderIcon.IconKey);
            Assert.NotNull(screen.Editor.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.GetType().Name == "Blur"));
        }

        [AvaloniaFact]
        public void Select_ATextureRow_ShowsTheEditorOfThatTexture()
        {
            var screen = Show();
            EntityOf(screen).Texture.DiffuseTexture.TextureManager.AddItem(typeof(CMiX.Core.Texturing.Sources.Image));
            TestServiceProviderFactory.Pump();
            var node = screen.Group<EntitiesGroup>().Children[0].Children.Single(group => group.Group is TexturesGroup).Children[0];

            Select(screen, node);

            Assert.Same(node.Item, screen.Editor.Content);
            Assert.Equal("Texture", screen.HeaderIcon.IconKey);
            Assert.NotNull(screen.Editor.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.GetType().Name == "Image"));
        }

        [AvaloniaFact]
        public void Select_AModifierInTheInspectorList_SelectsItInTheTree()
        {
            var screen = Show();
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
            var screen = Show();

            Select(screen, screen.LayerNode);

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
            var screen = Show();
            var node = screen.Group<EntitiesGroup>().Children[index];

            Select(screen, node);

            Assert.Same(node.Item, screen.Editor.Content);
            var view = screen.Editor.GetVisualDescendants().OfType<Control>().First(visual => visual.GetType() == editor);
            Assert.Same(node.Item, view.DataContext);
        }

        [AvaloniaFact]
        public void Select_ACameraOrALight_ShowsItsEditor()
        {
            var screen = Show();

            Select(screen, screen.Group<CamerasGroup>().Children[0]);
            Assert.NotNull(screen.Find<CameraView>());
            Assert.Equal("Camera", screen.HeaderIcon.IconKey);

            Select(screen, screen.Group<LightsGroup>().Children[0]);
            Assert.NotNull(screen.Find<LightView>());
            Assert.Null(screen.Find<CameraView>());
        }

        [AvaloniaFact]
        public void Select_AGroup_ShowsItsNameAndTheListPanelOfItsKind()
        {
            var screen = Show();

            Select(screen, screen.Group<CamerasGroup>());
            Assert.True(screen.Header.IsVisible);
            Assert.Equal("Cameras", screen.Title.Text);
            Assert.False(screen.Placeholder.IsVisible);
            Assert.True(screen.Scroller.IsVisible);
            Assert.Same(screen.Layer, Assert.IsType<CamerasGroup>(screen.Editor.Content).Layer);
            Assert.Same(screen.Layer, screen.Find<TabLayerCamera>()!.DataContext);
            Assert.Null(screen.Find<TabLayerEntity>());

            Select(screen, screen.Group<EntitiesGroup>());
            Assert.Equal("Entities", screen.Title.Text);
            Assert.Same(screen.Layer, screen.Find<TabLayerEntity>()!.DataContext);
            Assert.Null(screen.Find<TabLayerCamera>());

            Select(screen, screen.Group<LightsGroup>());
            Assert.Equal("Lights", screen.Title.Text);
            Assert.Same(screen.Layer, screen.Find<TabLayerLight>()!.DataContext);
            Assert.Null(screen.Find<TabLayerEntity>());
        }

        [AvaloniaFact]
        public void Select_AnotherItemOrRename_UpdatesTheInspector()
        {
            var screen = Show();
            var entity = screen.Group<EntitiesGroup>().Children[0];

            Select(screen, screen.LayerNode);
            Assert.NotNull(screen.Find<TabLayerPostFX>());

            Select(screen, entity);
            Assert.Null(screen.Find<TabLayerPostFX>());
            Assert.NotNull(screen.Find<ModelEntity>());

            ((IPrefab)entity.Item!).PrefabService.Name.Value = "Floor";
            TestServiceProviderFactory.Pump();
            Assert.Equal("Floor", screen.Title.Text);
        }

        [AvaloniaFact]
        public void Remove_TheSelectedItem_ClosesTheEditor()
        {
            var screen = Show();
            // The only camera: when it goes, the selection falls back to the group of cameras.
            var camera = screen.Group<CamerasGroup>().Children[0];
            Select(screen, camera);
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
            var screen = Show();
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
            var screen = Show();
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

            Select(screen, node);

            var expanders = screen.Editor.GetVisualDescendants().OfType<Expander>().ToList();
            Assert.NotEmpty(expanders);
            Assert.All(expanders, expander => Assert.True(expander.IsExpanded, "Closed panel: " + expander.Header));
        }

        // ---- the inspector follows what happens to the item ----

        [AvaloniaFact]
        public async Task Move_AnEntityToAnotherLayer_KeepsItInTheInspector()
        {
            var screen = Show();
            var composition = (Composition)screen.CompositionNode.Item!;
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            var entity = screen.Group<EntitiesGroup>().Children[0];
            var item = entity.Item;
            Select(screen, entity);
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
            var screen = Show();
            Select(screen, screen.LayerNode);
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
            var screen = Show();
            var camera = screen.Group<CamerasGroup>().Children[0];
            Select(screen, camera);

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

        [AvaloniaFact]
        public void Show_AfterAnotherTab_ShowsTheCurrentSelection()
        {
            var screen = Show();
            Select(screen, screen.LayerNode);

            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");
            Select(screen, screen.Group<CamerasGroup>().Children[0]);
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Inspector");

            var inspector = screen.Window.GetVisualDescendants().OfType<PanelInspector>().Single();
            Assert.NotNull(inspector.GetVisualDescendants().OfType<CameraView>().FirstOrDefault());
            Assert.Null(inspector.GetVisualDescendants().OfType<TabLayerPostFX>().FirstOrDefault());
        }

        // ---- the inspector tab opens on a click in the tree ----

        private static Point CenterOfRow(Screen screen, TreeNode node)
        {
            var tree = screen.Window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            var item = tree.GetVisualDescendants().OfType<TreeViewItem>().First(row => ReferenceEquals(row.DataContext, node));
            var rowBorder = item.GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
            // Right of the name and left of the buttons: on the row, on nothing else.
            return rowBorder.TranslatePoint(new Point(rowBorder.Bounds.Width * 0.5, rowBorder.Bounds.Height / 2), screen.Window)!.Value;
        }

        private static void Click(Screen screen, Point point)
        {
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            TestServiceProviderFactory.Pump();
        }

        private static void MiddleClick(Screen screen, Point point)
        {
            screen.Window.MouseDown(point, MouseButton.Middle);
            screen.Window.MouseUp(point, MouseButton.Middle);
            TestServiceProviderFactory.Pump();
        }

        private static string SelectedTab(Screen screen) =>
            (TestServiceProviderFactory.MainTabControl(screen.Window).SelectedItem as TabItem is { Header: AppIcon icon }) ? icon.IconKey : "";

        // A click beside the rows (under the last row) gives the tree the focus on its cursor row, so the arrow keys work at once.
        [AvaloniaFact]
        public void Click_BesideTheRows_FocusesTheCursorRow()
        {
            var screen = Show();
            var tree = screen.Window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var treeView = tree.GetVisualDescendants().OfType<TreeView>().First();
            screen.Window.UpdateLayout();
            var point = treeView.TranslatePoint(new Point(treeView.Bounds.Width / 2, treeView.Bounds.Height - 4), screen.Window)!.Value;

            Click(screen, point);

            var focused = screen.Window.FocusManager?.GetFocusedElement() as Visual;
            Assert.True(focused is TreeViewItem, $"focused: {focused?.GetType().Name ?? "nothing"}");
        }

        [AvaloniaFact]
        public void MiddleClick_ARow_KeepsTheTabAndTheInspectorHoldsThatItem()
        {
            var screen = Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            MiddleClick(screen, CenterOfRow(screen, screen.Group<CamerasGroup>().Children[0]));

            Assert.Equal("Settings", SelectedTab(screen));
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Inspector");
            Assert.NotNull(screen.Find<CameraView>());
        }

        [AvaloniaFact]
        public void MiddleClick_AGroup_KeepsTheTabAndTheInspectorHoldsTheGroup()
        {
            var screen = Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            MiddleClick(screen, CenterOfRow(screen, screen.Group<LightsGroup>()));

            Assert.Equal("Settings", SelectedTab(screen));
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Inspector");
            Assert.Equal("Lights", screen.Title.Text);
        }

        [AvaloniaFact]
        public void Click_TheVisibilityButton_KeepsTheTab()
        {
            var screen = Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");
            var tree = screen.Window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var item = tree.GetVisualDescendants().OfType<TreeViewItem>().First(row => ReferenceEquals(row.DataContext, screen.LayerNode));
            var toggle = item.GetVisualDescendants().OfType<TogglesRow>().First()
                .GetVisualDescendants().OfType<ToggleButton>().First(button => CMiX.UI.AttachedProperties.ToggleDrag.GetGroup(button) == "visibility");
            var point = toggle.TranslatePoint(new Point(toggle.Bounds.Width / 2, toggle.Bounds.Height / 2), screen.Window)!.Value;

            Click(screen, point);

            Assert.Equal("Settings", SelectedTab(screen));
        }

        [AvaloniaFact]
        public void Select_FromAProgramOrAList_KeepsTheTab()
        {
            var screen = Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            Select(screen, screen.LayerNode);
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
            var screen = Show();
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            Click(screen, CenterOfRow(screen, screen.LayerNode));

            Assert.Equal("Settings", SelectedTab(screen));
            Assert.Same(screen.Layer, screen.Tree.SelectedNode!.Item);
        }

        // The layer view is also a tab with its own scroll viewer. In the inspector that viewer is off, so only the inspector scrolls.
        [AvaloniaFact]
        public void HostedView_Show_DoesNotScrollOnItsOwn()
        {
            var screen = Show();
            Select(screen, screen.LayerNode);
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
    }
}
