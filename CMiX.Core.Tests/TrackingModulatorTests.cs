using System.Text.Json;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TrackingModulatorTests
    {
        private static TrackingModulator Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (TrackingModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(TrackingModulator));
        }

        [Fact]
        public void Outputs_OfANewModulator_AreCountXAndY()
        {
            Assert.Equal(new[] { "Count", "X", "Y" }, Create().Outputs.Select(o => o.Name));
        }

        [Fact]
        public void FromModel_OfToModel_KeepsEveryValue()
        {
            var modulator = Create();
            modulator.Count.Value = 7;
            modulator.X.Value = 1.5f;
            modulator.Y.Value = 2.5f;
            modulator.XRange.Minimum.Value = 10f;
            modulator.XRange.Maximum.Value = 11f;
            modulator.YRange.Minimum.Value = 20f;
            modulator.YRange.Maximum.Value = 21f;

            var copy = Create();
            copy.FromModel(modulator.ToModel());

            Assert.Equal(7, copy.Count.Value);
            Assert.Equal(1.5f, copy.X.Value);
            Assert.Equal(2.5f, copy.Y.Value);
            Assert.Equal(10f, copy.XRange.Minimum.Value);
            Assert.Equal(11f, copy.XRange.Maximum.Value);
            Assert.Equal(20f, copy.YRange.Minimum.Value);
            Assert.Equal(21f, copy.YRange.Maximum.Value);
        }

        [Fact]
        public void Serialize_ATrackingModulatorModel_KeepsTheKeyOrder()
        {
            var json = JsonSerializer.Serialize((IControlModel)new TrackingModulatorModel(), ProjectSerializer.Options);

            using var document = JsonDocument.Parse(json);
            var names = document.RootElement.EnumerateObject().Select(property => property.Name).ToArray();

            Assert.Equal(new[] { "$type", "ID", "PrefabService", "Count", "X", "Y", "XRange", "YRange" }, names);
        }

        [Fact]
        public void Serialize_ThenDeserialize_KeepsTheModel()
        {
            var modulator = Create();
            modulator.Count.Value = 7;
            modulator.X.Value = 1.5f;
            modulator.Y.Value = 2.5f;
            modulator.XRange.Minimum.Value = 10f;
            modulator.XRange.Maximum.Value = 11f;
            modulator.YRange.Minimum.Value = 20f;
            modulator.YRange.Maximum.Value = 21f;
            var source = (TrackingModulatorModel)modulator.ToModel();

            var json = JsonSerializer.Serialize((IControlModel)source, ProjectSerializer.Options);
            var result = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            var model = Assert.IsType<TrackingModulatorModel>(result);
            Assert.Equal(source.ID, model.ID);
            Assert.Equal(7, model.Count.Value);
            Assert.Equal(1.5f, model.X.Value);
            Assert.Equal(2.5f, model.Y.Value);
            Assert.Equal(10f, model.XRange.Minimum.Value);
            Assert.Equal(11f, model.XRange.Maximum.Value);
            Assert.Equal(20f, model.YRange.Minimum.Value);
            Assert.Equal(21f, model.YRange.Maximum.Value);
        }
    }
}
