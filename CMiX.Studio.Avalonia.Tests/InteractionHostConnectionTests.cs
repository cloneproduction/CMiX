using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Studio.Avalonia.Drag;
using CMiX.UI.Interaction;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Connect makes the InteractionHost of the editors open the drag of the value they hand.
    public class InteractionHostConnectionTests
    {
        [Fact]
        public void Connect_ThenBeginDragWithoutAValue_OpensNoDrag()
        {
            try
            {
                InteractionHostConnection.Connect();

                var scope = InteractionHost.BeginDrag(null);

                Assert.Null(scope);
            }
            finally
            {
                InteractionHost.BeginDragHandler = null;
            }
        }

        [Fact]
        public void Connect_ThenBeginDragWithAValue_OpensTheDragOfThatValue()
        {
            var provider = TestServiceProviderFactory.Create();
            try
            {
                InteractionHostConnection.Connect();
                var value = new GenericValue<float>(provider.GetRequiredService<IValueSync>(), provider.GetRequiredService<ControlRegistry>());

                var scope = InteractionHost.BeginDrag(value);

                Assert.NotNull(scope);

                scope!.Dispose();
            }
            finally
            {
                InteractionHost.BeginDragHandler = null;
            }
        }
    }
}
