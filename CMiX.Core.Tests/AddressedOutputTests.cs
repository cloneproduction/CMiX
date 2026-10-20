using CMiX.Core.Modulation.Modulators;
using Xunit;

namespace CMiX.Core.Tests
{
    public class AddressedOutputTests
    {
        [Fact]
        public void ItIsAFloatOutput()
        {
            Assert.IsAssignableFrom<ModulatorOutput<float>>(new AddressedOutput("id", "cmix:value"));
        }

        [Fact]
        public void ItKeepsTheNameAndTheAddress()
        {
            var output = new AddressedOutput("id", "cmix:value");

            Assert.Equal("id", output.Name);
            Assert.Equal("cmix:value", output.Address);
        }
    }
}
