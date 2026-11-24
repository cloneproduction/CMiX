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
    public class LISAModulatorTests
    {
        private static LISAModulator Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (LISAModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(LISAModulator));
        }

        private static void SetValues(LISAModulator modulator)
        {
            modulator.Count.Value = 7;
            modulator.X.Value = 1.5f;
            modulator.Y.Value = 2.5f;
            modulator.XRange.Minimum.Value = 10f;
            modulator.XRange.Maximum.Value = 11f;
            modulator.YRange.Minimum.Value = 20f;
            modulator.YRange.Maximum.Value = 21f;
        }

        [Fact]
        public void Outputs_OfANewModulator_HoldCountXAndY()
        {
            var modulator = Create();

            Assert.Equal(3, modulator.Outputs.Count);
            Assert.IsType<ModulatorOutput<int>>(modulator.Outputs[0]);
            Assert.Equal("Count", modulator.Outputs[0].Name);
            Assert.IsType<ModulatorOutput<float>>(modulator.Outputs[1]);
            Assert.Equal("X", modulator.Outputs[1].Name);
            Assert.IsType<ModulatorOutput<float>>(modulator.Outputs[2]);
            Assert.Equal("Y", modulator.Outputs[2].Name);
        }

        [Fact]
        public void Serialize_ThenDeserialize_KeepsTheModel()
        {
            var modulator = Create();
            SetValues(modulator);
            var source = (LISAModulatorModel)modulator.ToModel();

            var json = JsonSerializer.Serialize((IControlModel)source, ProjectSerializer.Options);
            var result = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            var model = Assert.IsType<LISAModulatorModel>(result);
            Assert.Equal(source.ID, model.ID);
            Assert.Equal(7, model.Count.Value);
            Assert.Equal(1.5f, model.X.Value);
            Assert.Equal(2.5f, model.Y.Value);
            Assert.Equal(10f, model.XRange.Minimum.Value);
            Assert.Equal(11f, model.XRange.Maximum.Value);
            Assert.Equal(20f, model.YRange.Minimum.Value);
            Assert.Equal(21f, model.YRange.Maximum.Value);
        }

        [Fact]
        public void Category_OfTheModulator_IsExternal()
        {
            var modulator = Create();

            Assert.Equal(ModulatorCategory.External, ((IModulator)modulator).Category);
        }

        [Fact]
        public void AddItem_ToAFilter_BindsAValueToTheXOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(LISAModulator));
            var modulator = (LISAModulator)blur.ModulatorManager.SelectedItem;
            var output = modulator.Outputs.Single(o => o.Name == "X");

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, output));

            Assert.True(blur.Strength.IsModulated);
        }
    }
}
