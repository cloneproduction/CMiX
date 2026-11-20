using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The toggles of a row are icons inside the colored row, at its right end, with no fill. The visibility toggle is the
    // first slot and the mask toggle follows it.
    public class ProjectTreeRowTogglesTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        private void Settle() => TreeTestScreen.Settle(window!);

        // A composition with a layer, which has an entity.
        private (ProjectTreeView View, ProjectTree Tree) Show(double scale = 1.0)
        {
            var scene = CreateScene([1], s => s.Layer.ModelEntityManager.AddItem(typeof(Entity)));
            var screen = TreeTestScreen.Show(scene.Tree, 500, 600, scale);
            window = screen.Window;
            return (screen.View, scene.Tree);
        }

        private static TogglesRow ColumnOf(ProjectTreeView view, TreeNode node) =>
            ItemOf(view, node).GetVisualDescendants().OfType<TogglesRow>().First();

        // The squares from the left: the mask first, the visibility last.
        private List<Border> SquaresOf(TogglesRow column) =>
            column.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot") && border.IsEffectivelyVisible)
                .OrderBy(border => BoundsIn(border).Left).ToList();

        private Rect BoundsIn(Control control) => new(control.TranslatePoint(new Point(0, 0), window!)!.Value, control.Bounds.Size);

        [AvaloniaFact]
        public void Show_TheToggles_AreInsideTheColoredRow()
        {
            var (view, tree) = Show();

            foreach (var node in All(tree.Roots).Where(node => node.Item != null))
            {
                var row = BoundsIn(RowOf(view, node));
                foreach (var square in SquaresOf(ColumnOf(view, node)))
                    Assert.True(BoundsIn(square).Left >= row.Left && BoundsIn(square).Right <= row.Right + 0.01, $"A toggle of {node.Header} is outside the colored row.");
            }
        }

        [AvaloniaFact]
        public void Show_TheToggles_PutTheMaskFirstAndTheEyeLast()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var squares = SquaresOf(ColumnOf(view, composition));

            Assert.Equal(2, squares.Count);
            var eye = squares[1].GetVisualDescendants().OfType<ToggleButton>().Single();
            var mask = squares[0].GetVisualDescendants().OfType<ToggleButton>().Single();
            Assert.Same(Application.Current!.FindResource("ToggleButtonVisibility"), eye.Theme);
            Assert.Same(Application.Current!.FindResource("ToggleButtonMask"), mask.Theme);
            Assert.True(BoundsIn(squares[0]).Right <= BoundsIn(squares[1]).Left);
        }

        [AvaloniaFact]
        public void Show_AnEntity_HasOnlyTheEyeAtThePlaceOfTheCompositionEye()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entities = layer.Children[0];
            var entity = entities.Children[0];

            // A group has two empty squares, an entity has the eye and an empty square. A square without a button is only a place.
            var groupSquares = SquaresOf(ColumnOf(view, entities));
            Assert.Equal(2, groupSquares.Count);
            Assert.All(groupSquares, square => Assert.Empty(square.GetVisualDescendants().OfType<ToggleButton>().Where(button => button.IsEffectivelyVisible)));

            var entitySquares = SquaresOf(ColumnOf(view, entity));
            Assert.Equal(2, entitySquares.Count);
            Assert.Single(entitySquares[1].GetVisualDescendants().OfType<ToggleButton>().Where(button => button.IsEffectivelyVisible));
            Assert.Empty(entitySquares[0].GetVisualDescendants().OfType<ToggleButton>().Where(button => button.IsEffectivelyVisible));
            Assert.Equal(BoundsIn(SquaresOf(ColumnOf(view, composition))[0]).Left, BoundsIn(entitySquares[0]).Left);
            Assert.Equal(BoundsIn(SquaresOf(ColumnOf(view, composition))[1]).Left, BoundsIn(entitySquares[1]).Left);
        }

        [AvaloniaFact]
        public void Show_EveryColoredRow_EndsAtTheSamePlace()
        {
            var (view, tree) = Show();

            var rights = All(tree.Roots).Select(node => BoundsIn(RowOf(view, node)).Right).Distinct().ToList();

            Assert.Single(rights);
        }

        [AvaloniaTheory]
        [InlineData(1.0)]
        [InlineData(1.25)]
        [InlineData(1.5)]
        public void Show_TheSquares_AreARowHighWithTheSpaceOfTheRows(double scale)
        {
            var (view, tree) = Show(scale);
            var composition = tree.CompositionNodes()[0];
            var row = BoundsIn(RowOf(view, composition));
            var squares = SquaresOf(ColumnOf(view, composition)).Select(BoundsIn).ToList();
            var space = Convert.ToDouble(((Control)view).Resources["TreeRowSpacing"]);

            Assert.Equal(row.Right, squares[1].Right, 3);
            Assert.Equal(space, squares[1].Left - squares[0].Right, 3);
            Assert.All(squares, square =>
            {
                Assert.Equal(row.Height, square.Height, 3);
                Assert.Equal(row.Height, square.Width, 3);
                Assert.Equal(row.Top, square.Top, 3);
            });
        }

        [AvaloniaFact]
        public void Show_TheToggle_IsInTheMiddleOfItsSquare()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];

            // The mask sits at the right of its square (see TogglesComposable), so only the eye is in the middle of its square.
            foreach (var square in SquaresOf(ColumnOf(view, composition)).Skip(1))
            {
                var toggle = BoundsIn(square.GetVisualDescendants().OfType<ToggleButton>().Single());
                var box = BoundsIn(square);

                Assert.Equal(toggle.Left - box.Left, box.Right - toggle.Right, 3);
                Assert.Equal(toggle.Top - box.Top, box.Bottom - toggle.Bottom, 3);
                Assert.Equal(toggle.Left - box.Left, toggle.Top - box.Top, 3);
                Assert.True(toggle.Left - box.Left > 0);
            }
        }

        [AvaloniaFact]
        public void Show_TheMask_IsAboutAsFarFromTheEyeAsTheEyeIsFromTheEdgeOfTheRow()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var squares = SquaresOf(ColumnOf(view, composition));
            var mask = BoundsIn(squares[0].GetVisualDescendants().OfType<ToggleButton>().Single());
            var eye = BoundsIn(squares[1].GetVisualDescendants().OfType<ToggleButton>().Single());
            var row = BoundsIn(RowOf(view, composition));

            var edge = row.Right - eye.Right;
            Assert.InRange(eye.Left - mask.Right, edge - 1.5, edge + 1.5);
        }

        [AvaloniaFact]
        public void Click_TheHideButtonOfAGroup_HidesTheListAndKeepsTheItemsAsTheyAre()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var entities = layer.Children.First(child => child.Group is EntitiesGroup);
            entities.IsExpanded = false;
            Settle();
            var items = entities.Group!.Manager.ManagerData.Items;
            var before = items.Select(item => ((IPrefab)item).PrefabService.Visibility.Value).ToList();

            var button = ColumnOf(view, entities).GetVisualDescendants().OfType<ToggleButton>().Single(toggle => toggle.Name == "groupHideToggle");
            Assert.True(button.IsEffectivelyVisible);
            button.IsChecked = true;

            Assert.True(entities.Group!.Manager.Hidden.Value);
            Assert.Equal(before, items.Select(item => ((IPrefab)item).PrefabService.Visibility.Value));

            button.IsChecked = false;
            Assert.False(entities.Group.Manager.Hidden.Value);
        }

        [AvaloniaFact]
        public void Click_TheHideButtonOfAGroup_GreysTheVisibilityTogglesBelowButLeavesThemEnabled()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var entities = layer.Children.First(child => child.Group is EntitiesGroup);
            var entity = entities.Children[0];
            ToggleButton EyeOf(TreeNode node) =>
                ColumnOf(view, node).GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name is "visibilityToggle" or "groupHideToggle" && toggle.IsEffectivelyVisible);

            Assert.False(entity.IsUnderHiddenParent);
            Assert.Equal(1, EyeOf(entity).Opacity);

            entities.Group!.Manager.Hidden.Value = true;
            Settle();

            // The rows below look disabled, but they are not disabled. The button of the group itself is as it was.
            Assert.True(entity.IsUnderHiddenParent);
            Assert.Equal(0.5, EyeOf(entity).Opacity);
            Assert.True(EyeOf(entity).IsEffectivelyEnabled);
            Assert.False(entities.IsUnderHiddenParent);
            Assert.Equal(1, EyeOf(entities).Opacity);
            Assert.Equal(1, EyeOf(layer).Opacity);

            entities.Group.Manager.Hidden.Value = false;
            Settle();

            Assert.False(entity.IsUnderHiddenParent);
            Assert.Equal(1, EyeOf(entity).Opacity);
        }

        [AvaloniaFact]
        public void Hide_AnItem_GreysEveryRowBelowItAtEveryDepth()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entities = layer.Children.First(child => child.Group is EntitiesGroup);
            var entity = entities.Children[0];
            ToggleButton EyeOf(TreeNode node) =>
                ColumnOf(view, node).GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name is "visibilityToggle" or "groupHideToggle" && toggle.IsEffectivelyVisible);

            // A hidden layer greys its groups and their items, and not itself or its composition.
            ((CMiX.Core.Prefabs.IPrefab)layer.Item!).PrefabService.Visibility.Value = true;
            Settle();

            Assert.True(layer.IsHidden);
            Assert.False(layer.IsUnderHiddenParent);
            Assert.False(composition.IsUnderHiddenParent);
            Assert.True(entities.IsUnderHiddenParent);
            Assert.True(entity.IsUnderHiddenParent);
            Assert.Equal(1, EyeOf(layer).Opacity);
            Assert.Equal(0.5, EyeOf(entities).Opacity);
            Assert.Equal(0.5, EyeOf(entity).Opacity);
            Assert.True(EyeOf(entity).IsEffectivelyEnabled);

            // Two hidden parents: the row stays greyed until both show again.
            entities.Group!.Manager.Hidden.Value = true;
            ((CMiX.Core.Prefabs.IPrefab)layer.Item!).PrefabService.Visibility.Value = false;
            Settle();
            Assert.True(entity.IsUnderHiddenParent);
            Assert.False(entities.IsUnderHiddenParent);

            entities.Group.Manager.Hidden.Value = false;
            Settle();
            Assert.False(entity.IsUnderHiddenParent);
            Assert.Equal(1, EyeOf(entity).Opacity);
        }

        [AvaloniaFact]
        public void Show_AnItemRowAndACompositionRow_HaveNoHideButtonOfAGroup()
        {
            var (view, tree) = Show();

            foreach (var node in new[] { tree.CompositionNodes()[0], tree.CompositionNodes()[0].Children[0].Children[0] })
                Assert.False(ColumnOf(view, node).GetVisualDescendants().OfType<ToggleButton>().Single(toggle => toggle.Name == "groupHideToggle").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_TheSlots_HaveNoFill()
        {
            var (view, tree) = Show();
            var expected = global::Avalonia.Media.Colors.Transparent;

            foreach (var square in SquaresOf(ColumnOf(view, tree.CompositionNodes()[0])))
                Assert.Equal(expected, ((ISolidColorBrush)square.Background!).Color);
        }

        [AvaloniaFact]
        public void Show_ASlotWithNoButton_HasNoFillEither()
        {
            var (view, tree) = Show();
            var entities = tree.CompositionNodes()[0].Children[0].Children[0].Children[0];
            var clear = global::Avalonia.Media.Colors.Transparent;
            var entity = entities.Children[0];

            // A group has two empty slots. An entity has the eye in a slot, then an empty slot. None has a fill.
            Assert.All(SquaresOf(ColumnOf(view, entities)), square => Assert.Equal(clear, ((ISolidColorBrush)square.Background!).Color));
            var entitySquares = SquaresOf(ColumnOf(view, entity));
            Assert.Equal(clear, ((ISolidColorBrush)entitySquares[0].Background!).Color);
            Assert.Equal(clear, ((ISolidColorBrush)entitySquares[1].Background!).Color);
        }

        [AvaloniaFact]
        public void Show_TheColumn_HasTheWidthOfTheSpaceAndTheSquares()
        {
            var (view, tree) = Show();
            var column = ColumnOf(view, tree.CompositionNodes()[0]);
            var row = BoundsIn(RowOf(view, tree.CompositionNodes()[0]));

            Assert.Equal(TogglesRow.WidthFor(row.Height, Convert.ToDouble(view.Resources["TreeRowSpacing"])), column.Bounds.Width, 3);
        }
    }
}
