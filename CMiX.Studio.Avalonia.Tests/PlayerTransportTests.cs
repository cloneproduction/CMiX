using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using PlayerTransport = CMiX.Studio.Avalonia.Views.PlayerTransport;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Play, Loop and Do Seek rows that the two players share.
    public class PlayerTransportTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private PlayerTransport Show(Type playerType, bool hasLoop, Action<PlayerTransport, object>? setUp = null)
        {
            var player = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(playerType);
            var transport = new PlayerTransport { DataContext = player, HasLoop = hasLoop };
            setUp?.Invoke(transport, player);
            var window = windows.Add(new Window { Content = transport, Width = 500, Height = 300 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return transport;
        }

        private static List<ControlPosition> RowPositions(PlayerTransport transport) =>
            transport.GetVisualDescendants().OfType<StackPanelPositioned>().First()
                .Children.Select(PositionedControl.GetPosition).ToList();

        [AvaloniaFact]
        public void Show_WithLoop_HasThreeRowsTopNoneBottom()
        {
            var transport = Show(typeof(SequencePlayer), hasLoop: true);

            Assert.Equal(
                new[] { ControlPosition.Top, ControlPosition.None, ControlPosition.Bottom },
                RowPositions(transport));
            Assert.Single(transport.GetVisualDescendants().OfType<ToggleButtonModulatable>(), t => t.Caption == "Loop");
        }

        [AvaloniaFact]
        public void Show_WithoutLoop_HasTwoRowsTopBottom()
        {
            var transport = Show(typeof(VideoPlayer), hasLoop: false);

            Assert.Equal(
                new[] { ControlPosition.Top, ControlPosition.Bottom },
                RowPositions(transport));
            Assert.DoesNotContain(transport.GetVisualDescendants().OfType<ToggleButtonModulatable>(), t => t.Caption == "Loop");
        }

        [AvaloniaFact]
        public void ModulatorManager_OfTheTransport_ReachesEveryAssignButton()
        {
            SequencePlayer? player = null;
            var transport = Show(typeof(SequencePlayer), hasLoop: true, (t, p) =>
            {
                player = (SequencePlayer)p;
                t.ModulatorManager = player.ModulatorManager;
            });

            var assigns = transport.GetVisualDescendants().OfType<ButtonModulatorAssign>().ToList();

            Assert.Equal(3, assigns.Count);
            Assert.All(assigns, a => Assert.Same(player!.ModulatorManager, a.ModulatorManager));
        }
    }
}
