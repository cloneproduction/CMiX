using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Every button that removes an item shows the same trash can, and the Add button of a slot is not rounded.
    public class DeleteButtonLookTests
    {
        private const string TrashCan = "Remove";

        private static PrefabManager CreateManager() => TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();

        private static void Show(Control view)
        {
            new Window { Content = view, Width = 500, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        private static string? IconOf(Button button) => (button.Content as AppIcon)?.IconKey;

        private static Button RemoveButtonIn(Control view) =>
            view.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "RemoveButton");

        [AvaloniaFact]
        public void SlotRemoveButton_ShowsTheTrashCan()
        {
            var view = new PrefabSlotManager { DataContext = CreateManager() };
            Show(view);

            Assert.Equal(TrashCan, IconOf(RemoveButtonIn(view)));
        }

        [AvaloniaFact]
        public void RepositoryRemoveButton_ShowsTheTrashCan()
        {
            var manager = CreateManager();
            var view = new RepositoryManager { DataContext = manager, ItemsSource = manager.ManagerData.Items };
            Show(view);

            Assert.Equal(TrashCan, IconOf(RemoveButtonIn(view)));
        }

        [AvaloniaFact]
        public void ToolbarRemoveButton_ShowsTheTrashCan()
        {
            var view = new Views.ComposableManagerToolbar { DataContext = CreateManager() };
            Show(view);

            Assert.Equal(TrashCan, IconOf(RemoveButtonIn(view)));
        }

        [AvaloniaFact]
        public void ToolbarRemoveButton_IsDisabledWithoutItems_AndEnabledWithOne()
        {
            var manager = CreateManager();
            var view = new Views.ComposableManagerToolbar { DataContext = manager };
            Show(view);
            var button = RemoveButtonIn(view);

            Assert.False(button.IsEnabled);

            manager.AddItem(typeof(Layer));
            Dispatcher.UIThread.RunJobs();

            Assert.True(button.IsEnabled);
        }

        private static ToggleButton ToggleOf(PopupButton popup) =>
            popup.GetVisualDescendants().OfType<ToggleButton>().Single(toggle => toggle.Name == "PopUpToggle");

        [AvaloniaFact]
        public void APopupButtonOutsideTheSlot_StaysRounded()
        {
            var popup = new PopupButton { Caption = "Add Filter" };
            Show(popup);

            Assert.Equal(ControlPosition.All, PositionedControl.GetPosition(ToggleOf(popup)));
        }
    }
}
