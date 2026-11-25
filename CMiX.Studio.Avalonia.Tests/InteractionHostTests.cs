using Avalonia.Headless.XUnit;
using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.UI.Interaction;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // App.Initialize sets the handler. The tests do not reset it, because other tests use it.
    public class InteractionHostTests
    {
        [AvaloniaFact]
        public void BeginDrag_OfAValue_OpensTheDragOfThatValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = new GenericValue<float>(provider.GetRequiredService<IValueSync>(), provider.GetRequiredService<ControlRegistry>());

            var handle = InteractionHost.BeginDrag(value);

            Assert.NotNull(handle);
            handle!.Dispose();
        }

        [AvaloniaFact]
        public void BeginDrag_WithoutAValue_OpensNothing()
        {
            Assert.Null(InteractionHost.BeginDrag(null));
            Assert.Null(InteractionHost.BeginDrag(new object()));
        }
    }
}
