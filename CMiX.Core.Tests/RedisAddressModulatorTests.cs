using System.Text.Json;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The list of a RedisAddressModulator: each address is one float output.
    public class RedisAddressModulatorTests
    {
        private static RedisAddressModulator NewModulator() =>
            (RedisAddressModulator)TestServiceProviderFactory.Create()
                .GetRequiredService<ControlFactory>().Create(typeof(RedisAddressModulator));

        private static RedisAddress AddAddress(RedisAddressModulator modulator, string text)
        {
            modulator.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)modulator.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        [Fact]
        public void ItStartsWithNoOutput()
        {
            Assert.Empty(NewModulator().Outputs);
        }

        [Fact]
        public void EachAddress_IsOneFloatOutput()
        {
            var modulator = NewModulator();
            AddAddress(modulator, "cmix:a");
            AddAddress(modulator, "cmix:b");

            Assert.Equal(new[] { "cmix:a", "cmix:b" },
                modulator.Outputs.Cast<AddressedOutput>().Select(output => output.Address));
            Assert.All(modulator.Outputs, output => Assert.IsAssignableFrom<ModulatorOutput<float>>(output));
        }

        [Fact]
        public void EditingAnAddress_KeepsTheOutputName()
        {
            var modulator = NewModulator();
            var address = AddAddress(modulator, "cmix:a");
            var nameBefore = modulator.Outputs[0].Name;

            address.Value = "cmix:changed";

            Assert.Equal(nameBefore, modulator.Outputs[0].Name);
            Assert.Equal("cmix:changed", ((AddressedOutput)modulator.Outputs[0]).Address);
        }

        [Fact]
        public void DeletingAnAddress_RemovesItsOutput()
        {
            var modulator = NewModulator();
            var first = AddAddress(modulator, "cmix:a");
            AddAddress(modulator, "cmix:b");

            modulator.AddressManager.DeleteItemCommand.Execute(first);

            Assert.Equal(new[] { "cmix:b" },
                modulator.Outputs.Cast<AddressedOutput>().Select(output => output.Address));
        }

        [Fact]
        public void TheAddresses_SurviveSavingAndLoading()
        {
            var modulator = NewModulator();
            AddAddress(modulator, "cmix:a");
            AddAddress(modulator, "cmix:b");

            var json = JsonSerializer.Serialize<IControlModel>(modulator.ToModel(), ProjectSerializer.Options);
            var model = (RedisAddressModulatorModel)JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options)!;

            Assert.Equal(new[] { "cmix:a", "cmix:b" },
                model.AddressManager.ManagerData.Items.Cast<RedisAddressModel>().Select(address => address.Value));
        }

        [Fact]
        public void LoadingAModel_GivesTheSameOutputNames()
        {
            var modulator = NewModulator();
            AddAddress(modulator, "cmix:a");
            var namesBefore = modulator.Outputs.Select(output => output.Name).ToList();

            var loaded = NewModulator();
            loaded.FromModel(modulator.ToModel());

            Assert.Equal(namesBefore, loaded.Outputs.Select(output => output.Name));
        }
    }
}
