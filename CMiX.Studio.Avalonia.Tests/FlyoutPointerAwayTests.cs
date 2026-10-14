using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The flyouts of the modulator assignment and of the sync settings close when the pointer goes far away from them.
    public class FlyoutPointerAwayTests
    {
        private static void MoveFarAway(Window window)
        {
            window.MouseMove(new Point(window.Width - 10, window.Height - 10));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void TheModulatorAssignMenu_ClosesWhenThePointerGoesFarAway()
        {
            var provider = TestServiceProviderFactory.Create();
            var model = (CMiX.Core.Texturing.Filters.Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Filters.Blur));
            model.ModulatorManager.AddItem(typeof(CMiX.Core.Modulation.Modulators.RandomModulator));
            var view = new Views.Blur { DataContext = model };
            var window = new Window { Content = view, Width = 900, Height = 900 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var assign = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single(b => b.DataContext == model.Strength);
            var button = assign.FindControl<Button>("assignButton")!;
            button.Flyout!.ShowAt(button);
            Dispatcher.UIThread.RunJobs();
            Assert.True(button.Flyout.IsOpen);

            MoveFarAway(window);

            Assert.False(button.Flyout.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("SyncButton")]
        [InlineData("EditConnectionButton")]
        public void TheSyncSettingsFlyouts_CloseWhenThePointerGoesFarAway(string buttonName)
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var view = new SyncSettings { DataContext = peer };
            var window = new Window { Content = view, Width = 1200, Height = 1000 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>(buttonName)!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();
            Assert.True(button.Flyout.IsOpen);

            MoveFarAway(window);

            Assert.False(button.Flyout.IsOpen);
            peer.Dispose();
            window.Close();
        }
    }
}
