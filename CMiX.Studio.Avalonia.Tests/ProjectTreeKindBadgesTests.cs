using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using AvaloniaColor = Avalonia.Media.Color;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // A collapsed row shows what it holds: a square for each kind, in the color of the kind, with how many. The tooltip names the kind.
    public class ProjectTreeKindBadgesTests : IDisposable
    {
        private Window? window;
        private readonly CategoryPalette palette = CategoryPalette.FromApplication();

        public void Dispose() => window?.Close();

        // A composition with two layers. The first layer has two entities, one filter and three modifiers.
        private (ListTreeProject View, ProjectTree Tree, TreeNode Layer, TreeNode Empty) Show()
        {
            var tree = CreateScene([2], s => AddKinds(s.Layer)).Tree;
            var screen = ListTreeTestScreen.Show(tree, 500, 600);
            window = screen.Window;
            var view = screen.View;
            tree.SelectedNode = null;
            Settle();
            return (view, tree, tree.CompositionNodes()[0].Children[0].Children[0], tree.CompositionNodes()[0].Children[0].Children[1]);
        }

        private void Settle() => ListTreeTestScreen.Settle(window!);

        // A row has two badges: one after the name box of an item, one after the name of a group. This gets the one for the node.
        private static BadgesKindCount BadgesOf(ListTreeProject view, TreeNode node) =>
            view.Rows.RowOf(node)!.GetVisualDescendants().OfType<BadgesKindCount>()
                .First(badges => (badges.FindAncestorOfType<TextBoxEditable>() != null) == (node.Item != null));

        private static AvaloniaColor FillOf(Border border) => ((ISolidColorBrush)border.Background!).Color;

        private static List<Border> SquaresOf(BadgesKindCount badges) =>
            badges.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("kindcount")).ToList();

        private static string TextIn(Border square) => square.GetVisualDescendants().OfType<TextBlock>().Single().Text!;

        [AvaloniaFact]
        public void Show_ACollapsedRow_ShowsASquareWithTheCountForEachKind()
        {
            var (view, _, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();

            var badges = BadgesOf(view, layer);
            var squares = SquaresOf(badges);

            Assert.True(badges.IsEffectivelyVisible);
            Assert.Equal(new[] { "2", "1", "3" }, squares.Select(TextIn));
            Assert.Equal(new[] { "2 Entities", "1 Filter", "3 Modifiers" }, squares.Select(square => (string)ToolTip.GetTip(square)!));
            Assert.Equal(
                new[] { TreeCategory.Entity, TreeCategory.Filter, TreeCategory.Modifier }.Select(category => palette.For(category).Kind),
                squares.Select(FillOf));
        }

        [AvaloniaFact]
        public void Show_AnOpenOrEmptyRow_ShowsNoSquares()
        {
            var (view, _, layer, empty) = Show();

            Assert.False(BadgesOf(view, layer).IsEffectivelyVisible);

            empty.IsExpanded = false;
            Settle();
            Assert.False(BadgesOf(view, empty).IsEffectivelyVisible);
            Assert.Empty(SquaresOf(BadgesOf(view, empty)));
        }

        [AvaloniaFact]
        public void Expand_ARow_HidesOrShowsTheSquares()
        {
            var (view, _, layer, _) = Show();

            layer.IsExpanded = false;
            Settle();
            Assert.True(BadgesOf(view, layer).IsEffectivelyVisible);

            layer.IsExpanded = true;
            Settle();
            Assert.False(BadgesOf(view, layer).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_ACollapsedGroup_ShowsItsCountInTheColorOfItsKind()
        {
            var (view, _, layer, _) = Show();
            var entities = layer.Children.Single(child => child.Group is EntitiesGroup);
            entities.IsExpanded = false;
            Settle();

            var square = Assert.Single(SquaresOf(BadgesOf(view, entities)));

            Assert.Equal("2", TextIn(square));
            Assert.Equal("2 Entities", ToolTip.GetTip(square));
            Assert.Equal(palette.For(TreeCategory.Entity).Kind, FillOf(square));
        }

        [AvaloniaFact]
        public void Select_ARow_KeepsTheSquaresTheSame()
        {
            var (view, tree, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();
            var badges = BadgesOf(view, layer);
            var tone = palette.For(TreeCategory.Entity);

            Assert.Equal(tone.Kind, FillOf(SquaresOf(badges).First()));

            // The pointer over the row does not change the squares: only the selection does.
            badges.IsHovered = true;
            Assert.Equal(tone.Kind, FillOf(SquaresOf(badges).First()));

            tree.SelectedNode = layer;
            Settle();
            Assert.Equal(tone.Kind, FillOf(SquaresOf(badges).First()));
        }

        private static AvaloniaColor CountColorOf(Border square) =>
            ((ISolidColorBrush)square.GetVisualDescendants().OfType<TextBlock>().Single().Foreground!).Color;

        private static AvaloniaColor ColorOf(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        // The count is always white: on the square at rest, under the pointer and on the full color of a selected row.
        [AvaloniaFact]
        public void Select_ARow_KeepsTheCountsWhite()
        {
            var (view, tree, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();
            var badges = BadgesOf(view, layer);

            Assert.All(SquaresOf(badges), square => Assert.Equal(ColorOf("ActiveForegroundBrush"), CountColorOf(square)));

            badges.IsHovered = true;
            Settle();
            Assert.All(SquaresOf(badges), square => Assert.Equal(ColorOf("ActiveForegroundBrush"), CountColorOf(square)));

            badges.IsHovered = false;
            tree.SelectedNode = layer;
            Settle();
            Assert.All(SquaresOf(badges), square => Assert.Equal(ColorOf("ActiveForegroundBrush"), CountColorOf(square)));
        }

        [AvaloniaFact]
        public void Show_ASquare_IsSixteenHighInTheMiddleOfTheRow()
        {
            var (view, _, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();
            var row = view.Rows.RowOf(layer)!.Band;
            var square = SquaresOf(BadgesOf(view, layer)).First();

            var top = square.TranslatePoint(new Point(0, 0), row)!.Value.Y;
            var bottom = row.Bounds.Height - (top + square.Bounds.Height);

            Assert.Equal(16, square.Bounds.Height);
            Assert.Equal(top, bottom, 1);
            Assert.True(top > 0);
        }

        private static Border MoreKindsOf(BadgesKindCount badges) =>
            badges.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "MoreKinds");

        [AvaloniaFact]
        public void Show_ARowWithFiveKinds_ShowsThreeSquaresAndPlusTwo()
        {
            var tree = CreateScene([1], s =>
            {
                AddKinds(s.Layer);
                s.Layer.CameraManager.AddItem(typeof(CMiX.Core.Rendering.Cameras.Camera));
                s.Layer.LightManager.AddItem(typeof(CMiX.Core.Rendering.Lights.LightEntity));
            }).Tree;
            var screen = ListTreeTestScreen.Show(tree, 500, 600);
            window = screen.Window;
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            layer.IsExpanded = false;
            Settle();

            var badges = BadgesOf(screen.View, layer);
            var more = MoreKindsOf(badges);

            Assert.Equal(new[] { "2", "1", "3" }, SquaresOf(badges).Select(TextIn));
            Assert.True(more.IsEffectivelyVisible);
            Assert.Equal("+2", TextIn(more));
            Assert.Equal("1 Camera" + Environment.NewLine + "1 Light", ToolTip.GetTip(more));
        }

        [AvaloniaFact]
        public void Show_ARowWithThreeKinds_ShowsNoPlusSquare()
        {
            var (view, _, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();

            Assert.False(MoreKindsOf(BadgesOf(view, layer)).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_ARowWithoutMissingAssets_ShowsNoMissingSign()
        {
            var (view, _, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();

            var sign = BadgesOf(view, layer).GetVisualDescendants().OfType<Border>().Single(border => border.Name == "MissingSign");

            Assert.False(layer.HasMissingAssets);
            Assert.False(sign.IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_TheMissingSign_IsTheOrangeWarningIcon()
        {
            var (view, _, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();

            var sign = BadgesOf(view, layer).GetVisualDescendants().OfType<Border>().Single(border => border.Name == "MissingSign");
            var icon = sign.GetVisualDescendants().OfType<AppIcon>().Single();

            Assert.Equal("Warning", icon.IconKey);
            Assert.Equal(ColorOf("PopupCardWarningBrush"), ((ISolidColorBrush)icon.Foreground!).Color);
            Assert.Empty(sign.GetVisualDescendants().OfType<global::Avalonia.Controls.Shapes.Path>());
        }

        [AvaloniaFact]
        public void Rename_ARow_MovesTheSquares()
        {
            var (view, _, layer, _) = Show();
            layer.IsExpanded = false;
            Settle();
            var row = view.Rows.RowOf(layer)!;
            var name = row.GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "textDisplay");
            var badges = BadgesOf(view, layer);

            var nameRight = name.TranslatePoint(new Point(name.Bounds.Width, 0), window!)!.Value.X;
            var badgesLeft = badges.TranslatePoint(new Point(0, 0), window!)!.Value.X;

            Assert.InRange(badgesLeft - nameRight, 0, 12);
        }
    }
}
