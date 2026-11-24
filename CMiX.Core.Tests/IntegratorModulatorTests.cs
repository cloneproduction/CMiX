using System.Text.Json;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class IntegratorModulatorTests
    {
        private static IntegratorModulator Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (IntegratorModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(IntegratorModulator));
        }

        [Fact]
        public void Outputs_OfANewModulator_HoldOneFloatValue()
        {
            var modulator = Create();

            var output = Assert.Single(modulator.Outputs);
            Assert.IsType<ModulatorOutput<float>>(output);
            Assert.Equal("Value", output.Name);
        }

        [Fact]
        public void Serialize_ThenDeserialize_KeepsTheModel()
        {
            var modulator = Create();
            modulator.Offset.Value = 0.5f;
            modulator.Step.Value = false;
            modulator.ResetValue.Value = 2f;
            var source = (IntegratorModulatorModel)modulator.ToModel();

            var json = JsonSerializer.Serialize((IControlModel)source, ProjectSerializer.Options);
            var result = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            var model = Assert.IsType<IntegratorModulatorModel>(result);
            Assert.Equal(source.ID, model.ID);
            Assert.Equal(0.5f, model.Offset.Value);
            Assert.False(model.Step.Value);
            Assert.Equal(2f, model.ResetValue.Value);
            Assert.Equal(source.Reset.ID, model.Reset.ID);
        }

        [Fact]
        public void ClickCommand_OfReset_RaisesTheClickEvent()
        {
            var modulator = Create();
            var count = 0;
            modulator.Reset.Click += (s, e) => count++;

            modulator.Reset.ClickCommand.Execute(null);

            Assert.Equal(1, count);
        }

        [Fact]
        public void AddItem_ToAFilter_BindsAValueToTheIntegrator()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(IntegratorModulator));
            var modulator = (IntegratorModulator)blur.ModulatorManager.SelectedItem;
            var output = modulator.Outputs.Single(o => o.Name == "Value");

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, output));

            Assert.True(blur.Strength.IsModulated);
        }
    }
}
