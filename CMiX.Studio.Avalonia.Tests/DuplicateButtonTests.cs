using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Duplicate button sits next to the Add button of the layer and composition list, and before the Remove button of a slot.
    public class DuplicateButtonTests
    {
        private static PrefabManager CreateManager() => TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();

        private static void Show(Control view)
        {
            new Window { Content = view, Width = 500, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        private static Button ButtonIn(Control view, string name) =>
            view.GetVisualDescendants().OfType<Button>().Single(button => button.Name == name);

        private static double LeftOf(Control control, Control view) => control.TranslatePoint(new Point(0, 0), view)!.Value.X;

        private static string? IconOf(Button button) => (button.Content as AppIcon)?.IconKey;

        [AvaloniaFact]
        public void ToolbarDuplicateButton_ShowsTheDuplicateIcon_AndIsDisabledWithoutItems()
        {
            var manager = CreateManager();
            var view = new Views.ComposableManagerToolbar { DataContext = manager };
            Show(view);
            var button = ButtonIn(view, "DuplicateButton");

            Assert.Equal("Duplicate", IconOf(button));
            Assert.False(button.IsEnabled);

            manager.AddItem(typeof(Layer));
            Dispatcher.UIThread.RunJobs();

            Assert.True(button.IsEnabled);
        }

        [AvaloniaFact]
        public void ToolbarDuplicateButton_SitsRightNextToTheAddButton()
        {
            var manager = CreateManager();
            manager.AddItem(typeof(Layer));
            var view = new Views.ComposableManagerToolbar { DataContext = manager };
            Show(view);

            var add = view.GetVisualDescendants().OfType<Button>().First(button => (button.Content as AppIcon)?.IconKey == "Add");
            var duplicate = ButtonIn(view, "DuplicateButton");
            var moveUp = view.GetVisualDescendants().OfType<Button>().First(button => (button.Content as AppIcon)?.IconKey == "ArrowUp");

            Assert.True(LeftOf(add, view) < LeftOf(duplicate, view));
            Assert.True(LeftOf(duplicate, view) < LeftOf(moveUp, view));
        }

        [AvaloniaFact]
        public void ToolbarDuplicateButton_DuplicatesTheSelectedItem()
        {
            var manager = CreateManager();
            manager.AddItem(typeof(Layer));
            var view = new Views.ComposableManagerToolbar { DataContext = manager };
            Show(view);
            var button = ButtonIn(view, "DuplicateButton");

            button.Command!.Execute(button.CommandParameter);

            Assert.Equal(2, manager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void SlotDuplicateButton_ShowsTheDuplicateIcon_AndIsDisabledWithoutItems()
        {
            var manager = CreateManager();
            var view = new PrefabSlotManager { DataContext = manager };
            Show(view);
            var button = ButtonIn(view, "DuplicateButton");

            Assert.Equal("Duplicate", IconOf(button));
            Assert.False(button.IsEnabled);

            manager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            Assert.True(button.IsEnabled);
        }

        [AvaloniaFact]
        public void SlotDuplicateButton_SitsBeforeTheRemoveButton()
        {
            var manager = CreateManager();
            var view = new PrefabSlotManager { DataContext = manager };
            Show(view);

            var duplicate = ButtonIn(view, "DuplicateButton");
            var remove = ButtonIn(view, "RemoveButton");
            var moveUp = ButtonIn(view, "MoveUpButton");

            Assert.True(LeftOf(moveUp, view) < LeftOf(duplicate, view));
            Assert.True(LeftOf(duplicate, view) < LeftOf(remove, view));
        }

        [AvaloniaFact]
        public void SlotDuplicateButton_DuplicatesTheSelectedItem()
        {
            var manager = CreateManager();
            var view = new PrefabSlotManager { DataContext = manager };
            Show(view);
            manager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            var button = ButtonIn(view, "DuplicateButton");

            button.Command!.Execute(button.CommandParameter);

            Assert.Equal(2, manager.ManagerData.Items.Count);
        }
    }
}
