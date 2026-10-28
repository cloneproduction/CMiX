using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Removing a global address takes the values bound to the modulators that select it back to normal.
    public class RedisAddressRemoveTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private RedisAddress AddAddress(string text)
        {
            Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        private static RedisAddressModulator AddModulator(PrefabManager manager)
        {
            manager.AddItem(typeof(RedisAddressModulator));
            return manager.ManagerData.Items.OfType<RedisAddressModulator>().Single();
        }

        // Selects the address on the modulator and binds the value to its output.
        private static void Bind(IModulatorBindable bindable, RedisAddressModulator modulator, RedisAddress address)
        {
            modulator.SelectedAddress = address;
            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs.Single()));
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfAFilter_TakesTheValueBackToNormal()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);

            Registry.RemoveAddress(address);

            Assert.Null(blur.Strength.ModulatorID);
            Assert.False(blur.Strength.IsModulated);
            Assert.Null(modulator.SelectedAddress);
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfAModifier_TakesTheValueBackToNormal()
        {
            var scale = (ScaleModifier)_provider.GetRequiredService<ControlFactory>().Create(typeof(ScaleModifier));
            var modulator = AddModulator(scale.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(scale.X, modulator, address);

            Registry.RemoveAddress(address);

            Assert.Null(scale.X.ModulatorID);
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfATextureSource_TakesTheValueBackToNormal()
        {
            var source = (VideoPlayer)_provider.GetRequiredService<ControlFactory>().Create(typeof(VideoPlayer));
            var modulator = AddModulator(source.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(source.SeekTarget, modulator, address);

            Registry.RemoveAddress(address);

            Assert.Null(source.SeekTarget.ModulatorID);
        }

        [Fact]
        public void RemoveAddress_AnotherAddress_KeepsTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var removed = AddAddress("cmix:a");
            var kept = AddAddress("cmix:b");
            Bind(blur.Strength, modulator, kept);

            Registry.RemoveAddress(removed);

            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal("Value", blur.Strength.BoundOutputName);
            Assert.Same(kept, modulator.SelectedAddress);
        }

        [Fact]
        public void EditingOrMovingTheAddress_KeepsTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var first = AddAddress("cmix:a");
            AddAddress("cmix:b");
            Bind(blur.Strength, modulator, first);

            first.Value = "cmix:changed";
            Registry.AddressManager.MoveItemTo(first, 1);

            Assert.Equal("Value", blur.Strength.BoundOutputName);
            Assert.True(blur.Strength.IsModulated);
            Assert.Same(first, modulator.SelectedAddress);
        }

        [Fact]
        public void ChangingTheSelection_KeepsTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var first = AddAddress("cmix:a");
            var second = AddAddress("cmix:b");
            Bind(blur.Strength, modulator, first);

            modulator.SelectedAddress = second;

            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal("Value", blur.Strength.BoundOutputName);
            Assert.True(blur.Strength.IsModulated);
        }

        [Fact]
        public void RemoveAddress_AfterTheModulatorIsRemoved_StillWorks()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);
            blur.ModulatorManager.RemoveItem(modulator);

            Registry.RemoveAddress(address);

            Assert.Empty(Registry.Addresses);
            Assert.Null(blur.Strength.ModulatorID);
        }

        [Fact]
        public void Undo_OfRemoveAddress_BringsBackTheAddressTheSelectionAndTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);
            _provider.GetRequiredService<UndoManager>().Clear();

            Registry.RemoveAddress(address);
            _provider.GetRequiredService<UndoManager>().Undo();

            Assert.Single(Registry.Addresses);
            Assert.Equal(address.ID, modulator.SelectedAddressID.Value);
            Assert.NotNull(modulator.SelectedAddress);
            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal("Value", blur.Strength.BoundOutputName);
        }
    }
}
