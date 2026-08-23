using CMiX.Core.Networking.Servers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Regression coverage for a code-review finding: ApplyAsync used to return whether the
    // settings merely passed validation, not whether Start() (called afterward) actually
    // succeeded in binding. This exercises the real happy path end to end, confirming the
    // returned bool now agrees with the server's actual running state.
    public class ServerApplyAsyncTests
    {
        [Fact]
        public async Task ApplyAsync_WithValidLoopbackSettings_ReturnsTrue_AndServerIsListening()
        {
            var server = TestServiceProviderFactory.Create().GetRequiredService<Server>();
            server.IP.Value = "127.0.0.1";
            server.Port.Value = 18085;

            try
            {
                var applied = await server.ApplyAsync();

                Assert.True(applied);
                Assert.NotNull(server.WatsonTcpServer);
                Assert.True(server.ServerIsRunning);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
