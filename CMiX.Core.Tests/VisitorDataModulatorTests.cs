using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class VisitorDataModulatorTests
    {
        private static VisitorDataModulator Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (VisitorDataModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(VisitorDataModulator));
        }

        [Fact]
        public void Outputs_OfAFreshModulator_NameTheSevenValuesInOrder()
        {
            var modulator = Create();

            Assert.Equal(
                new[] { "Count", "HeartRate", "Temperature", "Conductivity", "ETemp", "BlueIOTPosX", "BlueIOTPosY" },
                modulator.Outputs.Select(o => o.Name));
        }

        [Fact]
        public void FromModel_OfToModel_KeepsEveryValue()
        {
            var modulator = Create();
            modulator.Count.Value = 7;
            modulator.HeartRate.Value = 1.5f;
            modulator.Temperature.Value = 2.5f;
            modulator.Conductivity.Value = 3.5f;
            modulator.ETemp.Value = 4.5f;
            modulator.BlueIOTPosX.Value = 5.5f;
            modulator.BlueIOTPosY.Value = 6.5f;
            modulator.HeartRateRange.Minimum.Value = 10f;
            modulator.HeartRateRange.Maximum.Value = 11f;
            modulator.TemperatureRange.Minimum.Value = 20f;
            modulator.TemperatureRange.Maximum.Value = 21f;
            modulator.ConductivityRange.Minimum.Value = 30f;
            modulator.ConductivityRange.Maximum.Value = 31f;
            modulator.ETempRange.Minimum.Value = 40f;
            modulator.ETempRange.Maximum.Value = 41f;
            modulator.BlueIOTPosXRange.Minimum.Value = 50f;
            modulator.BlueIOTPosXRange.Maximum.Value = 51f;
            modulator.BlueIOTPosYRange.Minimum.Value = 60f;
            modulator.BlueIOTPosYRange.Maximum.Value = 61f;

            var copy = Create();
            copy.FromModel(modulator.ToModel());

            Assert.Equal(7, copy.Count.Value);
            Assert.Equal(1.5f, copy.HeartRate.Value);
            Assert.Equal(2.5f, copy.Temperature.Value);
            Assert.Equal(3.5f, copy.Conductivity.Value);
            Assert.Equal(4.5f, copy.ETemp.Value);
            Assert.Equal(5.5f, copy.BlueIOTPosX.Value);
            Assert.Equal(6.5f, copy.BlueIOTPosY.Value);
            Assert.Equal(10f, copy.HeartRateRange.Minimum.Value);
            Assert.Equal(11f, copy.HeartRateRange.Maximum.Value);
            Assert.Equal(20f, copy.TemperatureRange.Minimum.Value);
            Assert.Equal(21f, copy.TemperatureRange.Maximum.Value);
            Assert.Equal(30f, copy.ConductivityRange.Minimum.Value);
            Assert.Equal(31f, copy.ConductivityRange.Maximum.Value);
            Assert.Equal(40f, copy.ETempRange.Minimum.Value);
            Assert.Equal(41f, copy.ETempRange.Maximum.Value);
            Assert.Equal(50f, copy.BlueIOTPosXRange.Minimum.Value);
            Assert.Equal(51f, copy.BlueIOTPosXRange.Maximum.Value);
            Assert.Equal(60f, copy.BlueIOTPosYRange.Minimum.Value);
            Assert.Equal(61f, copy.BlueIOTPosYRange.Maximum.Value);
        }
    }
}
