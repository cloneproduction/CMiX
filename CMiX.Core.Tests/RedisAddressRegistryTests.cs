using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The global list of Redis addresses.
    public class RedisAddressRegistryTests
    {
        private static RedisAddressRegistry NewRegistry() =>
            TestServiceProviderFactory.Create().GetRequiredService<RedisAddressRegistry>();

        [Fact]
        public void ThereIsOneRegistryForTheWholeProject()
        {
            var provider = TestServiceProviderFactory.Create();

            Assert.Same(provider.GetRequiredService<RedisAddressRegistry>(), provider.GetRequiredService<RedisAddressRegistry>());
        }

        [Fact]
        public void TheListHasTheFixedID()
        {
            Assert.Equal(ManagerIDs.RedisAddressManager, NewRegistry().AddressManager.ManagerData.ID);
        }

        [Fact]
        public void ItStartsEmpty()
        {
            Assert.Empty(NewRegistry().Addresses);
        }

        [Fact]
        public void AddedAddresses_AreListedInOrder()
        {
            var registry = NewRegistry();

            foreach (var text in new[] { "cmix:a", "cmix:b" })
            {
                registry.AddressManager.AddItem(typeof(RedisAddress));
                ((RedisAddress)registry.AddressManager.SelectedItem).Value = text;
            }

            Assert.Equal(new[] { "cmix:a", "cmix:b" }, registry.Addresses.Select(address => address.Value));
        }
    }
}
