using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The modifier list of the Studio shows one PanelItem for each item of its manager.
    public class ModifierManagerTests
    {
        private static (PrefabManager manager, Window window, Views.ModifierManager view) Show(int modulators = 2)
        {
            var blur = (Blur)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Blur));
            for (var i = 0; i < modulators; i++)
                blur.ModulatorManager.AddItem(typeof(FFTModulator));

            var view = new Views.ModifierManager { DataContext = blur.ModulatorManager, Caption = "Add Modulator" };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (blur.ModulatorManager, window, view);
        }

        private static List<PanelItem> Items(Control view) => view.GetVisualDescendants().OfType<PanelItem>().ToList();

        private static Button ButtonWithIcon(PanelItem item, string iconKey) =>
            item.GetVisualDescendants().OfType<AppIcon>().Single(i => i.IconKey == iconKey).GetVisualAncestors().OfType<Button>().First();

        private static ToggleButton EyeOf(PanelItem item) =>
            item.GetVisualDescendants().OfType<AppIcon>().Single(i => i.IconKey is "Visible" or "NotVisible")
                .GetVisualAncestors().OfType<ToggleButton>().First();

        [AvaloniaFact]
        public void ShowsOnePanelItemForEachItem_WithTheNameOfTheItem()
        {
            var (manager, _, view) = Show(2);

            var items = Items(view);

            Assert.Equal(2, items.Count);
            Assert.Equal(manager.ManagerData.Items.Cast<IModulator>().Select(m => m.PrefabService.Name.Value), items.Select(i => i.Title));
        }

        [AvaloniaFact]
        public void TheEye_WritesTheVisibilityOfTheItem()
        {
            var (manager, window, view) = Show(1);
            var modulator = manager.ManagerData.Items.Cast<IModulator>().Single();
            var before = modulator.PrefabService.Visibility.Value;

            PointerInput.Click(window, EyeOf(Items(view).Single()), MouseButton.Left);

            Assert.Equal(!before, modulator.PrefabService.Visibility.Value);
        }

        [AvaloniaFact]
        public void TheCloseButton_RemovesTheItem()
        {
            var (manager, window, view) = Show(2);

            PointerInput.Click(window, ButtonWithIcon(Items(view).First(), "Close"), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Single(manager.ManagerData.Items);
            Assert.Single(Items(view));
        }

        [AvaloniaFact]
        public void ThePointerOverAPanel_HoversItsModulator()
        {
            var (manager, window, view) = Show(2);
            var panels = Items(view);
            var modulators = manager.ManagerData.Items.Cast<IModulator>().ToList();
            var second = panels[1];

            window.MouseMove(second.TranslatePoint(new Point(second.Bounds.Width / 2, second.Bounds.Height / 2), window)!.Value);
            Dispatcher.UIThread.RunJobs();

            Assert.False(modulators[0].IsHovered);
            Assert.True(modulators[1].IsHovered);
        }

        [AvaloniaFact]
        public void AFilterAddedToAComposition_IsOpenWhenItIsCreated_AndFollowsTheModel()
        {
            var composition = (Composition)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Composition));
            composition.TextureModifierManager.AddItem(typeof(Blur));
            var view = new Views.ModifierManager { DataContext = composition.TextureModifierManager, Caption = "Add Filter" };
            new Window { Content = view, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();
            var filter = (Blur)composition.TextureModifierManager.ManagerData.Items.Single();
            var panel = Items(view).Single();

            Assert.True(filter.IsExpanded);
            Assert.True(panel.IsExpanded);

            panel.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(filter.IsExpanded);

            filter.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(panel.IsExpanded);
        }

        [AvaloniaFact]
        public void AModulator_IsOpenWhenItIsCreated_AndFollowsTheModel()
        {
            var (manager, _, view) = Show(1);
            var modulator = manager.ManagerData.Items.Cast<IModulator>().Single();
            var panel = Items(view).Single();

            Assert.True(modulator.IsExpanded);
            Assert.True(panel.IsExpanded);

            panel.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(modulator.IsExpanded);
        }

        [AvaloniaFact]
        public void ThePanelOfTheAddButton_GetsItsBindings_BeforeThePopupOpens()
        {
            var blur = (Blur)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var panel = new Button();
            panel.Bind(Button.CommandProperty, new Binding("AddItemCommand"));
            var view = new Views.ModifierManager { DataContext = blur.ModulatorManager, Caption = "Add Modulator", SelectionPanel = panel };
            new Window { Content = view, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.NotNull(panel.Command);
        }

        [AvaloniaFact]
        public void TheAddButton_OpensThePopupThatHoldsThePanel()
        {
            var blur = (Blur)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var panel = new Button { Content = "the panel" };
            var view = new Views.ModifierManager { DataContext = blur.ModulatorManager, Caption = "Add Modulator", SelectionPanel = panel };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var toggle = view.GetVisualDescendants().OfType<ToggleButton>().Single(t => Equals(t.Content, "Add Modulator"));
            var popup = view.GetVisualDescendants().OfType<Popup>().Single();
            Assert.False(popup.IsOpen);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(popup.IsOpen);
            Assert.Same(panel, ((ContentControl)((ContentControl)popup.Child!).Content!).Content);
        }
    }
}
