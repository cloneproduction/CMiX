using CMiX.Core.Networking;
using Xunit;

namespace CMiX.Core.Tests
{
    // These tests only read. They never write to the Credential Manager.
    public class WindowsCredentialStoreTests
    {
        [Fact]
        public void TryRead_EmptyName_ReturnsFalseAndEmptyValues()
        {
            var store = new WindowsCredentialStore();

            Assert.False(store.TryRead("", out var user, out var password));
            Assert.Equal("", user);
            Assert.Equal("", password);

            Assert.False(store.TryRead(null, out user, out password));
            Assert.Equal("", user);
            Assert.Equal("", password);
        }

        [Fact]
        public void TryRead_UnknownName_ReturnsFalse()
        {
            var store = new WindowsCredentialStore();
            var name = "cmix-test-" + Guid.NewGuid().ToString("N");

            Assert.False(store.TryRead(name, out var user, out var password));
            Assert.Equal("", user);
            Assert.Equal("", password);
        }
    }
}
