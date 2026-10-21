using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // There is one chevron in the application: the same icon, size and opacity, and it turns when it opens.
    public class ChevronTests
    {
        private static T Show<T>(T control) where T : Control
        {
            new Window { Content = control, Width = 400, Height = 300 }.Show();
            Dispatcher.UIThread.RunJobs();
            return control;
        }

        private static double Token(string key) => (double)Application.Current!.FindResource(key)!;

        [AvaloniaFact]
        public void AChevron_PointsRightWhenClosed_AndDownWhenOpen()
        {
            var chevron = Show(new Chevron { Duration = TimeSpan.Zero });
            Assert.Equal(-90, chevron.Angle);

            chevron.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0, chevron.Angle);
        }

        [AvaloniaFact]
        public void AChevronWithOtherAngles_UsesThem()
        {
            var chevron = Show(new Chevron { ClosedAngle = 0, OpenAngle = 180, Duration = TimeSpan.Zero });
            Assert.Equal(0, chevron.Angle);

            chevron.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(180, chevron.Angle);
        }

        [AvaloniaFact]
        public void TheIcon_IsReallyTurned()
        {
            var chevron = Show(new Chevron { Duration = TimeSpan.Zero });
            var icon = chevron.GetVisualDescendants().OfType<AppIcon>().Single();
            Assert.Equal(-90, ((Avalonia.Media.RotateTransform)icon.RenderTransform!).Angle);

            chevron.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0, ((Avalonia.Media.RotateTransform)icon.RenderTransform!).Angle);
        }

        [AvaloniaFact]
        public void TheTurn_IsAnAnimation_NotAJump()
        {
            var chevron = Show(new Chevron());

            chevron.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            Assert.NotEqual(0, chevron.Angle);
        }

        [AvaloniaFact]
        public void TheFirstState_ShowsAtOnce()
        {
            var chevron = Show(new Chevron { IsOpen = true });

            Assert.Equal(0, chevron.Angle);
        }

        [AvaloniaFact]
        public void AChevron_IsTheSameIconAndSizeAndOpacity()
        {
            var chevron = Show(new Chevron());
            var icon = chevron.GetVisualDescendants().OfType<AppIcon>().Single();

            Assert.Equal("ChevronDown", icon.IconKey);
            Assert.Equal(Token("IconSizeXS"), icon.Size);
            Assert.Equal(0.5, chevron.Opacity);
        }

        private static ControlTheme Theme(string key) => (ControlTheme)Application.Current!.FindResource(key)!;

        public static TheoryData<string> Expanders => new() { "default", "ExpanderItem", "indented" };

        private static Control Expander(string kind) => kind switch
        {
            "indented" => new ExpanderIndented { Header = "Header", Content = new TextBlock { Text = "Content" } },
            "ExpanderItem" => new Expander { Theme = Theme("ExpanderItem"), Header = "Header", Content = new TextBlock { Text = "Content" } },
            _ => new Expander { Header = "Header", Content = new TextBlock { Text = "Content" } }
        };

        [AvaloniaTheory]
        [MemberData(nameof(Expanders))]
        public void EveryExpander_HasOneChevron_AndNoOtherArrow(string kind)
        {
            var view = Show(Expander(kind));

            var chevron = Assert.Single(view.GetVisualDescendants().OfType<Chevron>());
            Assert.Equal(-90, chevron.ClosedAngle);
            Assert.Equal(0, chevron.OpenAngle);
            Assert.DoesNotContain(view.GetVisualDescendants().OfType<Avalonia.Controls.Shapes.Path>(), path => path.Name == "arrow");
            Assert.All(
                view.GetVisualDescendants().OfType<AppIcon>().Where(icon => icon.GetVisualAncestors().OfType<Chevron>().Any() == false),
                icon => Assert.DoesNotContain("Chevron", icon.IconKey ?? string.Empty));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Expanders))]
        public void TheChevronOfAnExpander_FollowsIsExpanded(string kind)
        {
            var view = Show(Expander(kind));
            var chevron = view.GetVisualDescendants().OfType<Chevron>().Single();
            Assert.False(chevron.IsOpen);

            if (view is ExpanderIndented indented)
                indented.IsExpanded = true;
            else
                ((Expander)view).IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(chevron.IsOpen);
        }

        [AvaloniaFact]
        public void ADropDown_HasTheSameChevron_ThatTurnsUpWhenOpen()
        {
            var combo = Show(new ComboBox { ItemsSource = new[] { "A", "B" }, SelectedIndex = 0, Width = 150 });

            var chevron = Assert.Single(combo.GetVisualDescendants().OfType<Chevron>());
            Assert.Equal(0, chevron.ClosedAngle);
            Assert.Equal(180, chevron.OpenAngle);
            Assert.False(chevron.IsOpen);

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(chevron.IsOpen);
        }
    }
}
