using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The context menu of a row in the layer list and the composition list. It acts on the row that owns it.
    public class ComposableManagerItemMenuTests
    {
        private static (PrefabManager Manager, List<ComposableManagerItem> Rows, Window Window) Show(int layerCount = 3)
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var manager = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < layerCount; i++)
                manager.AddItem(typeof(Layer));

            var view = new ComposableManager { ItemType = typeof(Layer), DataContext = manager };
            var window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            return (manager, view.GetVisualDescendants().OfType<ComposableManagerItem>().ToList(), window);
        }

        private static MenuItem Row(ComposableManagerItem item, string header) =>
            item.ContextMenu!.Items.OfType<MenuItem>().Single(row => Equals(row.Header, header));

        // The right click of a user raises ContextRequested on the row.
        private static void OpenMenu(ComposableManagerItem item)
        {
            item.RaiseEvent(new ContextRequestedEventArgs { Source = item });
            Dispatcher.UIThread.RunJobs();
        }

        private static void Click(ComposableManagerItem item, string header)
        {
            OpenMenu(item);
            Row(item, header).RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Menu_StartsWithTheNameOfTheItemAndALine_ThenListsTheRows()
        {
            var (manager, rows, _) = Show(1);
            var name = ((Layer)manager.ManagerData.Items[0]).PrefabService.Name.Value;

            OpenMenu(rows[0]);
            var headers = rows[0].ContextMenu!.Items.OfType<MenuItem>().Select(row => row.Header).ToList();

            Assert.Equal(
                new object[] { name, "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Delete" },
                headers);
            Assert.False(Row(rows[0], name).IsEnabled);
        }

        [AvaloniaFact]
        public void Menu_UsesTheSharedPopupTheme()
        {
            var (_, rows, _) = Show(1);

            Assert.Same(Application.Current!.FindResource("ContextMenuDefault"), rows[0].ContextMenu!.Theme);
        }

        [AvaloniaFact]
        public void Title_FollowsARename()
        {
            var (manager, rows, _) = Show(1);
            ((Layer)manager.ManagerData.Items[0]).PrefabService.Name.Value = "Hero";

            OpenMenu(rows[0]);

            Assert.Equal("Hero", rows[0].ContextMenu!.Items.OfType<MenuItem>().First().Header);
        }

        [AvaloniaFact]
        public void TheFirstTime_DuplicateIsUnderThePointer_AndThenTheRowUsedLast()
        {
            var (_, rows, _) = Show();
            OpenMenu(rows[0]);
            Assert.True(MenuAnchor.GetIsAnchor(Row(rows[0], "Duplicate")));

            Click(rows[0], "Move To Bottom");
            OpenMenu(rows[1]);

            Assert.True(MenuAnchor.GetIsAnchor(Row(rows[1], "Move To Bottom")));
            Assert.False(MenuAnchor.GetIsAnchor(Row(rows[1], "Duplicate")));
        }

        [AvaloniaFact]
        public void Duplicate_OnARowThatIsNotSelected_CopiesThatRow()
        {
            var (manager, rows, _) = Show();
            var first = manager.ManagerData.Items[0];
            manager.SelectAt(2);

            Click(rows[0], "Duplicate");

            Assert.Equal(4, manager.ManagerData.Items.Count);
            Assert.Same(first, manager.ManagerData.Items[0]);
            Assert.NotEqual(first.ID, manager.ManagerData.Items[1].ID);
        }

        [AvaloniaFact]
        public void Delete_OnARow_RemovesThatRow()
        {
            var (manager, rows, _) = Show();
            var items = manager.ManagerData.Items.ToList();

            Click(rows[1], "Delete");

            Assert.Equal(new[] { items[0], items[2] }, manager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Move_Rows_MoveTheItemAndStopAtTheEdges()
        {
            var (manager, rows, _) = Show();
            var items = manager.ManagerData.Items.ToList();

            OpenMenu(rows[0]);
            Assert.False(Row(rows[0], "Move Up").IsEnabled);
            Assert.False(Row(rows[0], "Move To Top").IsEnabled);
            Assert.True(Row(rows[0], "Move Down").IsEnabled);
            rows[0].ContextMenu!.Close();

            OpenMenu(rows[2]);
            Assert.False(Row(rows[2], "Move Down").IsEnabled);
            Assert.False(Row(rows[2], "Move To Bottom").IsEnabled);
            rows[2].ContextMenu!.Close();

            Click(rows[0], "Move To Bottom");
            Assert.Equal(new[] { items[1], items[2], items[0] }, manager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Rename_StartsTheEditModeOfTheNameBox()
        {
            var (_, rows, _) = Show(1);

            Click(rows[0], "Rename");

            Assert.True(rows[0].GetVisualDescendants().OfType<TextBoxEditable>().Single().IsEditing);
        }
    }
}
