using CMiX.Core.Networking;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SyncOptionsTests
    {
        [Fact]
        public void WithFallbacks_OnAnEmptyPeerName_GivesTheMachineName()
        {
            var options = (SyncOptions.Defaults with { PeerName = string.Empty }).WithFallbacks();

            Assert.Equal(Environment.MachineName, options.PeerName);
        }

        [Fact]
        public void WithFallbacks_OnANullPeerName_GivesTheMachineName()
        {
            var options = (SyncOptions.Defaults with { PeerName = null }).WithFallbacks();

            Assert.Equal(Environment.MachineName, options.PeerName);
        }

        [Fact]
        public void WithFallbacks_KeepsAGivenPeerName()
        {
            var options = (SyncOptions.Defaults with { PeerName = "Engine 1" }).WithFallbacks();

            Assert.Equal("Engine 1", options.PeerName);
        }

        [Fact]
        public void WithFallbacks_FillsTheOtherEmptyFields()
        {
            var options = new SyncOptions("", 0, 2, "", null, "", "A", null).WithFallbacks();

            Assert.Equal("127.0.0.1", options.Ip);
            Assert.Equal(6379, options.Port);
            Assert.Equal(2, options.Database);
            Assert.Equal("default", options.User);
            Assert.Equal("", options.Password);
            Assert.Equal("cmix:default", options.KeyPrefix);
            Assert.Equal("A", options.PeerName);
            Assert.Equal("", options.Role);
        }

        [Fact]
        public void WithFallbacks_KeepsGivenValues()
        {
            var original = new SyncOptions("10.0.0.5", 6400, 2, "studio-user", "secret", "cmix:studio", "MyStudio", "studio");

            Assert.Equal(original, original.WithFallbacks());
        }

        [Fact]
        public void WithCredential_EmptyName_KeepsOptions()
        {
            var options = SyncOptions.Defaults with { User = "fileUser", Password = "filePassword" };
            var store = new FakeCredentialStore();
            store.Add("MyRedis", "alice", "secret");

            var fromEmpty = options.WithCredential("", store, "file");
            var fromNull = options.WithCredential(null, store, "file");

            Assert.Equal(options, fromEmpty.Options);
            Assert.Equal("from file", fromEmpty.UserSource);
            Assert.Equal(options, fromNull.Options);
            Assert.Equal("from file", fromNull.UserSource);
        }

        [Fact]
        public void WithCredential_Found_TakesUserAndPassword()
        {
            // The callers run WithFallbacks first, so the peer name is already set.
            var options = (SyncOptions.Defaults with { User = "fileUser", Password = "filePassword" }).WithFallbacks();
            var store = new FakeCredentialStore();
            store.Add("MyRedis", "alice", "secret");

            var result = options.WithCredential("MyRedis", store, "file");

            Assert.Equal("alice", result.Options.User);
            Assert.Equal("secret", result.Options.Password);
            Assert.Equal("from credential MyRedis", result.UserSource);
            Assert.Equal(options with { User = "alice", Password = "secret" }, result.Options);
        }

        [Fact]
        public void WithCredential_FoundWithEmptyUser_GivesDefault()
        {
            var options = SyncOptions.Defaults with { User = "fileUser", Password = "filePassword" };
            var store = new FakeCredentialStore();
            store.Add("MyRedis", "", "secret");

            var result = options.WithCredential("MyRedis", store, "file");

            Assert.Equal("default", result.Options.User);
            Assert.Equal("secret", result.Options.Password);
        }

        [Fact]
        public void WithCredential_NotFound_KeepsOptions()
        {
            var options = SyncOptions.Defaults with { User = "fileUser", Password = "filePassword" };
            var store = new FakeCredentialStore();
            store.Add("OtherRedis", "alice", "secret");

            var result = options.WithCredential("MyRedis", store, "file");

            Assert.Equal(options, result.Options);
            Assert.Equal("from file, credential MyRedis not found", result.UserSource);
        }

        [Fact]
        public void WithCredential_NotFoundWithEmptyGivenValues_GivesFallbacks()
        {
            var options = (SyncOptions.Defaults with { User = "", Password = "" }).WithFallbacks();

            var result = options.WithCredential("MyRedis", new FakeCredentialStore(), "file");

            Assert.Equal("default", result.Options.User);
            Assert.Equal("", result.Options.Password);
        }

        [Fact]
        public void WithCredential_UsesGivenSourceWord()
        {
            var result = SyncOptions.Defaults.WithCredential("", new FakeCredentialStore(), "arguments");

            Assert.Equal("from arguments", result.UserSource);
        }
    }
}
