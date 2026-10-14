using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Behaviors;
using CMiX.UI.Controls.Collection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The PanelItem of the library knows no modulator. This behavior tells the modulator when the pointer is over it.
    public class ModulatorHoverBehaviorTests
    {
        private static (Window window, PanelItem item) Show(object? model)
        {
            var item = new PanelItem { Title = "panel", DataContext = model, Width = 300, Height = 60 };
            global::Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(item).Add(new ModulatorHoverBehavior());
            var window = new Window { Content = new StackPanel { Children = { item } }, Width = 400, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, item);
        }

        private static FFTModulator NewModulator()
        {
            var blur = (Blur)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));
            return blur.ModulatorManager.ManagerData.Items.OfType<FFTModulator>().Single();
        }

        private static Point Inside(Window window, PanelItem item) =>
            item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), window)!.Value;

        [AvaloniaFact]
        public void WhileThePointerIsOverThePanel_TheModulatorIsHovered()
        {
            var modulator = NewModulator();
            var (window, item) = Show(modulator);

            window.MouseMove(Inside(window, item));
            Dispatcher.UIThread.RunJobs();
            Assert.True(modulator.IsHovered);

            window.MouseMove(new Point(390, 190));
            Dispatcher.UIThread.RunJobs();
            Assert.False(modulator.IsHovered);
        }

        [AvaloniaFact]
        public void APanelWithAnotherModel_IsLeftAlone()
        {
            var (window, item) = Show(new object());

            window.MouseMove(Inside(window, item));
            Dispatcher.UIThread.RunJobs();

            Assert.True(item.IsHovered);
        }
    }
}
