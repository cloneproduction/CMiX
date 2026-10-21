using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The multiplier of an address in a modulator.
    public class RedisAddressGainTests
    {
        private static RedisAddressGain NewGain() =>
            (RedisAddressGain)TestServiceProviderFactory.Create()
                .GetRequiredService<ControlFactory>().Create(typeof(RedisAddressGain));

        [Fact]
        public void ItStartsAtOne()
        {
            Assert.Equal(1.0f, NewGain().Value);
        }

        [Fact]
        public void Reset_GoesBackToOne()
        {
            var gain = NewGain();
            gain.Value = 3.0f;

            gain.Reset();

            Assert.Equal(1.0f, gain.Value);
        }

        [Fact]
        public void ItKeepsTheValueAndTheAddressThroughTheModel()
        {
            var addressID = Guid.NewGuid();
            var gain = NewGain();
            gain.Value = 2.5f;
            gain.AddressID.Value = addressID;

            var copy = NewGain();
            copy.FromModel(gain.ToModel());

            Assert.Equal(2.5f, copy.Value);
            Assert.Equal(addressID, copy.AddressID.Value);
            Assert.Equal(gain.ID, copy.ID);
        }
    }
}
