using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Pulse filter view: three modulatable values, a channel combo and an invert toggle.
    public class PulseViewTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private (Pulse Pulse, Views.Pulse View) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var pulse = (Pulse)provider.GetRequiredService<ControlFactory>().Create(typeof(Pulse));

            var view = new Views.Pulse { DataContext = pulse };
            var window = windows.Add(new Window { Content = view, Width = 500, Height = 700 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (pulse, view);
        }

        private static DragValue CenterDragValue(Views.Pulse view, Pulse pulse) =>
            view.GetVisualDescendants().OfType<DragValue>().Single(d => d.DataContext == pulse.Center);

        [AvaloniaFact]
        public void Show_CenterRow_HasAModulatorAssignButtonAtItsRight()
        {
            var (pulse, view) = Show();

            var dragValue = CenterDragValue(view, pulse);
            var assign = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single(a => a.DataContext == pulse.Center);

            Assert.Same(pulse.ModulatorManager, assign.ModulatorManager);
            Assert.True(assign.TranslatePoint(new Point(0, 0), view)!.Value.X > dragValue.TranslatePoint(new Point(0, 0), view)!.Value.X);
        }

        [AvaloniaFact]
        public void Show_ChannelComboBox_ListsTheEightChannelsInShaderOrder()
        {
            var (_, view) = Show();

            var combo = view.GetVisualDescendants().OfType<ComboBoxCaptioned>().Single(c => c.Caption == "Channel");

            Assert.Equal(
                new[]
                {
                    PulseChannel.Red, PulseChannel.Green, PulseChannel.Blue, PulseChannel.Hue,
                    PulseChannel.Saturation, PulseChannel.Value, PulseChannel.Lightness, PulseChannel.Alpha
                },
                combo.ItemsSource!.Cast<PulseChannel>().ToArray());
            Assert.Equal(PulseChannel.Red, combo.SelectedItem);
        }

        [AvaloniaFact]
        public void Reset_InvertToggle_RestoresTheModelDefault()
        {
            var (pulse, view) = Show();
            var toggle = view.GetVisualDescendants().OfType<ToggleButtonCaptioned>().Single(t => t.Caption == "Invert");
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(pulse.Invert.Value);

            toggle.ContextMenu!.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "Reset")).Command!.Execute(null);

            Assert.False(pulse.Invert.Value);
        }

        [AvaloniaFact]
        public void Bind_CenterRow_IsReadOnlyWhileAModulatorIsBound()
        {
            var (pulse, view) = Show();
            var dragValue = CenterDragValue(view, pulse);
            Assert.False(dragValue.IsReadOnly);

            pulse.ModulatorManager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)pulse.ModulatorManager.ManagerData.Items[0];
            pulse.Center.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            TestServiceProviderFactory.Pump();

            Assert.True(dragValue.IsReadOnly);
        }
    }
}
