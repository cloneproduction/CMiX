using System.Text.Json;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The behaviour that the three Redis modulators share: one selected address, one output named Value, the values it binds.
    public class RedisModulatorTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private RedisModulatorBase NewModulator(Type modulatorType) =>
            (RedisModulatorBase)_provider.GetRequiredService<ControlFactory>().Create(modulatorType);

        private RedisAddress AddAddress(string text)
        {
            Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        private IModulatorBindable NewValue(Type valueType) =>
            (IModulatorBindable)_provider.GetRequiredService(typeof(ModulatableValue<>).MakeGenericType(valueType));

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void Create_ANewModulator_IsAModulatorThatSelectsNothing(Type modulatorType)
        {
            AddAddress("cmix:a");

            var modulator = NewModulator(modulatorType);

            Assert.IsAssignableFrom<IModulator>(modulator);
            Assert.Null(modulator.SelectedAddress);
            Assert.Null(modulator.SelectedAddressID.Value);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator), typeof(float))]
        [InlineData(typeof(RedisBoolModulator), typeof(bool))]
        [InlineData(typeof(RedisIntModulator), typeof(int))]
        public void Outputs_WithoutASelection_IsOneOutputNamedValue(Type modulatorType, Type valueType)
        {
            var modulator = NewModulator(modulatorType);

            var output = Assert.Single(modulator.Outputs);
            Assert.IsType(typeof(AddressedOutput<>).MakeGenericType(valueType), output);
            Assert.IsAssignableFrom(typeof(ModulatorOutput<>).MakeGenericType(valueType), output);
            Assert.Equal("Value", output.Name);
            Assert.Equal("Value", ((IAddressedOutput)output).Address);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void Outputs_WithASelection_KeepsTheNameAndShowsTheAddress(Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            var address = AddAddress("cmix:a");

            modulator.SelectedAddress = address;

            var output = Assert.Single(modulator.Outputs);
            Assert.Equal("Value", output.Name);
            Assert.Equal("cmix:a", ((IAddressedOutput)output).Address);
            Assert.Same(address, modulator.SelectedAddress);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void SelectedAddress_TwoModulators_CanSelectDifferentAddresses(Type modulatorType)
        {
            var first = NewModulator(modulatorType);
            var second = NewModulator(modulatorType);
            var a = AddAddress("cmix:a");
            var b = AddAddress("cmix:b");

            first.SelectedAddress = a;
            second.SelectedAddress = b;

            Assert.Same(a, first.SelectedAddress);
            Assert.Same(b, second.SelectedAddress);
            Assert.Equal("cmix:a", ((IAddressedOutput)first.Outputs[0]).Address);
            Assert.Equal("cmix:b", ((IAddressedOutput)second.Outputs[0]).Address);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void FromModel_OfToModel_KeepsTheIDAndTheSelection(Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            var address = AddAddress("cmix:a");
            modulator.SelectedAddress = address;

            var copy = NewModulator(modulatorType);
            copy.FromModel(modulator.ToModel());

            Assert.Equal(modulator.ID, copy.ID);
            Assert.Same(address, copy.SelectedAddress);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void FromModel_AnUnknownAddressID_GivesNoSelectedAddress(Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            var model = (RedisModulatorModelBase)modulator.ToModel();
            model.SelectedAddressID = new(Guid.NewGuid());

            modulator.FromModel(model);

            Assert.NotNull(modulator.SelectedAddressID.Value);
            Assert.Null(modulator.SelectedAddress);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void OnAddressRemoved_TheSelectedAddress_ClearsTheSelectionAndTellsTheOutputName(Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            var address = AddAddress("cmix:a");
            modulator.SelectedAddress = address;
            string removed = null;
            modulator.OutputRemoved += name => removed = name;

            modulator.OnAddressRemoved(address.ID);

            Assert.Null(modulator.SelectedAddressID.Value);
            Assert.Equal("Value", removed);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator))]
        [InlineData(typeof(RedisBoolModulator))]
        [InlineData(typeof(RedisIntModulator))]
        public void OnAddressRemoved_AnotherAddress_KeepsTheSelection(Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            var address = AddAddress("cmix:a");
            modulator.SelectedAddress = address;

            modulator.OnAddressRemoved(Guid.NewGuid());

            Assert.Same(address, modulator.SelectedAddress);
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator), typeof(float), typeof(bool))]
        [InlineData(typeof(RedisBoolModulator), typeof(bool), typeof(float))]
        [InlineData(typeof(RedisIntModulator), typeof(int), typeof(float))]
        public void CanBind_AValueOfTheOutputType_IsTrueAndOfAnotherTypeIsFalse(Type modulatorType, Type valueType, Type otherType)
        {
            var output = NewModulator(modulatorType).Outputs.Single();

            Assert.True(NewValue(valueType).CanBind(output));
            Assert.False(NewValue(otherType).CanBind(output));
        }

        [Theory]
        [InlineData(typeof(RedisBoolModulator), typeof(SequencePlayer), nameof(SequencePlayer.Playing), true)]
        [InlineData(typeof(RedisBoolModulator), typeof(VideoPlayer), nameof(VideoPlayer.DoSeek), true)]
        [InlineData(typeof(RedisBoolModulator), typeof(Blur), nameof(Blur.Strength), false)]
        [InlineData(typeof(RedisIntModulator), typeof(SequencePlayer), nameof(SequencePlayer.SeekTarget), true)]
        [InlineData(typeof(RedisIntModulator), typeof(Blur), nameof(Blur.Strength), false)]
        public void CanBind_AValueOfARealControl_FollowsTheOutputType(Type modulatorType, Type controlType, string valueName, bool expected)
        {
            var control = _provider.GetRequiredService<ControlFactory>().Create(controlType);
            var value = (IModulatorBindable)controlType.GetProperty(valueName)!.GetValue(control)!;

            Assert.Equal(expected, value.CanBind(NewModulator(modulatorType).Outputs.Single()));
        }

        [Theory]
        [InlineData(typeof(RedisFloatModulator), "$type,ID,PrefabService,SelectedAddressID,Range")]
        [InlineData(typeof(RedisBoolModulator), "$type,ID,PrefabService,SelectedAddressID")]
        [InlineData(typeof(RedisIntModulator), "$type,ID,PrefabService,SelectedAddressID")]
        public void ToModel_OfEachRedisModulator_SerializesTheSameKeysAsBefore(Type modulatorType, string keys)
        {
            var model = NewModulator(modulatorType).ToModel();

            var json = JsonSerializer.Serialize(model, ProjectSerializer.Options);

            using var document = JsonDocument.Parse(json);
            var written = document.RootElement.EnumerateObject().Select(property => property.Name);
            Assert.Equal(keys, string.Join(",", written));
            Assert.Equal(modulatorType.FullName + "Model", document.RootElement.GetProperty("$type").GetString());
        }
    }
}
