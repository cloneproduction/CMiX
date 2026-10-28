using CMiX.Core.Modulation.Modulators;
using Xunit;

namespace CMiX.Core.Tests
{
    public class AddressedOutputTests
    {
        [Fact]
        public void AFloatOutput_IsAModulatorOutputOfFloat()
        {
            Assert.IsAssignableFrom<ModulatorOutput<float>>(new AddressedOutput<float>("id", "cmix:value"));
        }

        [Fact]
        public void ABoolOutput_IsAModulatorOutputOfBool()
        {
            Assert.IsAssignableFrom<ModulatorOutput<bool>>(new AddressedOutput<bool>("id", "cmix:value"));
        }

        [Fact]
        public void AFloatOutput_KeepsTheNameAndTheAddress()
        {
            var output = new AddressedOutput<float>("id", "cmix:value");

            Assert.Equal("id", output.Name);
            Assert.Equal("cmix:value", output.Address);
            Assert.Equal("cmix:value", ((IAddressedOutput)output).Address);
        }

        [Fact]
        public void ABoolOutput_KeepsTheNameAndTheAddress()
        {
            var output = new AddressedOutput<bool>("id", "cmix:value");

            Assert.Equal("id", output.Name);
            Assert.Equal("cmix:value", output.Address);
            Assert.Equal("cmix:value", ((IAddressedOutput)output).Address);
        }
    }
}
