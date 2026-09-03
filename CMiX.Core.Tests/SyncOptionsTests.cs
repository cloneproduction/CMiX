using System;
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
    }
}
