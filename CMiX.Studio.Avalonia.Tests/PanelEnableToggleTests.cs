using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Boolean;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The switch in the header of a panel that turns the panel on is the box of the captioned toggle, without the caption.
    public class PanelEnableToggleTests
    {
        public static TheoryData<string> Panels => new() { "AmbientOcclusion", "LocalReflection", "FilterPanel" };

        private static Control Create(string panel) => panel switch
        {
            "AmbientOcclusion" => new Views.AmbientOcclusion(),
            "LocalReflection" => new Views.LocalReflection(),
            _ => new Views.FilterPanel()
        };

        [AvaloniaTheory]
        [MemberData(nameof(Panels))]
        public void TheHeaderToggle_IsTheBoxOfTheCaptionedToggle(string panel)
        {
            var view = Create(panel);
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            var toggle = view.GetVisualDescendants().OfType<ToggleButtonCMiX>().First();

            Assert.Equal("IsChecked", toggle.GetVisualDescendants().OfType<AppIcon>().Single().IconKey);
            Assert.Contains(toggle.GetVisualDescendants().OfType<Border>(), border => border.Name == "box");
            Assert.Equal((double)Application.Current!.FindResource("ControlHeightXS")!, toggle.Bounds.Height);
        }
    }
}
