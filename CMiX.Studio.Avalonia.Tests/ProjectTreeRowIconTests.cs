using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The icon of a row has the color of the name. The kind of the row shows in the fill behind it.
    public class ProjectTreeRowIconTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        private (ProjectTreeView View, ProjectTree Tree) Show()
        {
            var tree = CreateScene([1]).Tree;
            var screen = TreeTestScreen.Show(tree, 400, 400, instant: false, settle: false);
            window = screen.Window;
            var view = screen.View;
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(60);
            Dispatcher.UIThread.RunJobs();
            return (view, tree);
        }


        private static Color ColorOf(IBrush? brush) => ((ISolidColorBrush)brush!).Color;

        [AvaloniaFact]
        public void Show_TheIconOfEveryRow_IsOpaqueInTheColorOfItsKindAndTheNameIsTheRestText()
        {
            var (view, tree) = Show();
            var foreground = ColorOf((IBrush)Application.Current!.FindResource("TreeTextRestBrush")!);
            var group = tree.CompositionNodes()[0].Children[0].Children[0].Children[0];
            var name = ItemOf(view, group).GetVisualDescendants().OfType<TextBlock>().First(text => text.Text == group.Header);

            foreach (var node in new[] { tree.CompositionNodes()[0], tree.CompositionNodes()[0].Children[0].Children[0] })
            {
                var icon = ItemOf(view, node).GetVisualDescendants().OfType<AppIcon>().First();
                Assert.Equal(1.0, icon.Opacity);
                Assert.Equal((Color)Application.Current!.FindResource(CMiX.Studio.Avalonia.Themes.CategoryPalette.ColorKey(node.Category!.Value, node.ComposableDepth))!, ColorOf(icon.Foreground));
            }

            var emptyGroup = ColorOf((IBrush)Application.Current!.FindResource("Grey700Brush")!);
            Assert.Equal(group.Children.Count == 0 ? emptyGroup : foreground, ColorOf(name.Foreground));
        }
    }
}
