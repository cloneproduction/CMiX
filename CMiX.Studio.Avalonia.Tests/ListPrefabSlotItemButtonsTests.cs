using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The remove, move up and move down buttons of the slot act on the items inside it.
    public class ListPrefabSlotItemButtonsTests
    {
        private static readonly string[] ButtonNames = { "removeButton", "moveUpButton", "moveDownButton" };

        private static (ListPrefabSlot View, PrefabManager Manager) Show()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            var view = new ListPrefabSlot { DataContext = manager };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (view, manager);
        }

        private static List<bool> EnabledStates(ListPrefabSlot view) =>
            ButtonNames.Select(name => view.FindControl<Button>(name)!.IsEnabled).ToList();

        [AvaloniaFact]
        public void Show_AnEmptySlot_DisablesRemoveMoveUpAndMoveDown()
        {
            var (view, _) = Show();

            Assert.All(EnabledStates(view), enabled => Assert.False(enabled));
        }

        [AvaloniaFact]
        public void AddItem_ToAnEmptySlot_EnablesRemoveMoveUpAndMoveDown()
        {
            var (view, manager) = Show();

            manager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            Assert.All(EnabledStates(view), enabled => Assert.True(enabled));
        }

        [AvaloniaFact]
        public void RemoveItem_TheLastItemOfASlot_DisablesTheButtonsAgain()
        {
            var (view, manager) = Show();
            manager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            manager.RemoveItem(manager.ManagerData.Items[0]);
            Dispatcher.UIThread.RunJobs();

            Assert.All(EnabledStates(view), enabled => Assert.False(enabled));
        }
    }
}
