using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatableBoolTests
    {
        private static ModulatableValue<bool> Create(IServiceProvider provider) =>
            provider.GetRequiredService<ModulatableValue<bool>>();

        [Fact]
        public void BoolValue_CanBindToABoolOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(LFOModulator));
            var lfo = (LFOModulator)manager.ManagerData.Items[0];
            var value = Create(provider);
            var gate = lfo.Outputs.OfType<ModulatorOutput<bool>>().Single();

            Assert.True(((IModulatorBindable)value).CanBind(gate));
        }

        [Fact]
        public void BoolValue_CannotBindToAFloatOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(LFOModulator));
            var lfo = (LFOModulator)manager.ManagerData.Items[0];
            var value = Create(provider);
            var phase = lfo.Outputs.OfType<ModulatorOutput<float>>().Single();

            Assert.False(((IModulatorBindable)value).CanBind(phase));
        }

        [Fact]
        public void FloatValue_CannotBindToABoolOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<ModulatableValue<float>>();

            Assert.False(((IModulatorBindable)value).CanBind(new ModulatorOutput<bool>("Gate")));
        }

        [Fact]
        public void BoolValue_KeepsItsValueAndBindingThroughTheModel()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = Create(provider);
            value.Value = true;
            var id = Guid.NewGuid();
            value.ModulatorID = id;
            value.BoundOutputName = "Gate";

            var copy = Create(provider);
            copy.FromModel(value.ToModel());

            Assert.True(copy.Value);
            Assert.Equal(id, copy.ModulatorID);
            Assert.Equal("Gate", copy.BoundOutputName);
        }

        [Fact]
        public void BeatStepAndLfo_OfferAGateAfterTheirFirstOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(LFOModulator));
            manager.AddItem(typeof(BeatStepModulator));

            foreach (var modulator in manager.ManagerData.Items.Cast<IModulator>())
            {
                Assert.IsNotType<ModulatorOutput<bool>>(modulator.Outputs[0]);
                Assert.Contains(modulator.Outputs, o => o is ModulatorOutput<bool> { Name: "Gate" });
            }
        }
    }
}
