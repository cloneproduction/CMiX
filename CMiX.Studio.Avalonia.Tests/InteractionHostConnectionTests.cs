using Avalonia.Headless.XUnit;
using CMiX.Core.BaseControls;
using CMiX.UI.Interaction;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The App connects the InteractionHost of the editors to the ValueInteraction of the model.
    public class InteractionHostConnectionTests
    {
        [AvaloniaFact]
        public void App_ConnectsTheHostToTheValueInteraction()
        {
            var scope = InteractionHost.BeginScope();

            Assert.NotNull(scope);
            Assert.True(ValueInteraction.IsActive);

            scope!.Dispose();

            Assert.False(ValueInteraction.IsActive);
        }
    }
}
