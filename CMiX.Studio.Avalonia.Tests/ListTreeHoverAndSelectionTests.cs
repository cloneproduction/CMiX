// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;

namespace CMiX.Studio.Avalonia.Tests
{
    // The hover and the selection of the list tree: surfaces, left borders and text, measured on the rows and the card areas.
    public class ListTreeHoverAndSelectionTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeHoverAndSelectionTests()
        {
            scene.Tree.SelectedNode = null;
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private FrameListTreeCard AreaOf(TreeNode node) => Assert.IsType<FrameListTreeCard>(view.Rows.Surfaces.CardAreaOf(ListTreeKeys.Of(node)));

        private static double LeftIn(Visual part, Visual root) => part.TranslatePoint(new Point(0, 0), root)!.Value.X;

        // A point of the row between the name and the toggles.
        private Point BlankOf(RowListTree row) => row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height / 2), window)!.Value;

        private void Select(TreeNode node)
        {
            scene.Tree.SelectedNode = node;
            window.Settle();
        }

        private static void AssertMark(Border mark, Color color, double left, Visual root)
        {
            Assert.True(mark.IsEffectivelyVisible);
            Assert.Equal(3, mark.Bounds.Width);
            Assert.Equal(color, FillOf(mark.Background));
            Assert.Equal(left, LeftIn(mark, root));
        }

        [AvaloniaFact]
        public void Show_AnItemAtRest_HasTheRestBandAndNoMark()
        {
            var row = RowOf(Entity);

            Assert.Equal(Colors.Transparent, FillOf(row.Background));
            Assert.Equal(Token("ListTreeRowRestBandBrush"), FillOf(Named(row, "band").Background));
            Assert.False(Named(row, "mark").IsEffectivelyVisible);
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(row));
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
        }

        [AvaloniaFact]
        public void Hover_AnItem_LightsTheRowFullWidthWithAHoverBorderAtTheLeftEdge()
        {
            var row = RowOf(Entity);

            window.Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowHoverBrush"), FillOf(row.Background));
            Assert.Equal(Colors.Transparent, FillOf(Named(row, "band").Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).HoverBorder, 0, row);
            Assert.Equal(Token("ListTreeTextActiveBrush"), TextOf(row));
            Assert.Equal(1, row.Opacity);
        }

        [AvaloniaFact]
        public void Hover_TheToggles_LightTheRow()
        {
            var row = RowOf(Entity);

            window.Hover(row.Toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.IsEffectivelyVisible));

            Assert.Equal(Token("ListTreeRowHoverBrush"), FillOf(row.Background));
        }

        [AvaloniaFact]
        public void Hover_TheGapUnderARow_LightsNoRow()
        {
            var row = RowOf(Entity);
            var gap = row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height + 1.5), window)!.Value;

            window.Hover(gap);

            Assert.All(view.Rows.Layout.Places, place => Assert.False(view.Rows.RowOf(place.Key)!.IsPointerOver));
            Assert.Equal(Colors.Transparent, FillOf(row.Background));
            Assert.All(view.Rows.Layout.Places.Where(place => place.CanToggle),
                place => Assert.Equal(Colors.Transparent, FillOf(view.Rows.Surfaces.CardAreaOf(place.Key)!.Surface.Background)));
        }

        [AvaloniaFact]
        public void Hover_ACard_LightsOnlyItsHeaderRow()
        {
            var row = RowOf(Layer);

            window.Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowHoverBrush"), FillOf(row.Background));
            Assert.Equal(Colors.Transparent, FillOf(Named(row, "band").Background));
            AssertMark(Named(row, "mark"), KindOf(Layer).HoverBorder, 0, row);
            Assert.Equal(row.Bounds.Height, Named(row, "mark").Bounds.Height);
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
            Assert.Equal(Token("ListTreeTextActiveBrush"), TextOf(row));
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(RowOf(Entity)));
        }

        [AvaloniaFact]
        public void Select_AnItem_LightsTheRowWithTheKindBorderAndWhiteText()
        {
            Select(Entity);
            var row = RowOf(Entity);

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).SelectionBorder, 0, row);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
            Assert.NotEqual(Token("ListTreeTextSelectedBrush"), FillOf(row.GetVisualDescendants().OfType<AppIcon>().First(icon => icon.Name == "kindIcon").Foreground));
        }

        [AvaloniaFact]
        public void Select_ACard_LightsItsHeaderRowAndLeavesItsChildrenAtRest()
        {
            Select(Layer);
            var row = RowOf(Layer);

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Layer).SelectionBorder, 0, row);
            Assert.Equal(row.Bounds.Height, Named(row, "mark").Bounds.Height);
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(RowOf(Entities)));
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(RowOf(Entity)));
            Assert.Equal(Colors.Transparent, FillOf(RowOf(Entity).Background));
        }

        [AvaloniaFact]
        public void Row_TheInspectedRow_HasTheInspectedTint()
        {
            scene.Tree.Inspect(Entity);
            window.Settle();
            var row = RowOf(Entity);
            var toggle = row.Toggles.GetVisualDescendants().OfType<ToggleButton>().First(button => button.IsEffectivelyVisible);

            Assert.Equal(KindOf(Entity).Inspected, FillOf(row.Background));
            Assert.False(Named(row, "mark").IsEffectivelyVisible);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), FillOf(toggle.Foreground));

            window.Hover(BlankOf(row));

            Assert.Equal(KindOf(Entity).InspectedHover, FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).HoverBorder, 0, row);
        }

        [AvaloniaFact]
        public void Row_ASelectedInspectedRow_KeepsTheSelectionBorder()
        {
            Select(Entity);
            scene.Tree.Inspect(Entity);
            window.Settle();
            var row = RowOf(Entity);

            Assert.Equal(KindOf(Entity).Inspected, FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).SelectionBorder, 0, row);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
        }

        [AvaloniaFact]
        public void Hover_ASelectedItem_IsTwentyPercentAndKeepsTheSelectionBorder()
        {
            Select(Entity);
            var row = RowOf(Entity);

            window.Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowSelectedHoverBrush"), FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).SelectionBorder, 0, row);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
        }

        [AvaloniaFact]
        public void Hover_ASelectedCard_IsTwentyPercentAndKeepsTheSelectionBorder()
        {
            Select(Layer);
            var row = RowOf(Layer);

            window.Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowSelectedHoverBrush"), FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Layer).SelectionBorder, 0, row);
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
        }

        [AvaloniaFact]
        public void Hover_AnItemInASelectedCard_LightsTheItemAsAHover()
        {
            Select(Layer);
            var row = RowOf(Entity);

            window.Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(RowOf(Layer).Background));
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
            Assert.Equal(Token("ListTreeRowHoverBrush"), FillOf(row.Background));
            Assert.Equal(Colors.Transparent, FillOf(Named(row, "band").Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).HoverBorder, 0, row);
            Assert.Equal(Token("ListTreeTextActiveBrush"), TextOf(row));
        }

        [AvaloniaFact]
        public void Hover_ACardInASelectedCard_LightsOnlyItsHeaderRow()
        {
            Select(Layer);
            var row = RowOf(Entities);

            window.Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(RowOf(Layer).Background));
            Assert.Equal(Token("ListTreeRowHoverBrush"), FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Entities).HoverBorder, 0, row);
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Entities).Surface.Background));
        }

        [AvaloniaFact]
        public void Show_NestedCards_PaintTheInnerCardAbove()
        {
            Entities.IsExpanded = false;
            window.Settle();
            Entities.IsExpanded = true;
            window.Settle();

            Assert.True(AreaOf(Entities).ZIndex > AreaOf(Layer).ZIndex);
            var highestArea = view.Rows.Surfaces.Areas.Max(area => area.ZIndex);
            Assert.All(view.Rows.RealizedRows, row => Assert.True(row.ZIndex > highestArea));
        }

        [AvaloniaFact]
        public void Arrange_TheCardAreas_AreUnderTheRowsAndSpanTheirCards()
        {
            var area = AreaOf(Layer);
            var place = area.Place!;

            Assert.Equal(ListTreeLayers.CardArea + place.Depth, area.ZIndex);
            Assert.True(area.ZIndex < RowOf(Layer).ZIndex);
            Assert.Equal(place.Y, area.Bounds.Y);
            Assert.Equal(place.AreaHeight, area.Bounds.Height);
            Assert.Equal(view.Rows.Bounds.Width, area.Bounds.Width);
            Assert.False(area.IsHitTestVisible);
        }

        [AvaloniaFact]
        public void Click_ARow_SelectsItsNodeAndMovesTheCursor()
        {
            window.Click(BlankOf(RowOf(Entity)));

            Assert.Same(Entity, scene.Tree.SelectedNode);
            Assert.Same(Entity, scene.Tree.CursorNode);
            Assert.True(RowOf(Entity).IsSelected);
        }

        [AvaloniaFact]
        public void Click_TheIconOfAnItem_SelectsIt()
        {
            window.Click(RowOf(Entity).IconHit);

            Assert.Same(Entity, scene.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_TheIconOfACard_OnlyOpensOrClosesIt()
        {
            window.Click(RowOf(Layer).IconHit);

            Assert.False(Layer.IsExpanded);
            Assert.Null(scene.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_AToggle_OnlyDoesTheToggle()
        {
            var visibility = RowOf(Entity).Toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "visibilityToggle");
            var before = visibility.IsChecked;

            window.Click(visibility);

            Assert.NotEqual(before, visibility.IsChecked);
            Assert.Null(scene.Tree.SelectedNode);
        }
    }
}
