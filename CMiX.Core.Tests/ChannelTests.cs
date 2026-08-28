using CMiX.Core.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ChannelTests
    {
        [Fact]
        public void Channel_DefaultsToUnbound()
        {
            var provider = TestServiceProviderFactory.Create();
            var channel = provider.GetRequiredService<Channel>();

            Assert.Null(channel.Binding.ModulatorID);
        }

        [Fact]
        public void Channel_ToModel_FromModel_RoundTripsValueAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var channel = provider.GetRequiredService<Channel>();

            channel.Label = "X";
            channel.Value.Value = 2.5f;
            channel.Binding.ModulatorID = Guid.NewGuid();
            channel.Binding.Depth.Value = -0.3f;

            var model = channel.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Channel>();
            reloaded.FromModel(model);

            Assert.Equal("X", reloaded.Label);
            Assert.Equal(2.5f, reloaded.Value.Value);
            Assert.Equal(channel.Binding.ModulatorID, reloaded.Binding.ModulatorID);
            Assert.Equal(-0.3f, reloaded.Binding.Depth.Value);
        }
    }
}
