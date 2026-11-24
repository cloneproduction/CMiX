using CMiX.Core.BaseControls;
using CMiX.Studio.Avalonia.Drag;
using CMiX.UI.Interaction;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Connect makes the InteractionHost of the editors open the ValueInteraction of the given provider.
    public class InteractionHostConnectionTests
    {
        [Fact]
        public void Connect_OfAProvider_OpensTheValueInteractionOfThatProvider()
        {
            var provider = TestServiceProviderFactory.Create();
            try
            {
                InteractionHostConnection.Connect(provider);
                var interaction = provider.GetRequiredService<ValueInteraction>();

                var scope = InteractionHost.BeginScope();

                Assert.NotNull(scope);
                Assert.True(interaction.IsActive);

                scope!.Dispose();

                Assert.False(interaction.IsActive);
            }
            finally
            {
                InteractionHost.BeginScopeHandler = null;
            }
        }
    }
}
