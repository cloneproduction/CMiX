using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Themes.Simple;
using CMiX.UI;
using CMiX.UI.Themes;
using Material.Icons.Avalonia;

[assembly: AvaloniaTestApplication(typeof(CMiX.UI.Tests.TestAppBuilder))]

namespace CMiX.UI.Tests
{
    // A test application that has only what an application needs to use CMiX.UI: a base theme,
    // the icon styles and CMiXTheme. A control that needs more than this fails in these tests.
    public class TestApp : Application
    {
        public override void Initialize()
        {
            RequestedThemeVariant = ThemeVariant.Dark;
            Styles.Add(new SimpleTheme());
            Styles.Add(new MaterialIconStyles(null));
            Styles.Add(new CMiXTheme());
        }
    }

    public static class TestAppBuilder
    {
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<TestApp>()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions());
        }
    }
}
