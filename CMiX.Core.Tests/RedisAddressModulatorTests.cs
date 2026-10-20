using System.Text.Json;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The outputs of a RedisAddressModulator: one float output for each address of the project.
    public class RedisAddressModulatorTests
    {
        private static (RedisAddressModulator Modulator, RedisAddressRegistry Registry, IServiceProvider Provider) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulator = (RedisAddressModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(RedisAddressModulator));
            return (modulator, provider.GetRequiredService<RedisAddressRegistry>(), provider);
        }

        private static RedisAddress AddAddress(RedisAddressRegistry registry, string text)
        {
            registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        [Fact]
        public void ItStartsWithNoOutput()
        {
            Assert.Empty(Create().Modulator.Outputs);
        }

        [Fact]
        public void EachAddress_IsOneFloatOutput()
        {
            var (modulator, registry, _) = Create();
            AddAddress(registry, "cmix:a");
            AddAddress(registry, "cmix:b");

            Assert.Equal(new[] { "cmix:a", "cmix:b" },
                modulator.Outputs.Cast<AddressedOutput>().Select(output => output.Address));
            Assert.All(modulator.Outputs, output => Assert.IsAssignableFrom<ModulatorOutput<float>>(output));
        }

        [Fact]
        public void TwoModulators_GiveTheSameOutputs()
        {
            var (first, registry, provider) = Create();
            var second = (RedisAddressModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(RedisAddressModulator));
            AddAddress(registry, "cmix:a");

            Assert.Equal(first.Outputs, second.Outputs);
        }

        [Fact]
        public void EditingAnAddress_KeepsTheOutputName()
        {
            var (modulator, registry, _) = Create();
            var address = AddAddress(registry, "cmix:a");
            var nameBefore = modulator.Outputs[0].Name;

            address.Value = "cmix:changed";

            Assert.Equal(nameBefore, modulator.Outputs[0].Name);
            Assert.Equal("cmix:changed", ((AddressedOutput)modulator.Outputs[0]).Address);
        }

        [Fact]
        public void DeletingAnAddress_RemovesItsOutput()
        {
            var (modulator, registry, _) = Create();
            var first = AddAddress(registry, "cmix:a");
            AddAddress(registry, "cmix:b");

            registry.AddressManager.DeleteItemCommand.Execute(first);

            Assert.Equal(new[] { "cmix:b" },
                modulator.Outputs.Cast<AddressedOutput>().Select(output => output.Address));
        }

        [Fact]
        public void MovingAnAddress_ChangesTheOrderOfTheOutputs()
        {
            var (modulator, registry, _) = Create();
            var first = AddAddress(registry, "cmix:a");
            AddAddress(registry, "cmix:b");

            registry.AddressManager.MoveItemTo(first, 1);

            Assert.Equal(new[] { "cmix:b", "cmix:a" },
                modulator.Outputs.Cast<AddressedOutput>().Select(output => output.Address));
        }

        [Fact]
        public void TheModulator_SurvivesSavingAndLoading()
        {
            var (modulator, _, _) = Create();

            var json = JsonSerializer.Serialize<IControlModel>(modulator.ToModel(), ProjectSerializer.Options);
            var model = (RedisAddressModulatorModel)JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options)!;

            Assert.Equal(modulator.ID, model.ID);
        }
    }
}
