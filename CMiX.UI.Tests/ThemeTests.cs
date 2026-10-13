using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI;
using CMiX.UI.Themes;
using Xunit;

namespace CMiX.UI.Tests
{
    // The test application has only CMiXTheme. A control that needs a resource from anywhere else fails here.
    public class ThemeTests
    {
        private static IEnumerable<Type> ControlTypes() =>
            typeof(CMiXTheme).Assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Control).IsAssignableFrom(t)
                         && t.GetConstructor(Type.EmptyTypes) != null);

        [AvaloniaFact]
        public void EveryControl_LoadsWithOnlyTheCMiXTheme()
        {
            var types = ControlTypes().ToList();
            var failures = new List<string>();

            foreach (var type in types)
            {
                try
                {
                    var control = (Control)Activator.CreateInstance(type)!;
                    var window = new Window { Content = control, Width = 400, Height = 300 };
                    window.Show();
                    Dispatcher.UIThread.RunJobs();
                    window.Close();
                }
                catch (Exception e)
                {
                    failures.Add($"{type.Name}: {e.GetType().Name}: {e.Message}");
                }
            }

            Assert.True(types.Count > 25, $"Only {types.Count} controls were found. The scan is broken.");
            Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
        }

        [AvaloniaFact]
        public void TheTokens_AreResolvedFromTheTheme()
        {
            var app = Avalonia.Application.Current!;

            foreach (var key in new[] { "ForegroundBrush", "Grey100Brush", "AccentBrush", "SpacingXXS", "ControlHeightMD", "ContextMenuDefault", "FlyoutPresenterPanel" })
                Assert.True(app.TryFindResource(key, app.ActualThemeVariant, out var value) && value != null, $"The resource '{key}' was not found.");
        }
    }
}
