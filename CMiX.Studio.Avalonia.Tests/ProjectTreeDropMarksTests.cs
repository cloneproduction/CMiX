using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using AvaloniaColor = Avalonia.Media.Color;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // During a drag the rows that can take the node have a frame, the other rows are covered, and nothing moves.
    public class ProjectTreeDropMarksTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // A composition with two layers. The first has two entities. The second has one.
        private (TreeViewProject View, ProjectTree Tree) Show()
        {
            var scene = CreateScene([2], s =>
            {
                s.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
                s.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
                s.LayerOfA(0).TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
                s.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
            });
            var screen = TreeTestScreen.Show(scene.Tree, 500, 700);
            window = screen.Window;
            return (screen.View, scene.Tree);
        }

        private void Settle() => TreeTestScreen.Settle(window!);

        private static TreeNode FirstEntity(ProjectTree tree) =>
            tree.CompositionNodes()[0].Children[0].Children[0].Children.Single(child => child.Group is EntitiesGroup).Children[0];


        private static CategoryTone ToneOf(TreeNode node) =>
            CategoryPalette.FromApplication().For(node.Category!.Value, node.ComposableDepth, node.KindDepth);

        [AvaloniaFact]
        public void BeginDrag_ANode_KeepsEveryFillAndCoversTheRowsThatTakeNothing()
        {
            var (view, tree) = Show();
            var source = FirstEntity(tree);

            tree.BeginDrag(source);
            Settle();

            foreach (var node in All(tree.Roots))
            {
                Assert.Equal(ExpectedFill(node, ToneRole.Fill), FillOf(view, node));
                Assert.Equal(node.IsDimmed, Part(view, node, "DimLayer").Classes.Contains("dimmed"));
            }

            Assert.True(All(tree.Roots).Any(node => node.IsDropCandidate));
            Assert.True(All(tree.Roots).Any(node => node.IsDimmed));
            Assert.True(Part(view, source, "DimLayer").Classes.Contains("dimmed"));
        }

        [AvaloniaFact]
        public void EndDrag_AfterADrag_ClearsEveryMark()
        {
            var (view, tree) = Show();
            tree.BeginDrag(FirstEntity(tree));
            Settle();

            tree.EndDrag();
            Settle();

            foreach (var node in All(tree.Roots))
            {
                Assert.Equal(ExpectedFill(node, ToneRole.Fill), FillOf(view, node));
                Assert.False(Part(view, node, "DimLayer").Classes.Contains("dimmed"));
            }
        }

        [AvaloniaFact]
        public void BeginDrag_ANode_DrawsNoRowBorder()
        {
            var (view, tree) = Show();
            tree.BeginDrag(FirstEntity(tree));
            Settle();

            foreach (var node in All(tree.Roots))
            {
                var borders = ItemOf(view, node).GetVisualDescendants().OfType<Border>().Where(border => border.Name is not ("DropLine" or "CursorFrame"));
                Assert.All(borders, border => Assert.Equal(new Thickness(0), border.BorderThickness));
            }

            Assert.DoesNotContain(All(tree.Roots), node => ItemOf(view, node).GetVisualDescendants().OfType<Border>().Any(border => border.Name is "CandidateFrame" or "HintFrame"));
        }

        [AvaloniaFact]
        public void BeginDrag_ACollapsedRowWithACandidateUnderIt_LeavesTheRowUncovered()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            composition.IsExpanded = false;
            Settle();

            tree.BeginDrag(FirstEntity(tree));
            Settle();

            Assert.True(composition.IsDropHint);
            Assert.False(Part(view, composition, "DimLayer").Classes.Contains("dimmed"));
            Assert.Contains("drophint", Part(view, composition, "DimLayer").Classes);
            Assert.Equal(0.3, (double)view.FindResource("TreeHintCoverOpacity")!);
            Assert.Equal(ToneOf(composition).Fill, FillOf(view, composition));
        }

        // A modifier of an entity: a layer cannot hold it, so the layer rows are blocked. They are covered like any row that takes nothing.
        [AvaloniaFact]
        public void BlockedRow_DuringADrag_IsCoveredAndKeepsItsReasonForTheTooltip()
        {
            var (view, tree) = Show();
            ((Entity)FirstEntity(tree).Item!).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            Settle();
            var modifier = FirstEntity(tree).Children.Single(child => child.Group is ModifiersGroup).Children[0];
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];
            Assert.False(Part(view, layer, "DimLayer").Classes.Contains("dimmed"));

            tree.BeginDrag(modifier);
            Settle();

            Assert.True(layer.IsDropBlocked);
            Assert.NotNull(layer.DropBlockReason);
            Assert.Equal(ToneOf(layer).Fill, FillOf(view, layer));
            Assert.True(Part(view, layer, "DimLayer").Classes.Contains("dimmed"));
            Assert.DoesNotContain(ItemOf(view, layer).GetVisualDescendants().OfType<Border>(), border => border.Name == "BlockedFrame");

            tree.EndDrag();
            Settle();
            Assert.False(Part(view, layer, "DimLayer").Classes.Contains("dimmed"));
        }

        [AvaloniaFact]
        public void Show_TheCover_UsesTheThemeColorsAndTakesNoClick()
        {
            var (view, tree) = Show();
            var source = FirstEntity(tree);
            tree.BeginDrag(source);
            Settle();
            var cover = Part(view, tree.CompositionNodes()[0], "DimLayer");

            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("TreeBackgroundBrush")!).Color, ((ISolidColorBrush)cover.Background!).Color);
            Assert.Contains("dimmed", cover.Classes);
            Assert.Contains(cover.Transitions!, transition => transition is global::Avalonia.Animation.DoubleTransition { Property: var property } && property == Visual.OpacityProperty);
            Assert.Contains(cover.Transitions!, transition => transition is global::Avalonia.Animation.DoubleTransition { Duration: var duration } && duration == CMiX.UI.Themes.Motion.FadeDuration);
            Assert.False(cover.IsHitTestVisible);
        }

        [AvaloniaFact]
        public void BeginDrag_ANode_KeepsThePlaceAndTheSizeOfEveryRow()
        {
            var (view, tree) = Show();
            var before = All(tree.Roots).Select(node => (Part(view, node, "RowBorder").Bounds, Part(view, node, "RowBorder").TranslatePoint(new Point(0, 0), window!))).ToList();

            tree.BeginDrag(FirstEntity(tree));
            Settle();
            var during = All(tree.Roots).Select(node => (Part(view, node, "RowBorder").Bounds, Part(view, node, "RowBorder").TranslatePoint(new Point(0, 0), window!))).ToList();

            Assert.Equal(before, during);
        }

        // The group that is lit takes the node: a drop on it moves the filter to the list of that group.
        [AvaloniaFact]
        public void Drop_OnTheLitGroupOfFilters_MovesTheFilterThere()
        {
            var (view, tree) = Show();
            var firstLayer = tree.CompositionNodes()[0].Children[0].Children[0];
            var secondLayer = tree.CompositionNodes()[0].Children[0].Children[1];
            var filter = firstLayer.Children.Single(child => child.Group is FiltersGroup).Children[0];
            var target = secondLayer.Children.Single(child => child.Group is FiltersGroup);
            var lists = ((Layer)firstLayer.Item!, (Layer)secondLayer.Item!);
            var point = Center(view, target);

            TreeDrag.Over(window!, Center(view, filter), point);
            Assert.True(tree.IsDragging);
            Assert.Contains(":droptarget", ItemOf(view, target).Classes);
            Assert.True(Part(view, target, "DropInsideFrame").IsEffectivelyVisible);
            TreeDrag.Release(window!, point);

            Assert.Empty(lists.Item1.TextureModifierManager.ManagerData.Items);
            Assert.Single(lists.Item2.TextureModifierManager.ManagerData.Items);
        }

        // A drop inside a row shows the cursor frame of TreeSelectionThickness inside the row. The fill stays the fill at rest.
        [AvaloniaFact]
        public void DragOver_InsideARow_GivesItTheCursorFrameAndKeepsTheFill()
        {
            var (view, tree) = Show();
            var firstLayer = tree.CompositionNodes()[0].Children[0].Children[0];
            var secondLayer = tree.CompositionNodes()[0].Children[0].Children[1];
            var filter = firstLayer.Children.Single(child => child.Group is FiltersGroup).Children[0];
            var target = secondLayer.Children.Single(child => child.Group is FiltersGroup);
            var point = Center(view, target);
            var hidden = Part(view, target, "DropInsideFrame");
            Assert.False(hidden.IsEffectivelyVisible);
            Assert.Equal(new Thickness(0), hidden.BorderThickness);

            TreeDrag.Over(window!, Center(view, filter), point);

            var frame = Part(view, target, "DropInsideFrame");
            var row = Part(view, target, "RowBorder");
            Assert.True(frame.IsEffectivelyVisible);
            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("CursorFrameBrush")!).Color, ((ISolidColorBrush)frame.BorderBrush!).Color);
            Assert.Equal((Thickness)view.FindResource("TreeSelectionThickness")!, frame.BorderThickness);
            Assert.Equal(row.Bounds.Size, frame.Bounds.Size);
            Assert.False(frame.IsHitTestVisible);
            Assert.Equal(ExpectedFill(target, ToneRole.Fill), FillOf(view, target));
            TreeDrag.Cancel(window!, point);
        }

        // The cursor frame never hides the drop frame: the drop frame is drawn after it and is not thinner.
        [AvaloniaFact]
        public void DragOver_InsideTheCursorRow_DrawsTheAccentFrameAboveTheHiddenCursorFrame()
        {
            var (view, tree) = Show();
            var firstLayer = tree.CompositionNodes()[0].Children[0].Children[0];
            var secondLayer = tree.CompositionNodes()[0].Children[0].Children[1];
            var filter = firstLayer.Children.Single(child => child.Group is FiltersGroup).Children[0];
            var target = secondLayer.Children.Single(child => child.Group is FiltersGroup);
            var point = Center(view, target);

            TreeDrag.Over(window!, Center(view, filter), point);
            tree.CursorNode = target;
            Settle();

            Assert.Contains(":droptarget", ItemOf(view, target).Classes);
            var drop = Part(view, target, "DropInsideFrame");
            var cursor = Part(view, target, "CursorFrame");
            var layers = ((Panel)drop.GetVisualParent()!).Children;
            Assert.Same(layers, ((Panel)cursor.GetVisualParent()!).Children);
            Assert.True(layers.IndexOf(drop) > layers.IndexOf(cursor));
            Assert.True(drop.IsEffectivelyVisible);
            Assert.True(drop.BorderThickness.Top >= cursor.BorderThickness.Top);
            TreeDrag.Cancel(window!, point);
        }

        // The center of the row of the node, in the window.
        private Point Center(TreeViewProject view, TreeNode node)
        {
            var row = Part(view, node, "RowBorder");
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), window!)!.Value;
        }

        [AvaloniaFact]
        public void Drag_WithThePointer_StartsTheTreeDragAndMarksTheRows()
        {
            var (view, tree) = Show();
            var source = FirstEntity(tree);
            var point = Center(view, source);

            TreeDrag.Over(window!, point, point + new Vector(30, 0));

            Assert.True(tree.IsDragging);
            Assert.True(source.IsDragSource);
            Assert.True(All(tree.Roots).Any(node => node.IsDropCandidate));

            // The pointer is released on the dragged row itself: no drop, the drag ends, and no row keeps a mark.
            TreeDrag.Release(window!, point + new Vector(30, 0));
            Assert.False(tree.IsDragging);
            Assert.All(All(tree.Roots), node => Assert.False(node.IsDragActive));
        }

        [AvaloniaFact]
        public void Press_OnTheToggleColumn_StartsNoDrag()
        {
            var (view, tree) = Show();
            var source = FirstEntity(tree);
            var toggles = ItemOf(view, source).GetVisualDescendants().OfType<TogglesRow>().First();

            // The right end of the column: an entity has no mask toggle there, so it is the space around the buttons.
            var point = toggles.TranslatePoint(new Point(toggles.Bounds.Width - 3, toggles.Bounds.Height / 2), window!)!.Value;
            TreeDrag.Over(window!, point, point + new Vector(30, 0));

            var started = tree.IsDragging;
            TreeDrag.Release(window!, point + new Vector(30, 0));

            Assert.False(started);
        }
    }
}
