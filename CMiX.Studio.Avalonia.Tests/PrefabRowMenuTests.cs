using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The menu of a row in the lists of prefabs. Each kind of list gets its own rows.
    public class PrefabRowMenuTests
    {
        private static (PrefabManager Manager, ListBox List) Show(Control view, PrefabManager manager, int itemCount)
        {
            new Window { Content = view, Width = 500, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
            for (var i = 0; i < itemCount; i++)
                manager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            return (manager, view.GetVisualDescendants().OfType<ListBox>().First());
        }

        private static (PrefabManager Manager, ListBox List) ShowSlotList(RowMenuKind kind, int itemCount)
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            return Show(new PrefabSlotManager { DataContext = manager, RowMenu = kind }, manager, itemCount);
        }

        private static (PrefabManager Manager, ListBox List) ShowRepositoryList(RowMenuKind kind, int itemCount)
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            return Show(new RepositoryManager { DataContext = manager, ItemsSource = manager.ManagerData.Items, RowMenu = kind }, manager, itemCount);
        }

        private static Control RowAt(ListBox list, int index) => list.ContainerFromIndex(index)!;

        // The right click of a user raises ContextRequested on the row.
        private static List<MenuItem> OpenMenu(Control row)
        {
            row.RaiseEvent(new ContextRequestedEventArgs { Source = row });
            Dispatcher.UIThread.RunJobs();
            return row.ContextMenu!.Items.OfType<MenuItem>().ToList();
        }

        private static object?[] HeadersOf(IEnumerable<MenuItem> rows) => rows.Select(row => row.Header).ToArray();

        private static string NameAt(PrefabManager manager, int index) =>
            ((IPrefab)manager.ManagerData.Items[index]).PrefabService.Name.Value;

        [AvaloniaFact]
        public void SlotMenu_StartsWithTheNameAndListsDuplicateRenameMovesAndRemove()
        {
            var (manager, list) = ShowSlotList(RowMenuKind.Slot, 2);

            var rows = OpenMenu(RowAt(list, 0));

            Assert.Equal(
                new object[] { NameAt(manager, 0), "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Remove" },
                HeadersOf(rows));
        }

        [AvaloniaFact]
        public void SlotMenu_DisablesTheMovesThatCannotMoveTheItem()
        {
            var (_, list) = ShowSlotList(RowMenuKind.Slot, 3);

            var first = OpenMenu(RowAt(list, 0));
            var middle = OpenMenu(RowAt(list, 1));
            var last = OpenMenu(RowAt(list, 2));

            static bool IsEnabled(List<MenuItem> rows, string header) => rows.Single(row => Equals(row.Header, header)).IsEnabled;

            Assert.False(IsEnabled(first, "Move Up"));
            Assert.False(IsEnabled(first, "Move To Top"));
            Assert.True(IsEnabled(first, "Move Down"));
            Assert.True(IsEnabled(first, "Move To Bottom"));

            Assert.True(IsEnabled(middle, "Move Up"));
            Assert.True(IsEnabled(middle, "Move Down"));

            Assert.True(IsEnabled(last, "Move Up"));
            Assert.False(IsEnabled(last, "Move Down"));
            Assert.False(IsEnabled(last, "Move To Bottom"));
        }

        [AvaloniaFact]
        public void SlotMenu_MoveDown_MovesTheItemOfTheRowAndNotTheSelectedItem()
        {
            var (manager, list) = ShowSlotList(RowMenuKind.Slot, 3);
            var item = manager.ManagerData.Items[0];
            manager.SelectAt(2);

            OpenMenu(RowAt(list, 0)).Single(row => Equals(row.Header, "Move Down"))
                .RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(MenuItem.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, manager.ManagerData.Items.IndexOf(item));
        }

        [AvaloniaFact]
        public void RepositoryMenu_StartsWithTheNameAndListsDuplicateRenameAndDeleteEverywhere()
        {
            var (manager, list) = ShowRepositoryList(RowMenuKind.Repository, 2);

            var rows = OpenMenu(RowAt(list, 0));

            Assert.Equal(
                new object[] { NameAt(manager, 0), "-", "Duplicate", "Rename", "-", "Delete Everywhere" },
                HeadersOf(rows));
        }

        [AvaloniaFact]
        public void RowMenuNone_GivesTheRowsNoMenu()
        {
            var (_, slotList) = ShowSlotList(RowMenuKind.None, 1);
            var (_, repositoryList) = ShowRepositoryList(RowMenuKind.None, 1);

            Assert.Null(RowAt(slotList, 0).ContextMenu);
            Assert.Null(RowAt(repositoryList, 0).ContextMenu);
        }
    }
}
