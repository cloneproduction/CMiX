using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Adding or deleting a global address changes the gains of every modulator, in one undo step.
    public class RedisAddressRegistryGainTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private UndoManager Undo => _provider.GetRequiredService<UndoManager>();

        private Blur NewBlur() => (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));

        private static RedisAddressModulator AddModulator(PrefabManager manager)
        {
            manager.AddItem(typeof(RedisAddressModulator));
            return (RedisAddressModulator)manager.SelectedItem;
        }

        [Fact]
        public void AddingAnAddress_GivesAGainToEveryModulator()
        {
            var first = AddModulator(NewBlur().ModulatorManager);
            var second = AddModulator(NewBlur().ModulatorManager);

            Registry.AddAddress();

            Assert.Single(first.Gains);
            Assert.Single(second.Gains);
            Assert.Equal(Registry.Addresses.Single().ID, first.Gains.Single().AddressID.Value);
        }

        [Fact]
        public void RemovingAnAddress_TakesAwayItsGainInEveryModulator()
        {
            var first = AddModulator(NewBlur().ModulatorManager);
            var second = AddModulator(NewBlur().ModulatorManager);
            Registry.AddAddress();

            Registry.RemoveAddress(Registry.Addresses.Single());

            Assert.Empty(first.Gains);
            Assert.Empty(second.Gains);
        }

        [Fact]
        public void OneUndo_TakesBackTheAddedAddressAndItsGains()
        {
            var modulator = AddModulator(NewBlur().ModulatorManager);
            Undo.Clear();

            Registry.AddAddress();
            Undo.Undo();

            Assert.Empty(Registry.Addresses);
            Assert.Empty(modulator.Gains);
        }

        [Fact]
        public void OneUndo_BringsBackTheRemovedAddressWithTheValueOfItsGain()
        {
            var modulator = AddModulator(NewBlur().ModulatorManager);
            Registry.AddAddress();
            modulator.Gains.Single().Value = 2.5f;
            Undo.Clear();

            Registry.RemoveAddress(Registry.Addresses.Single());
            Undo.Undo();

            Assert.Single(Registry.Addresses);
            Assert.Equal(2.5f, modulator.Gains.Single().Value);
            Assert.Equal(Registry.Addresses.Single().ID, modulator.Gains.Single().AddressID.Value);
        }

        [Fact]
        public void AModulatorThatWasRemoved_IsLeftAlone()
        {
            var blur = NewBlur();
            var modulator = AddModulator(blur.ModulatorManager);
            blur.ModulatorManager.RemoveItem(modulator);

            Registry.AddAddress();

            Assert.Empty(modulator.Gains);
        }
    }
}
