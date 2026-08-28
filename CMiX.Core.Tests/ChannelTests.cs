using CMiX.Core.Animations;
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
            Assert.Null(channel.Binding.BoundModulator);
        }

        [Fact]
        public void Channel_ToModel_FromModel_RoundTripsValueAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var channel = provider.GetRequiredService<Channel>();

            channel.Label = "X";
            channel.Value.Value = 2.5f;
            channel.Binding.ModulatorID = Guid.NewGuid();

            var model = channel.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Channel>();
            reloaded.FromModel(model);

            Assert.Equal("X", reloaded.Label);
            Assert.Equal(2.5f, reloaded.Value.Value);
            Assert.Equal(channel.Binding.ModulatorID, reloaded.Binding.ModulatorID);
        }

        [Fact]
        public void SetModulatorCommand_SetsIdAndLiveReference_ThenNullClearsBoth()
        {
            var provider = TestServiceProviderFactory.Create();
            var channel = provider.GetRequiredService<Channel>();
            var beatModifier = provider.GetRequiredService<BeatModifier>();

            channel.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, channel.Binding.ModulatorID);
            Assert.Same(beatModifier, channel.Binding.BoundModulator);

            channel.Binding.SetModulatorCommand.Execute(null);

            Assert.Null(channel.Binding.ModulatorID);
            Assert.Null(channel.Binding.BoundModulator);
        }
    }
}
