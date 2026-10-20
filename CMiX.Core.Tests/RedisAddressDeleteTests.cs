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
    // Deleting a global address takes the values bound to it back to normal.
    public class RedisAddressDeleteTests
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

        private static void Bind(IModulatorBindable bindable, RedisAddressModulator modulator, RedisAddress address) =>
            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(
                modulator, modulator.Outputs.Single(output => output.Name == address.ID.ToString())));

        [Fact]
        public void ForAFilter_TheValueGoesBackToNormal()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);

            Registry.DeleteAddress(address);

            Assert.Null(blur.Strength.ModulatorID);
            Assert.False(blur.Strength.IsModulated);
        }

        [Fact]
        public void ForAModifier_TheValueGoesBackToNormal()
        {
            var scale = (ScaleModifier)_provider.GetRequiredService<ControlFactory>().Create(typeof(ScaleModifier));
            var modulator = AddModulator(scale.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(scale.X, modulator, address);

            Registry.DeleteAddress(address);

            Assert.Null(scale.X.ModulatorID);
        }

        [Fact]
        public void ForATextureSource_TheValueGoesBackToNormal()
        {
            var source = (VideoPlayer)_provider.GetRequiredService<ControlFactory>().Create(typeof(VideoPlayer));
            var modulator = AddModulator(source.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(source.SeekTarget, modulator, address);

            Registry.DeleteAddress(address);

            Assert.Null(source.SeekTarget.ModulatorID);
        }

        [Fact]
        public void TheValuesBoundToOtherAddresses_KeepTheirAssignment()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var deleted = AddAddress("cmix:a");
            var kept = AddAddress("cmix:b");
            Bind(blur.Strength, modulator, kept);

            Registry.DeleteAddress(deleted);

            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal(kept.ID.ToString(), blur.Strength.BoundOutputName);
        }

        [Fact]
        public void EditingOrMovingAnAddress_KeepsTheAssignment()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var first = AddAddress("cmix:a");
            AddAddress("cmix:b");
            Bind(blur.Strength, modulator, first);

            first.Value = "cmix:changed";
            Registry.AddressManager.MoveItemTo(first, 1);

            Assert.Equal(first.ID.ToString(), blur.Strength.BoundOutputName);
            Assert.True(blur.Strength.IsModulated);
        }

        [Fact]
        public void AfterTheModulatorIsRemoved_TheDeleteStillWorks()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);
            blur.ModulatorManager.DeleteItem(modulator);

            Registry.DeleteAddress(address);

            Assert.Empty(Registry.Addresses);
            Assert.Null(blur.Strength.ModulatorID);
        }

        [Fact]
        public void OneUndo_BringsBackTheAddressAndTheAssignment()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);
            _provider.GetRequiredService<UndoManager>().Clear();

            Registry.DeleteAddress(address);
            _provider.GetRequiredService<UndoManager>().Undo();

            Assert.Single(Registry.Addresses);
            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal(address.ID.ToString(), blur.Strength.BoundOutputName);
        }
    }
}
