using CMiX.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Tests
{
    // Builds the same object graph the apps build at startup, so tests exercise the real
    // DI wiring rather than a hand rolled substitute. See CMiX.Studio.Avalonia App.axaml.cs
    // and CMiX.Console Program.cs for the pattern this mirrors.
    public static class TestServiceProviderFactory
    {
        public static IServiceProvider Create()
        {
            return new ServiceCollection().AddCMiXCore().BuildServiceProvider();
        }
    }
}
