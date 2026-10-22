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
    public class PrefabSlotManagerItemButtonsTests
    {
        private static readonly string[] ButtonNames = { "RemoveButton", "MoveUpButton", "MoveDownButton" };

        private static (PrefabSlotManager View, PrefabManager Manager) Show()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            var view = new PrefabSlotManager { DataContext = manager };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (view, manager);
        }

        private static List<bool> EnabledStates(PrefabSlotManager view) =>
            ButtonNames.Select(name => view.FindControl<Button>(name)!.IsEnabled).ToList();

        [AvaloniaFact]
        public void SlotWithoutItems_DisablesRemoveMoveUpAndMoveDown()
        {
            var (view, _) = Show();

            Assert.All(EnabledStates(view), enabled => Assert.False(enabled));
        }

        [AvaloniaFact]
        public void SlotWithAnItem_EnablesRemoveMoveUpAndMoveDown()
        {
            var (view, manager) = Show();

            manager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            Assert.All(EnabledStates(view), enabled => Assert.True(enabled));
        }

        [AvaloniaFact]
        public void SlotEmptiedAgain_DisablesTheButtonsAgain()
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
