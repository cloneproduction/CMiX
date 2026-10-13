using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
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
    }
}
