using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Views;
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
    // The context menu of a row in the lists of entities, cameras and lights of a layer.
    // It acts on the row that owns it, as the menu of the layer list does.
    public class SlotRowMenuTests
    {
        public static IEnumerable<object[]> Kinds() =>
            new[] { typeof(Entity), typeof(Camera), typeof(LightEntity) }.Select(kind => new object[] { kind });

        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public Layer Layer { get; init; } = null!;
            public PrefabManager Manager { get; init; } = null!;
            public ListBoxCMiX List { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;

            public List<ListItemIcon> Rows => List.GetVisualDescendants().OfType<ListItemIcon>().ToList();

            public Point CenterOf(Control control) =>
                control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), Window)!.Value;
        }

        private static PrefabManager ManagerOf(Layer layer, Type kind) =>
            kind == typeof(Camera) ? layer.CameraManager
            : kind == typeof(LightEntity) ? layer.LightManager
            : layer.ModelEntityManager;

        private static Screen Show(Type kind, int count = 3, IServiceProvider? provider = null)
        {
            provider ??= TestServiceProviderFactory.Create();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var manager = ManagerOf(layer, kind);
            for (var i = 0; i < count; i++)
                manager.AddItem(kind);

            Control tab = kind == typeof(Camera) ? new LayerCameraTab { DataContext = layer }
                : kind == typeof(LightEntity) ? new LayerLightTab { DataContext = layer }
                : new LayerEntityTab { DataContext = layer };
            var window = new Window { Content = tab, Width = 500, Height = 900 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Screen
            {
                Window = window,
                Layer = layer,
                Manager = manager,
                List = tab.GetVisualDescendants().OfType<ListBoxCMiX>().First(),
                Undo = undo
            };
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
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Menu_StartsWithTheNameOfTheItemAndALine_ThenListsTheSlotRows(Type kind)
        {
            var screen = Show(kind, 1);
            var name = ((IPrefab)screen.Manager.ManagerData.Items[0]).PrefabService.Name.Value;
            var row = screen.Rows.Single();

            OpenMenu(row);
            var headers = MenuOf(row)!.Items.OfType<MenuItem>().Select(item => item.Header).ToList();

            Assert.Equal(
                new object[] { name, "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" },
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
        public void RightClickOnAnUnselectedRow_SelectsItWhenThePressStarts_AndOpensTheMenuWhenItEnds(Type kind)
        {
            var screen = Show(kind);
            var rows = screen.Rows;
            Assert.Equal(2, screen.Manager.ManagerData.SelectedIndex);
            var point = screen.CenterOf(rows[0]);

            screen.Window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
            screen.Window.MouseDown(point, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0, screen.Manager.ManagerData.SelectedIndex);
            Assert.False(MenuOf(rows[0])?.IsOpen ?? false);

            screen.Window.MouseUp(point, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.True(MenuOf(rows[0])!.IsOpen);
            Assert.False(rows[0].GetVisualDescendants().OfType<TextBoxEditable>().Single().IsEditing);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Duplicate_OnARowThatIsNotSelected_CopiesThatRowAsOneUndoStep(Type kind)
        {
            var screen = Show(kind);
            var items = screen.Manager.ManagerData.Items.ToList();
            screen.Manager.SelectAt(2);
            screen.Undo.Clear();

            Click(screen.Rows[0], "Duplicate");

            Assert.Equal(4, screen.Manager.ManagerData.Items.Count);
            Assert.Same(items[0], screen.Manager.ManagerData.Items[0]);
            Assert.NotEqual(items[0].ID, screen.Manager.ManagerData.Items[1].ID);
            Assert.IsType(kind, screen.Manager.ManagerData.Items[1]);

            screen.Undo.Undo();

            Assert.Equal(items, screen.Manager.ManagerData.Items);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Move_Rows_MoveTheItemAndStopAtTheEdges(Type kind)
        {
            var screen = Show(kind);
            var items = screen.Manager.ManagerData.Items.ToList();
            var rows = screen.Rows;

            OpenMenu(rows[0]);
            Assert.False(Row(rows[0], "Move Up").IsEnabled);
            Assert.False(Row(rows[0], "Move To Top").IsEnabled);
            Assert.True(Row(rows[0], "Move Down").IsEnabled);
            MenuOf(rows[0])!.Close();

            OpenMenu(rows[2]);
            Assert.False(Row(rows[2], "Move Down").IsEnabled);
            Assert.False(Row(rows[2], "Move To Bottom").IsEnabled);
            MenuOf(rows[2])!.Close();

            Click(rows[0], "Move To Bottom");

            Assert.Equal(new[] { items[1], items[2], items[0] }, screen.Manager.ManagerData.Items);
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

        [AvaloniaFact]
        public void Remove_OnARow_TakesThatItemOutOfThisLayerOnly()
        {
            var provider = TestServiceProviderFactory.Create();
            var screen = Show(typeof(Entity), 3, provider);
            var items = screen.Manager.ManagerData.Items.ToList();
            var otherLayer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            otherLayer.ModelEntityManager.AddExistingItem(items[1]);

            Click(screen.Rows[1], "Remove");

            Assert.Equal(new[] { items[0], items[2] }, screen.Manager.ManagerData.Items);
            Assert.Same(items[1], Assert.Single(otherLayer.ModelEntityManager.ManagerData.Items));
        }

        [AvaloniaFact]
        public void ARightClickBelowTheRows_OpensNoMenu()
        {
            var screen = Show(typeof(Entity), 1);
            var row = screen.Rows.Single();
            var below = screen.CenterOf(row) + new Point(0, row.Bounds.Height * 1.5);

            screen.Window.MouseMove(below);
            Dispatcher.UIThread.RunJobs();
            screen.Window.MouseDown(below, MouseButton.Right);
            screen.Window.MouseUp(below, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();

            Assert.False(MenuOf(row)?.IsOpen ?? false);
        }
    }
}
