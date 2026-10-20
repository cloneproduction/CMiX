using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Camera = CMiX.Core.Rendering.Cameras.Camera;
using Layer = CMiX.Core.Compositing.Layer;
using LightEntity = CMiX.Core.Rendering.Lights.LightEntity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The context menu of a row in the lists of the project: its entities, its cameras and its lights.
    // An item of these lists can be in a layer and not in the list of the manager of the tab.
    public class RepositoryRowMenuTests
    {
        public static IEnumerable<object[]> Kinds() =>
            new[] { typeof(Entity), typeof(Camera), typeof(LightEntity) }.Select(kind => new object[] { kind });

        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public IServiceProvider Provider { get; init; } = null!;
            public MainViewModel ViewModel { get; init; } = null!;
            public Type Kind { get; init; } = null!;
            public PrefabManager Manager { get; init; } = null!;
            public ListBoxCMiX List { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;

            public List<ListItemIcon> Rows => List.GetVisualDescendants().OfType<ListItemIcon>().ToList();

            public ListItemIcon RowOf(IControl item) => Rows.Single(row => ReferenceEquals(row.DataContext, item));

            // Every item of the kind in the project, in a layer or in the list of the tab.
            public IEnumerable<IControl> ItemsOfTheProject => Kind == typeof(Camera) ? ViewModel.ControlRepository.Cameras
                : Kind == typeof(LightEntity) ? ViewModel.ControlRepository.Lights
                : ViewModel.ControlRepository.Entities;

            public Point CenterOf(Control control) =>
                control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), Window)!.Value;
        }

        private static PrefabManager LayerListOf(Layer layer, Type kind) =>
            kind == typeof(Camera) ? layer.CameraManager
            : kind == typeof(LightEntity) ? layer.LightManager
            : layer.ModelEntityManager;

        private static Screen Show(Type kind, int count = 3)
        {
            var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var manager = kind == typeof(Camera) ? viewModel.CameraManager
                : kind == typeof(LightEntity) ? viewModel.LightManager
                : viewModel.EntityManager;
            var tabs = TestServiceProviderFactory.MainTabControl(window);
            tabs.SelectedItem = kind == typeof(Entity)
                ? tabs.Items.OfType<TabItem>().Single(tab => tab.Content is Views.MainWindowEntitiesTab)
                : tabs.Items.OfType<RepositoryTab>().Single(tab => ReferenceEquals(tab.DataContext, manager));
            Dispatcher.UIThread.RunJobs();
            for (var i = 0; i < count; i++)
                manager.AddItem(kind);
            TestServiceProviderFactory.Pump();

            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Screen
            {
                Window = window,
                Provider = provider,
                ViewModel = viewModel,
                Kind = kind,
                Manager = manager,
                List = window.GetVisualDescendants().OfType<RepositoryManager>().Single().GetVisualDescendants().OfType<ListBoxCMiX>().First(),
                Undo = undo
            };
        }

        // An item that lives in a layer. It shows in the list of the project, but not in the list of the manager of the tab.
        private static IControl AddToALayer(Screen screen)
        {
            var layer = (Layer)screen.Provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var list = LayerListOf(layer, screen.Kind);
            list.AddItem(screen.Kind);
            TestServiceProviderFactory.Pump();
            screen.Undo.Clear();
            return list.ManagerData.Items.Single();
        }

        private static ContextMenu? MenuOf(Control row) =>
            row.GetSelfAndVisualAncestors().OfType<Control>().Select(control => control.ContextMenu).FirstOrDefault(menu => menu != null);

        private static MenuItem Row(Control row, string header) =>
            MenuOf(row)!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));

        // The right click of a user raises ContextRequested on the row.
        private static void OpenMenu(Control row)
        {
            row.RaiseEvent(new ContextRequestedEventArgs { Source = row });
            Dispatcher.UIThread.RunJobs();
        }

        private static void Click(Control row, string header)
        {
            OpenMenu(row);
            Row(row, header).RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Menu_StartsWithTheNameOfTheItemAndALine_ThenListsTheRepositoryRows(Type kind)
        {
            var screen = Show(kind, 1);
            var item = screen.Manager.ManagerData.Items.Single();
            var row = screen.RowOf(item);

            OpenMenu(row);
            var headers = MenuOf(row)!.Items.OfType<MenuItem>().Select(menuItem => menuItem.Header).ToList();

            Assert.Equal(
                new object[] { ((IPrefab)item).PrefabService.Name.Value, "-", "Duplicate", "Rename", "-", "Delete Everywhere" },
                headers);
        }

        [AvaloniaFact]
        public void Menu_UsesTheSharedPopupTheme()
        {
            var screen = Show(typeof(Entity), 1);
            var row = screen.Rows.Single();

            OpenMenu(row);

            Assert.Same(Application.Current!.FindResource("ContextMenuDefault"), MenuOf(row)!.Theme);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Duplicate_OnARowOfAnItemThatLivesInALayer_MakesACopyInTheProjectAsOneUndoStep(Type kind)
        {
            var screen = Show(kind);
            var inALayer = AddToALayer(screen);
            var before = screen.ItemsOfTheProject.Count();

            Click(screen.RowOf(inALayer), "Duplicate");

            Assert.Equal(before + 1, screen.ItemsOfTheProject.Count());
            var copy = screen.Manager.ManagerData.Items.Last();
            Assert.NotEqual(inALayer.ID, copy.ID);
            Assert.IsType(kind, copy);

            screen.Undo.Undo();

            Assert.Equal(before, screen.ItemsOfTheProject.Count());
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Duplicate_OnARowOfTheListOfTheTab_PutsTheCopyRightAfterIt(Type kind)
        {
            var screen = Show(kind);
            var items = screen.Manager.ManagerData.Items.ToList();

            Click(screen.RowOf(items[0]), "Duplicate");

            Assert.Equal(4, screen.Manager.ManagerData.Items.Count);
            Assert.Same(items[0], screen.Manager.ManagerData.Items[0]);
            Assert.Same(items[1], screen.Manager.ManagerData.Items[2]);
            Assert.NotEqual(items[0].ID, screen.Manager.ManagerData.Items[1].ID);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void DeleteEverywhere_TakesTheItemOutOfEveryListAsOneUndoStep(Type kind)
        {
            var screen = Show(kind);
            var inALayer = AddToALayer(screen);
            screen.Manager.AddExistingItem(inALayer);
            TestServiceProviderFactory.Pump();
            screen.Undo.Clear();
            var layerList = screen.ViewModel.ControlRepository.GetCollectionManagers(inALayer);
            Assert.Equal(2, layerList.Count);

            Click(screen.RowOf(inALayer), "Delete Everywhere");

            Assert.Empty(screen.ViewModel.ControlRepository.GetCollectionManagers(inALayer));
            Assert.DoesNotContain(inALayer, screen.ItemsOfTheProject);

            screen.Undo.Undo();

            Assert.Equal(2, screen.ViewModel.ControlRepository.GetCollectionManagers(inALayer).Count);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Rename_StartsTheEditModeOfTheNameBox(Type kind)
        {
            var screen = Show(kind, 1);
            var row = screen.Rows.Single();

            Click(row, "Rename");

            Assert.True(row.GetVisualDescendants().OfType<TextBoxEditable>().Single().IsEditing);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void RightClickOnAnUnselectedRow_SelectsItWhenThePressStarts_AndOpensTheMenuWhenItEnds(Type kind)
        {
            var screen = Show(kind);
            var items = screen.Manager.ManagerData.Items.ToList();
            Assert.Same(items[2], screen.Manager.SelectedItem);
            var row = screen.RowOf(items[0]);
            var point = screen.CenterOf(row);

            screen.Window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
            screen.Window.MouseDown(point, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.Same(items[0], screen.Manager.SelectedItem);
            Assert.False(MenuOf(row)?.IsOpen ?? false);

            screen.Window.MouseUp(point, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.True(MenuOf(row)!.IsOpen);
            Assert.False(row.GetVisualDescendants().OfType<TextBoxEditable>().Single().IsEditing);
        }
    }
}
