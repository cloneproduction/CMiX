using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia;

[assembly: AvaloniaTestApplication(typeof(CMiX.Studio.Avalonia.Tests.TestAppBuilder))]

namespace CMiX.Studio.Avalonia.Tests
{
    // Configures the headless runtime around the real App class so App.Initialize runs and
    // loads the XAML resources the views under test need (SimpleTheme, Themes/Generic.axaml,
    // Themes/Basic/Basic.axaml). App.OnFrameworkInitializationCompleted only runs its body
    // when ApplicationLifetime is a desktop lifetime, which the headless runtime does not
    // provide, so the tests build their own DI provider instead of relying on it.
    public static class TestAppBuilder
    {
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions());
        }
    }
}
