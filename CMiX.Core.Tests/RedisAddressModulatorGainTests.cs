using System.Text.Json;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The gains of a RedisAddressModulator: one multiplier for each address of the project.
    public class RedisAddressModulatorGainTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private RedisAddressModulator NewModulator() =>
            (RedisAddressModulator)_provider.GetRequiredService<ControlFactory>().Create(typeof(RedisAddressModulator));

        private RedisAddress AddAddress(string text)
        {
            Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        [Fact]
        public void ItStartsWithNoGain()
        {
            Assert.Empty(NewModulator().Gains);
        }

        [Fact]
        public void EnsureGains_GivesOneGainOfOneToEachAddress()
        {
            var modulator = NewModulator();
            var first = AddAddress("cmix:a");
            var second = AddAddress("cmix:b");

            modulator.EnsureGains();

            Assert.Equal(new Guid?[] { first.ID, second.ID }, modulator.Gains.Select(gain => gain.AddressID.Value));
            Assert.All(modulator.Gains, gain => Assert.Equal(1.0f, gain.Value));
        }

        [Fact]
        public void EnsureGains_TwiceDoesNotDuplicate()
        {
            var modulator = NewModulator();
            AddAddress("cmix:a");

            modulator.EnsureGains();
            modulator.EnsureGains();

            Assert.Single(modulator.Gains);
        }

        [Fact]
        public void EnsureGains_KeepsTheValueOfAGain()
        {
            var modulator = NewModulator();
            AddAddress("cmix:a");
            modulator.EnsureGains();
            modulator.Gains.Single().Value = 2.5f;
            AddAddress("cmix:b");

            modulator.EnsureGains();

            Assert.Equal(2.5f, modulator.Gains.First().Value);
        }

        [Fact]
        public void EnsureGains_TakesAwayTheGainOfARemovedAddress()
        {
            var modulator = NewModulator();
            var first = AddAddress("cmix:a");
            var second = AddAddress("cmix:b");
            modulator.EnsureGains();

            Registry.AddressManager.RemoveItem(first);
            modulator.EnsureGains();

            Assert.Equal(new Guid?[] { second.ID }, modulator.Gains.Select(gain => gain.AddressID.Value));
        }

        [Fact]
        public void TwoModulators_HaveTheirOwnGains()
        {
            var first = NewModulator();
            var second = NewModulator();
            AddAddress("cmix:a");
            first.EnsureGains();
            second.EnsureGains();

            first.Gains.Single().Value = 4.0f;

            Assert.Equal(1.0f, second.Gains.Single().Value);
        }

        [Fact]
        public void AGain_KnowsItsAddress()
        {
            var modulator = NewModulator();
            var address = AddAddress("cmix:a");
            modulator.EnsureGains();

            address.Value = "cmix:changed";

            Assert.Same(address, modulator.Gains.Single().Address);
            Assert.Equal("cmix:changed", modulator.Gains.Single().Address.Value);
        }

        [Fact]
        public void TheGains_SurviveSavingAndLoading()
        {
            var modulator = NewModulator();
            var address = AddAddress("cmix:a");
            modulator.EnsureGains();
            modulator.Gains.Single().Value = 3.0f;

            var json = JsonSerializer.Serialize<IControlModel>(modulator.ToModel(), ProjectSerializer.Options);
            var model = (RedisAddressModulatorModel)JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options)!;
            var loaded = NewModulator();
            loaded.FromModel(model);

            var gain = loaded.Gains.Single();
            Assert.Equal(3.0f, gain.Value);
            Assert.Equal(address.ID, gain.AddressID.Value);
        }
    }
}
