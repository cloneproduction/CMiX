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
    }
}
