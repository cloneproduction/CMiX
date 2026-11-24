using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The expanders open and close their content with the animation of PanelCollapse.
    public class ExpanderAnimationTests
    {
        private static Control Show(string kind)
        {
            Control view = kind switch
            {
                "indented" => new ExpanderIndented { Header = "Header", Content = new Border { Height = 80 } },
                "ExpanderItem" => new Expander { Theme = Tokens.Theme("ExpanderItem"), Header = "Header", Content = new Border { Height = 80 } },
                _ => new Expander { Header = "Header", Content = new Border { Height = 80 } }
            };
            TestWindow.Show(new StackPanel { Children = { view } }, 300, 400);
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
