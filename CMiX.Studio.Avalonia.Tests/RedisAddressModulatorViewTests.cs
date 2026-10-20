using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewRedisAddressModulator = CMiX.Studio.Avalonia.Views.RedisAddressModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list of a RedisAddressModulator in the window.
    public class RedisAddressModulatorViewTests
    {
        private static (RedisAddressModulator Modulator, ViewRedisAddressModulator View) Show(int addressCount)
        {
            var provider = TestServiceProviderFactory.Create();
            var modulator = (RedisAddressModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(RedisAddressModulator));
            var view = new ViewRedisAddressModulator { DataContext = modulator };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            for (var i = 0; i < addressCount; i++)
                modulator.AddressManager.AddItem(typeof(RedisAddress));
            Dispatcher.UIThread.RunJobs();
            return (modulator, view);
        }

        [AvaloniaFact]
        public void EachAddress_IsAClickToEditText()
        {
            var (_, view) = Show(2);

            Assert.Equal(2, view.GetVisualDescendants().OfType<TextBoxEditable>().Count());
        }

        [AvaloniaFact]
        public void TheListHasMoveUpAndMoveDownCommands()
        {
            var (_, view) = Show(2);
            var list = view.GetVisualDescendants().OfType<ListBoxEditable>().Single();

            Assert.NotNull(list.MoveUpCommand);
            Assert.NotNull(list.MoveDownCommand);
        }

        [AvaloniaFact]
        public void MovingAnAddressDown_ChangesTheOrderOfTheOutputs()
        {
            var (modulator, view) = Show(0);
            foreach (var text in new[] { "cmix:a", "cmix:b" })
            {
                modulator.AddressManager.AddItem(typeof(RedisAddress));
                ((RedisAddress)modulator.AddressManager.SelectedItem).Value = text;
            }
            var list = view.GetVisualDescendants().OfType<ListBoxEditable>().Single();

            list.MoveDownCommand!.Execute(modulator.AddressManager.ManagerData.Items[0]);

            Assert.Equal(new[] { "cmix:b", "cmix:a" },
                modulator.Outputs.Cast<AddressedOutput>().Select(output => output.Address));
        }
    }
}
