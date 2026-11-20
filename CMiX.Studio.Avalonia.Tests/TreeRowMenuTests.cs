using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Camera = CMiX.Core.Rendering.Cameras.Camera;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using FullscreenQuad = CMiX.Core.Compositing.FullscreenQuad;
using Layer = CMiX.Core.Compositing.Layer;
using LightEntity = CMiX.Core.Rendering.Lights.LightEntity;
using Project = CMiX.Core.Compositing.Project;
using TextEntity = CMiX.Core.Compositing.TextEntity;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // The menu of a row of the project tree: the rows of the menus of the lists, and the rows that add.
    public class TreeRowMenuTests : IDisposable
    {
        private readonly List<Screen> screens = new();

        // The menus and the windows of a test are closed, so an open popup never reaches the next test.
        public void Dispose()
        {
            foreach (var screen in screens)
            {
                screen.TreeView.ContextMenu?.Close();
                screen.Panel.Close();
                screen.Window.Close();
            }
        }

        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public TreeViewProject View { get; init; } = null!;
            public ProjectTree Tree { get; init; } = null!;
            public Project Project { get; init; } = null!;
            public Composition A { get; init; } = null!;
            public Composition B { get; init; } = null!;

            public TreeView TreeView => View.GetVisualDescendants().OfType<TreeView>().First();

            public TreeViewItem RowOf(TreeNode node) =>
                View.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, node));

            public Layer LayerOfA(int index) => (Layer)A.LayerManager.ManagerData.Items[index];
            public TreeNode LayerNodeOfA(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
            public TreeNode Group<T>(int layerIndex) where T : TreeGroup => LayerNodeOfA(layerIndex).Children.Single(group => group.Group is T);

            // A right click on the row. The pointer goes back to the left of the window first, so a menu that opens at the pointer
            // never moves to the right of the last click and out of the window.
            public ContextMenu? OpenMenu(TreeNode node)
            {
                Window.MouseMove(new Point(30, 30));
                Dispatcher.UIThread.RunJobs();
                var row = RowOf(node);
                row.RaiseEvent(new ContextRequestedEventArgs { Source = row });
                Dispatcher.UIThread.RunJobs();
                return TreeView.ContextMenu;
            }

            public static List<string> Headers(ContextMenu menu) =>
                menu.Items.OfType<MenuItem>().Where(item => item.IsVisible).Select(item => item.Header?.ToString() ?? string.Empty).ToList();


            public static void Click(ContextMenu menu, string header)
            {
                menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header))
                    .RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                Dispatcher.UIThread.RunJobs();
            }

            // The panel of the Add row of the menus.
            public AddPanel Panel => View.MenuAddPanel;

            // A click on the Add row of the open menu opens the panel beside it.
            public AddPanel OpenAddPanel(ContextMenu menu)
            {
                Click(menu, "Add");
                Settle();
                return Panel;
            }

            public void Settle()
            {
                Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Dispatcher.UIThread.RunJobs();
            }

            // A pointer click on a row of the panel. A header can have the text of its only row, so the row is not a header.
            public void Pick(string text)
            {
                Settle();
                var texts = Panel.List.VisibleTexts;
                var row = Panel.List.RowVisuals.Where((visual, index) => !visual.Classes.Contains("header") && texts[index] == text).First();
                // Every group is open, so the list can be longer than the panel: the row scrolls into view first.
                row.BringIntoView();
                Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                var point = row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), Window)!.Value;
                Window.MouseMove(point);
                Dispatcher.UIThread.RunJobs();
                Window.MouseDown(point, MouseButton.Left);
                Window.MouseUp(point, MouseButton.Left);
                Dispatcher.UIThread.RunJobs();
            }
        }

        // A holds two layers, B holds one.
        private Screen Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            project.CompositionManager.AddItem(typeof(Composition));
            var a = (Composition)project.CompositionManager.ManagerData.Items[0];
            var b = (Composition)project.CompositionManager.ManagerData.Items[1];
            a.LayerManager.AddItem(typeof(Layer));
            a.LayerManager.AddItem(typeof(Layer));
            b.LayerManager.AddItem(typeof(Layer));

            var tree = TreeFixtures.CreateTree(provider);
            var view = new TreeViewProject { DataContext = tree, Width = 400, HorizontalAlignment = HorizontalAlignment.Left };
            var window = new Window { Content = view, Width = 1200, Height = 900 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var screen = new Screen { Window = window, View = view, Tree = tree, Project = project, A = a, B = b };
            screens.Add(screen);
            return screen;
        }

        [AvaloniaFact]
        public void Open_TheMenuOfAComposition_StartsWithTheNameAndHasTheListRows()
        {
            var screen = Show();

            var headers = Screen.Headers(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!);

            Assert.Equal(screen.A.PrefabService.Name.Value, headers[0]);
            Assert.Equal(new[] { "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Delete" },
                headers.Skip(2));
        }

        [AvaloniaFact]
        public void RightClick_ARow_OpensTheMenu()
        {
            var screen = Show();

            var menu = screen.OpenMenu(screen.Tree.CompositionNodes()[0])!;

            Assert.True(menu.IsOpen);
        }

        [AvaloniaFact]
        public void RightClick_AnotherRow_OpensTheMenuOfThatRow()
        {
            var screen = Show();
            screen.OpenMenu(screen.Tree.CompositionNodes()[0]);

            var menu = screen.OpenMenu(screen.Tree.CompositionNodes()[1])!;

            Assert.True(menu.IsOpen);
            Assert.Equal(screen.B.PrefabService.Name.Value, Screen.Headers(menu)[0]);
        }

        // The menu of a row must be the menu of that row, whatever rows were clicked before and whatever rows are above it.
        [AvaloniaFact]
        public void RightClick_AGroupAfterItsComposition_OpensTheMenuOfTheGroup()
        {
            var screen = Show();
            screen.OpenMenu(screen.Tree.CompositionNodes()[0]);
            screen.OpenMenu(screen.LayerNodeOfA(0));

            var entities = Screen.Headers(screen.OpenMenu(screen.Group<EntitiesGroup>(0))!);
            var cameras = Screen.Headers(screen.OpenMenu(screen.Group<CamerasGroup>(0))!);
            var lights = Screen.Headers(screen.OpenMenu(screen.Group<LightsGroup>(0))!);

            Assert.Equal(new[] { "Entities", "-", "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" }, entities);
            Assert.Equal(new[] { "Cameras", "-", "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" }, cameras);
            Assert.Equal(new[] { "Lights", "-", "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" }, lights);
        }

        [AvaloniaFact]
        public void RightClick_AnEntityAfterItsGroup_OpensTheEntityMenuWithoutTheGroupAddRows()
        {
            var screen = Show();
            screen.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            var entityNode = screen.Group<EntitiesGroup>(0).Children[0];
            screen.OpenMenu(screen.Tree.CompositionNodes()[0]);
            screen.OpenMenu(screen.Group<EntitiesGroup>(0));

            var headers = Screen.Headers(screen.OpenMenu(entityNode)!);

            Assert.Equal(new[] { "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" },
                headers.Skip(2));
            Assert.DoesNotContain(headers, header => header is "Add Entity" or "Add Fullscreen Quad" or "Add Text" or "Add Layer");
        }

        [AvaloniaTheory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void RightClick_AfterAnyOtherClick_OpensExactlyOneMenu(int rounds)
        {
            var screen = Show();
            screen.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            var order = new[] { screen.Tree.CompositionNodes()[0], screen.LayerNodeOfA(0), screen.Group<EntitiesGroup>(0), screen.Group<EntitiesGroup>(0).Children[0] };

            for (var round = 0; round < rounds; round++)
                foreach (var node in order)
                {
                    var menu = screen.OpenMenu(node)!;

                    Assert.True(menu.IsOpen);
                    Assert.Equal(node.Header, Screen.Headers(menu)[0]);
                }

            Assert.All(screen.View.GetVisualDescendants().OfType<TreeViewItem>(), row => Assert.Null(row.ContextMenu));
        }

        [AvaloniaFact]
        public void AddLayer_OnAComposition_AddsALayerToIt()
        {
            var screen = Show();

            screen.OpenAddPanel(screen.OpenMenu(screen.Tree.CompositionNodes()[1])!);
            screen.Pick("Layer");

            Assert.Equal(2, screen.B.LayerManager.ManagerData.Items.Count);
            Assert.Equal(2, screen.Tree.CompositionNodes()[1].Children[0].Children.Count);
        }

        [AvaloniaFact]
        public void Open_TheMenuOfALayer_StartsWithAddThenHasTheListRows()
        {
            var screen = Show();

            var headers = Screen.Headers(screen.OpenMenu(screen.LayerNodeOfA(0))!);

            Assert.Equal(new[] { "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Delete" },
                headers.Skip(2));
        }

        [AvaloniaFact]
        public void Open_TheAddPanelOfALayer_EnablesTheContent()
        {
            var screen = Show();

            var panel = screen.OpenAddPanel(screen.OpenMenu(screen.LayerNodeOfA(0))!);

            Assert.True(panel.IsOpen);
            Assert.Equal(new[] { "Content", "Filter", "Modifier", "Project" }, panel.List.Groups!.Select(group => group.Header));
            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text", "Camera", "Light" }, panel.List.Groups!.Single(group => group.Header == "Content").Items.Where(item => item.IsEnabled).Select(item => item.Text));
        }

        [AvaloniaFact]
        public void Pick_EachKindInTheAddPanelOfALayer_AddsItToItsList()
        {
            var screen = Show();
            var layer = screen.LayerOfA(1);
            var node = screen.LayerNodeOfA(1);

            foreach (var header in new[] { "Entity", "Fullscreen Quad", "Text", "Camera", "Light" })
            {
                screen.OpenAddPanel(screen.OpenMenu(node)!);
                screen.Pick(header);
            }

            Assert.Collection(layer.ModelEntityManager.ManagerData.Items,
                item => Assert.IsType<Entity>(item),
                item => Assert.IsType<FullscreenQuad>(item),
                item => Assert.IsType<TextEntity>(item));
            Assert.IsType<Camera>(Assert.Single(layer.CameraManager.ManagerData.Items));
            Assert.IsType<LightEntity>(Assert.Single(layer.LightManager.ManagerData.Items));
            Assert.Empty(screen.LayerOfA(0).ModelEntityManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Duplicate_OnALayer_CopiesTheLayerRightAfterIt()
        {
            var screen = Show();
            var original = screen.LayerOfA(0);

            Screen.Click(screen.OpenMenu(screen.LayerNodeOfA(0))!, "Duplicate");

            Assert.Equal(3, screen.A.LayerManager.ManagerData.Items.Count);
            Assert.Same(original, screen.A.LayerManager.ManagerData.Items[0]);
            Assert.NotSame(original, screen.A.LayerManager.ManagerData.Items[1]);
        }

        [AvaloniaFact]
        public void MoveDown_OnTheFirstComposition_MovesItAndTheTree()
        {
            var screen = Show();

            Screen.Click(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!, "Move Down");

            Assert.Equal(new IControl[] { screen.B, screen.A }, screen.Project.CompositionManager.ManagerData.Items);
            Assert.Same(screen.B, screen.Tree.CompositionNodes()[0].Item);
        }

        [AvaloniaFact]
        public void MoveUp_OnTheFirstRow_IsDisabled()
        {
            var screen = Show();

            var menu = screen.OpenMenu(screen.Tree.CompositionNodes()[0])!;

            Assert.False(menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Move Up")).IsEnabled);
            Assert.True(menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Move Down")).IsEnabled);
        }

        [AvaloniaFact]
        public void RightClick_ARowThatIsNotSelected_ActsOnTheClickedRow()
        {
            var screen = Show();
            var first = screen.LayerOfA(0);
            var second = screen.LayerOfA(1);
            screen.Tree.SelectedNode = screen.LayerNodeOfA(1);

            Screen.Click(screen.OpenMenu(screen.LayerNodeOfA(0))!, "Delete");

            Assert.Equal(new IControl[] { second }, screen.A.LayerManager.ManagerData.Items);
            Assert.DoesNotContain(first, screen.A.LayerManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Open_TheMenuOfTheEntitiesGroup_EnablesOnlyEntities()
        {
            var screen = Show();

            var headers = Screen.Headers(screen.OpenMenu(screen.Group<EntitiesGroup>(0))!);

            Assert.Equal(new[] { "Entities", "-", "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" }, headers);
            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text" }, screen.OpenAddPanel(screen.TreeView.ContextMenu!).List.Groups!.Single(group => group.Header == "Content").Items.Where(item => item.IsEnabled).Select(item => item.Text));
        }

        [AvaloniaFact]
        public void Pick_TheRowsOfTheEntitiesGroupPanel_AddTheirTypeToTheLayer()
        {
            var screen = Show();
            var layer = screen.LayerOfA(0);

            foreach (var header in new[] { "Entity", "Fullscreen Quad", "Text" })
            {
                screen.OpenAddPanel(screen.OpenMenu(screen.Group<EntitiesGroup>(0))!);
                screen.Pick(header);
            }

            Assert.Collection(layer.ModelEntityManager.ManagerData.Items,
                item => Assert.IsType<Entity>(item),
                item => Assert.IsType<FullscreenQuad>(item),
                item => Assert.IsType<TextEntity>(item));
            Assert.Equal(3, screen.Group<EntitiesGroup>(0).Children.Count);
        }

        [AvaloniaFact]
        public void Pick_ThePanelOfTheCamerasOrLightsGroup_AddsACameraOrALight()
        {
            var screen = Show();
            var layer = screen.LayerOfA(0);

            screen.OpenAddPanel(screen.OpenMenu(screen.Group<CamerasGroup>(0))!);
            screen.Pick("Camera");
            screen.OpenAddPanel(screen.OpenMenu(screen.Group<LightsGroup>(0))!);
            screen.Pick("Light");

            Assert.IsType<Camera>(Assert.Single(layer.CameraManager.ManagerData.Items));
            Assert.IsType<LightEntity>(Assert.Single(layer.LightManager.ManagerData.Items));
        }

        [AvaloniaFact]
        public void Open_TheMenuOfAnEntity_HasTheSlotRowsWithRemove()
        {
            var screen = Show();
            screen.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            var headers = Screen.Headers(screen.OpenMenu(screen.Group<EntitiesGroup>(0).Children[0])!);

            Assert.Equal(new[] { "Add", "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" },
                headers.Skip(2));
        }

        [AvaloniaFact]
        public void Remove_OnAnEntity_TakesItOutOfThatLayerOnly()
        {
            var screen = Show();
            var layer = screen.LayerOfA(0);
            layer.ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            Screen.Click(screen.OpenMenu(screen.Group<EntitiesGroup>(0).Children[0])!, "Remove");

            Assert.Empty(layer.ModelEntityManager.ManagerData.Items);
            Assert.Empty(screen.Group<EntitiesGroup>(0).Children);
        }

        // ---- the row under the pointer when the menu opens (see MenuAnchor) ----

        private static string? AnchorOf(ContextMenu menu) =>
            menu.Items.OfType<MenuItem>().Where(item => item.IsVisible && CMiX.UI.AttachedProperties.MenuAnchor.GetIsAnchor(item))
                .Select(item => item.Header?.ToString()).SingleOrDefault();

        [AvaloniaFact]
        public void Open_TheMenuOfAComposition_HighlightsAdd()
        {
            var screen = Show();

            var menu = screen.OpenMenu(screen.Tree.CompositionNodes()[0])!;

            Assert.Equal("Add", AnchorOf(menu));
        }

        [AvaloniaFact]
        public void Open_TheMenuOfALayerAGroupOrAnEntity_HighlightsAdd()
        {
            var screen = Show();
            screen.A.LayerManager.ManagerData.Items.OfType<Layer>().First().ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Add", AnchorOf(screen.OpenMenu(screen.LayerNodeOfA(0))!));
            Assert.Equal("Add", AnchorOf(screen.OpenMenu(screen.Group<EntitiesGroup>(0))!));
            Assert.Equal("Add", AnchorOf(screen.OpenMenu(screen.Group<EntitiesGroup>(0).Children[0])!));
        }

        // A right click with the pointer: the middle of the Add row is on the pointer, and after a use of Duplicate, the middle of Duplicate.
        [AvaloniaFact]
        public void RightClick_ARow_PutsTheAddRowThenTheLastRowUsedUnderThePointer()
        {
            var screen = Show();
            screen.Settle();
            var row = screen.RowOf(screen.LayerNodeOfA(0));
            var border = row.GetVisualDescendants().OfType<Border>().First(b => b.Name == "RowBorder");
            // Far enough from the left edge of the window, so the menu, which is centered on the pointer, is not pushed right.
            var point = border.TranslatePoint(new Point(border.Bounds.Width - 40, border.Bounds.Height / 2), screen.Window)!.Value;

            Point MiddleOf(ContextMenu menu, string header)
            {
                screen.Settle();
                var item = menu.Items.OfType<MenuItem>().Single(candidate => Equals(candidate.Header, header));
                return item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), screen.Window)!.Value;
            }

            ContextMenu RightClick()
            {
                screen.Window.MouseMove(point);
                screen.Window.MouseDown(point, MouseButton.Right);
                screen.Window.MouseUp(point, MouseButton.Right);
                Dispatcher.UIThread.RunJobs();
                return screen.TreeView.ContextMenu!;
            }

            var menu = RightClick();
            var add = MiddleOf(menu, "Add");
            Assert.Equal(point.X, add.X, 1.5);
            Assert.Equal(point.Y, add.Y, 1.5);

            Screen.Click(menu, "Duplicate");
            screen.Panel.Close();
            menu.Close();
            Dispatcher.UIThread.RunJobs();

            var again = RightClick();
            var duplicate = MiddleOf(again, "Duplicate");
            Assert.Equal(point.X, duplicate.X, 1.5);
            Assert.Equal(point.Y, duplicate.Y, 1.5);
        }

        [AvaloniaFact]
        public void Open_TheMenuAfterANewComposition_StillHighlightsAdd()
        {
            var screen = Show();
            screen.TreeView.RaiseEvent(new ContextRequestedEventArgs { Source = screen.TreeView });
            Dispatcher.UIThread.RunJobs();
            screen.OpenAddPanel(screen.TreeView.ContextMenu!);
            screen.Pick("Composition");

            var menu = screen.OpenMenu(screen.Tree.CompositionNodes()[0])!;

            Assert.Equal("Add", AnchorOf(menu));
        }

        [AvaloniaFact]
        public void Open_TheMenuAgain_PutsTheLastRowUsedUnderThePointer()
        {
            var screen = Show();
            Screen.Click(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!, "Duplicate");

            Assert.Equal("Duplicate", AnchorOf(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!));
            Assert.Equal("Duplicate", AnchorOf(screen.OpenMenu(screen.Tree.CompositionNodes()[1])!));

            Screen.Click(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!, "Add");

            Assert.Equal("Add", AnchorOf(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!));
        }

        [AvaloniaFact]
        public void RightClick_OutsideTheRows_OpensTheMenuWhoseAddPanelAddsAComposition()
        {
            var screen = Show();
            var before = screen.Project.CompositionManager.ManagerData.Items.Count;

            screen.TreeView.RaiseEvent(new ContextRequestedEventArgs { Source = screen.TreeView });
            Dispatcher.UIThread.RunJobs();

            var menu = screen.TreeView.ContextMenu!;
            Assert.NotNull(menu);
            Assert.Contains("Add", Screen.Headers(menu));
            Assert.False(menu.Items.OfType<MenuItem>().First(item => item.Header?.ToString() == "Duplicate").IsEnabled);
            Assert.False(menu.Items.OfType<MenuItem>().First(item => item.Header?.ToString() == "Remove").IsEnabled);
            Assert.All(screen.View.GetVisualDescendants().OfType<TreeViewItem>(), row => Assert.Null(row.ContextMenu));

            // The menu adds nothing by itself. The composition is an entry of the panel.
            Assert.Equal(before, screen.Project.CompositionManager.ManagerData.Items.Count);
            screen.OpenAddPanel(menu);
            screen.Pick("Composition");

            Assert.Equal(before + 1, screen.Project.CompositionManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void RightClick_ARow_OffersACompositionOnlyWhereItCanGo()
        {
            var screen = Show();

            Assert.DoesNotContain("Add Composition", Screen.Headers(screen.OpenMenu(screen.LayerNodeOfA(0))!));
            Assert.DoesNotContain("Add Composition", Screen.Headers(screen.OpenMenu(screen.Tree.CompositionNodes()[0])!));
        }

        private static bool IsRenaming(TextBoxEditable box) =>
            box.GetVisualDescendants().OfType<TextBox>().Any(input => input.IsVisible);

        // A double click on the name starts the rename.
        private static (TextBoxEditable Box, Point Point) StartRename(Screen screen, TreeNode node)
        {
            var box = screen.RowOf(node).GetVisualDescendants().OfType<TextBoxEditable>().First();
            var point = box.TranslatePoint(new Point(10, box.Bounds.Height / 2), screen.Window)!.Value;
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            return (box, point);
        }

        [AvaloniaFact]
        public void ContextRequest_WhileTheNameIsEdited_OpensNoMenu()
        {
            var screen = Show();
            var node = screen.LayerNodeOfA(0);
            var (box, _) = StartRename(screen, node);
            Assert.True(IsRenaming(box));

            var row = screen.RowOf(node);
            var input = box.GetVisualDescendants().OfType<TextBox>().First(textBox => textBox.IsVisible);
            input.RaiseEvent(new ContextRequestedEventArgs { Source = input });
            Dispatcher.UIThread.RunJobs();

            Assert.Null(screen.TreeView.ContextMenu);
        }

        [AvaloniaFact]
        public void RightClick_DuringARename_EndsTheRenameBeforeTheNextMenu()
        {
            var screen = Show();
            var node = screen.LayerNodeOfA(0);
            var row = screen.RowOf(node);
            var (box, point) = StartRename(screen, node);
            Assert.True(IsRenaming(box));

            screen.Window.MouseDown(point, MouseButton.Right);
            screen.Window.MouseUp(point, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.False(IsRenaming(box));
            Assert.Null(screen.TreeView.ContextMenu);

            screen.Window.MouseDown(point, MouseButton.Right);
            screen.Window.MouseUp(point, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.True(screen.TreeView.ContextMenu?.IsOpen);
        }

        [AvaloniaFact]
        public void Rename_OnARow_StartsTheEditOfItsName()
        {
            var screen = Show();
            var node = screen.LayerNodeOfA(1);

            Screen.Click(screen.OpenMenu(node)!, "Rename");

            var box = screen.RowOf(node).GetVisualDescendants().OfType<CMiX.UI.Controls.String.TextBoxEditable>().First();
            Assert.True(box.IsEditing);
        }

        private static MenuItem RowOf(ContextMenu menu, string header) =>
            menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));

        [AvaloniaFact]
        public void Open_TheMenuOfALayer_ShowsTheShortcutsOfTheMapOnItsRows()
        {
            var screen = Show();

            var menu = screen.OpenMenu(screen.LayerNodeOfA(0))!;

            Assert.Equal(new KeyGesture(Key.A, KeyModifiers.Shift), RowOf(menu, "Add").InputGesture);
            Assert.Equal(new KeyGesture(Key.D, KeyModifiers.Shift), RowOf(menu, "Duplicate").InputGesture);
            Assert.Equal(new KeyGesture(Key.R, KeyModifiers.Control), RowOf(menu, "Rename").InputGesture);
            Assert.Equal(new KeyGesture(Key.Up, KeyModifiers.Alt), RowOf(menu, "Move Up").InputGesture);
            Assert.Equal(new KeyGesture(Key.Down, KeyModifiers.Alt), RowOf(menu, "Move Down").InputGesture);
            Assert.Equal(new KeyGesture(Key.Delete), RowOf(menu, "Delete").InputGesture);
            Assert.Null(RowOf(menu, "Move To Top").InputGesture);
        }

        [AvaloniaFact]
        public void Open_TheMenuAfterARebind_ShowsTheNewGesture()
        {
            var screen = Show();
            screen.Tree.Shortcuts.Set("Tree.Duplicate", new KeyGesture(Key.D, KeyModifiers.Control | KeyModifiers.Shift));

            var menu = screen.OpenMenu(screen.LayerNodeOfA(0))!;

            Assert.Equal(new KeyGesture(Key.D, KeyModifiers.Control | KeyModifiers.Shift), RowOf(menu, "Duplicate").InputGesture);
        }

        [AvaloniaFact]
        public void Open_TheMenuOfARow_MarksTheTitleAndTheAddRowOnly()
        {
            var screen = Show();
            screen.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            var layer = screen.OpenMenu(screen.LayerNodeOfA(0))!;
            var title = layer.Items.OfType<MenuItem>().First();
            Assert.Contains("title", title.Classes);
            Assert.Contains("opens", RowOf(layer, "Add").Classes);
            Assert.DoesNotContain("danger", RowOf(layer, "Delete").Classes);
            Assert.DoesNotContain("danger", RowOf(layer, "Duplicate").Classes);

            var entity = screen.OpenMenu(screen.Group<EntitiesGroup>(0).Children[0])!;
            Assert.DoesNotContain("danger", RowOf(entity, "Remove").Classes);
        }
    }
}
