using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(CMiX.UI.Tests.TestAppBuilder))]

namespace CMiX.UI.Tests
{
    public static class TestAppBuilder
    {
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<TestApp>()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions());
        }
    }
}
