using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // The expanders open and close their content with the animation of CollapsePanel.
    public class ExpanderAnimationTests
    {
        private static ControlTheme Theme(string key) => (ControlTheme)Avalonia.Application.Current!.FindResource(key)!;

        private static (Control view, CollapsePanel panel) Show(string kind, bool expanded)
        {
            Control view = kind switch
            {
                "indented" => new ExpanderIndented { Header = "Header", Content = new Border { Height = 80 }, IsExpanded = expanded },
                "ExpanderItem" => new Expander { Theme = Theme("ExpanderItem"), Header = "Header", Content = new Border { Height = 80 }, IsExpanded = expanded },
                _ => new Expander { Header = "Header", Content = new Border { Height = 80 }, IsExpanded = expanded }
            };
            new Window { Content = new StackPanel { Children = { view } }, Width = 300, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (view, view.GetVisualDescendants().OfType<CollapsePanel>().Single());
        }

        private static void Toggle(Control view, bool expanded)
        {
            if (view is ExpanderIndented indented)
                indented.IsExpanded = expanded;
            else
                ((Expander)view).IsExpanded = expanded;
            Dispatcher.UIThread.RunJobs();
        }

        public static TheoryData<string> Kinds => new() { "default", "ExpanderItem", "indented" };

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void AClosedExpander_HidesItsContent_AndAnOpenOneShowsIt(string kind)
        {
            var (_, closed) = Show(kind, false);
            var (_, open) = Show(kind, true);

            Assert.False(closed.Child!.IsVisible);
            Assert.Equal(0, closed.Bounds.Height);
            Assert.True(open.Child!.IsVisible);
            Assert.True(open.Bounds.Height >= 80);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Opening_IsAnAnimation(string kind)
        {
            var (view, panel) = Show(kind, false);

            Toggle(view, true);

            Assert.True(panel.Child!.IsVisible);
            Assert.InRange(panel.Progress, 0, 0.999);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void AfterTheAnimation_TheExpanderIsOpenOrClosed(string kind)
        {
            var (view, panel) = Show(kind, false);
            panel.Duration = TimeSpan.Zero;

            Toggle(view, true);
            Assert.Equal(1, panel.Progress);

            Toggle(view, false);
            Assert.Equal(0, panel.Progress);
            Assert.False(panel.Child!.IsVisible);
        }
    }
}
