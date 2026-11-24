using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // The expanders open and close their content with the animation of PanelCollapse.
    public class ExpanderAnimationTests
    {
        private static ControlTheme Theme(string key) => (ControlTheme)Avalonia.Application.Current!.FindResource(key)!;

        private static Control Show(string kind)
        {
            Control view = kind switch
            {
                "indented" => new ExpanderIndented { Header = "Header", Content = new Border { Height = 80 } },
                "ExpanderItem" => new Expander { Theme = Theme("ExpanderItem"), Header = "Header", Content = new Border { Height = 80 } },
                _ => new Expander { Header = "Header", Content = new Border { Height = 80 } }
            };
            new Window { Content = new StackPanel { Children = { view } }, Width = 300, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        public static TheoryData<string> Kinds => new() { "default", "ExpanderItem", "indented" };

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Show_AnExpanderOfEachTheme_HasOnePanelCollapse(string kind)
        {
            var view = Show(kind);

            Assert.Single(view.GetVisualDescendants().OfType<PanelCollapse>());
        }
    }
}
