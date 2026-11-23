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
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The hover and the selection of the new list tree: surfaces, left borders and text, measured on the rows and the card areas.
    public class ListTreeHoverAndSelectionTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeHoverAndSelectionTests()
        {
            scene.Tree.SelectedNode = null;
            view = new ListTreeProject { DataContext = scene.Tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            window = new Window { Content = view, Width = 400, Height = 900 };
            window.Show();
            Settle();
        }

        public void Dispose() => window.Close();

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private void Settle() => TreeTestScreen.Settle(window);

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private FrameListTreeCard AreaOf(TreeNode node) => Assert.IsType<FrameListTreeCard>(view.Rows.Surfaces.CardAreaOf(ListTreeKeys.Of(node)));

        private static Border Named(Control root, string name) => root.GetVisualDescendants().OfType<Border>().First(border => border.Name == name);

        private static Color FillOf(IBrush? brush) => brush is ISolidColorBrush solid ? solid.Color : Colors.Transparent;

        private static Color Token(string key) => FillOf((IBrush)Application.Current!.FindResource(key)!);

        private static Color TextOf(RowListTree row) =>
            FillOf(row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault(box => box.IsVisible)?.Foreground
                ?? row.GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "groupName").Foreground);

        private static ListTreeKindColors KindOf(TreeNode node) => CategoryPalette.FromApplication().ForListTree(node.Category!.Value, node.ComposableDepth);

        private static double LeftIn(Visual part, Visual root) => part.TranslatePoint(new Point(0, 0), root)!.Value.X;

        private Point CenterOf(Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        // A point of the row between the name and the toggles.
        private Point BlankOf(RowListTree row) => row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height / 2), window)!.Value;

        private void Hover(Point point)
        {
            TreeDrag.Render(window);
            window.MouseMove(point);
            Settle();
        }

        private void Click(Point point)
        {
            TreeDrag.Render(window);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Settle();
        }

        private void Select(TreeNode node)
        {
            scene.Tree.SelectedNode = node;
            Settle();
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
            Assert.False(AreaOf(Layer).Mark.IsEffectivelyVisible);
            Assert.Equal(Colors.Transparent, FillOf(AreaOf(Layer).Surface.Background));
        }

        [AvaloniaFact]
        public void Hover_AnItem_LightsTheRowFullWidthWithAHoverBorderAtTheLeftEdge()
        {
            var row = RowOf(Entity);

            Hover(BlankOf(row));

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

            Hover(CenterOf(row.Toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.IsEffectivelyVisible)));

            Assert.Equal(Token("ListTreeRowHoverBrush"), FillOf(row.Background));
        }

        [AvaloniaFact]
        public void Hover_TheGapUnderARow_LightsNoRow()
        {
            var row = RowOf(Entity);
            var gap = row.TranslatePoint(new Point(row.Bounds.Width - 100, row.Bounds.Height + 1.5), window)!.Value;

            Hover(gap);

            Assert.All(view.Rows.Layout.Places, place => Assert.False(view.Rows.RowOf(place.Key)!.IsPointerOver));
            Assert.Equal(Colors.Transparent, FillOf(row.Background));
            Assert.All(view.Rows.Layout.Places.Where(place => place.CanToggle),
                place => Assert.Equal(Colors.Transparent, FillOf(view.Rows.Surfaces.CardAreaOf(place.Key)!.Surface.Background)));
        }

        [AvaloniaFact]
        public void Hover_ACard_LightsItsAreaWithAHoverBorderTheFullHeightOfTheCard()
        {
            var area = AreaOf(Layer);

            Hover(BlankOf(RowOf(Layer)));

            Assert.Equal(Token("ListTreeCardHoverBrush"), FillOf(area.Surface.Background));
            Assert.Equal(0, LeftIn(area.Surface, area));
            AssertMark(area.Mark, KindOf(Layer).HoverBorder, 0, area);
            Assert.Equal(area.Place!.AreaHeight, area.Mark.Bounds.Height);
            Assert.True(area.Place.AreaHeight > 3 * 35);
            Assert.Equal(Colors.Transparent, FillOf(Named(RowOf(Layer), "band").Background));
            Assert.Equal(Colors.Transparent, FillOf(RowOf(Layer).Background));
            Assert.Equal(Token("ListTreeTextActiveBrush"), TextOf(RowOf(Layer)));
            Assert.Equal(Token("ListTreeTextActiveBrush"), TextOf(RowOf(Entity)));
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
        public void Select_ACard_LightsItsAreaAndMakesTheTextOfAllItsRowsWhite()
        {
            Select(Layer);
            var area = AreaOf(Layer);

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(area.Surface.Background));
            AssertMark(area.Mark, KindOf(Layer).SelectionBorder, 0, area);
            Assert.Equal(area.Place!.AreaHeight, area.Mark.Bounds.Height);
            Assert.Equal(Colors.Transparent, FillOf(RowOf(Layer).Background));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(RowOf(Layer)));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(RowOf(Entities)));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(RowOf(Entity)));
        }

        [AvaloniaFact]
        public void Hover_ASelectedItem_IsTwentyPercentAndKeepsTheSelectionBorder()
        {
            Select(Entity);
            var row = RowOf(Entity);

            Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowSelectedHoverBrush"), FillOf(row.Background));
            AssertMark(Named(row, "mark"), KindOf(Entity).SelectionBorder, 0, row);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
        }

        [AvaloniaFact]
        public void Hover_ASelectedCard_IsTwentyPercentAndKeepsTheSelectionBorder()
        {
            Select(Layer);
            var area = AreaOf(Layer);

            Hover(BlankOf(RowOf(Layer)));

            Assert.Equal(Token("ListTreeRowSelectedHoverBrush"), FillOf(area.Surface.Background));
            AssertMark(area.Mark, KindOf(Layer).SelectionBorder, 0, area);
        }

        [AvaloniaFact]
        public void Hover_AnItemInASelectedCard_AddsTheBandFromItsIndentAndKeepsTheAreaOfTheCard()
        {
            Select(Layer);
            var row = RowOf(Entity);
            var indent = view.Rows.Layout.PlaceOf(ListTreeKeys.Of(Entity))!.Indent;

            Hover(BlankOf(row));

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(AreaOf(Layer).Surface.Background));
            AssertMark(AreaOf(Layer).Mark, KindOf(Layer).SelectionBorder, 0, AreaOf(Layer));
            Assert.Equal(Colors.Transparent, FillOf(row.Background));
            Assert.Equal(Token("ListTreeRowHoverInSelectionBandBrush"), FillOf(Named(row, "band").Background));
            Assert.Equal(indent, LeftIn(Named(row, "band"), row));
            AssertMark(Named(row, "mark"), KindOf(Entity).HoverBorder, indent, row);
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
        }

        [AvaloniaFact]
        public void Hover_ACardInASelectedCard_AddsTheAreaBandFromItsIndentAndKeepsTheOuterArea()
        {
            Select(Layer);
            var area = AreaOf(Entities);
            var indent = area.Place!.Indent;

            Hover(BlankOf(RowOf(Entities)));

            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(AreaOf(Layer).Surface.Background));
            Assert.Equal(Token("ListTreeCardHoverInSelectionBandBrush"), FillOf(area.Surface.Background));
            Assert.Equal(indent, LeftIn(area.Surface, area));
            AssertMark(area.Mark, KindOf(Entities).HoverBorder, indent, area);
            Assert.Equal(area.Place.AreaHeight, area.Mark.Bounds.Height);
        }

        [AvaloniaFact]
        public void Place_AReusedCardAreaGetsANewPlace_MovesItsSurfaceToTheNewIndent()
        {
            var place = AreaOf(Entities).Place!;
            var area = new FrameListTreeCard { DataContext = Entities, Place = place };
            area.ShowMarks(new ListTreeMarks(Layer, null, null));
            window.Content = area;
            Settle();
            var indent = place.Indent + 16;

            area.Place = place with { Indent = indent };
            Settle();

            Assert.Equal(indent, area.IndentMargin.Left);
            Assert.Equal(indent, LeftIn(area.Surface, area));
        }

        [AvaloniaFact]
        public void Show_NestedCardAreasAfterARebuild_DrawTheInnerAreaOverItsOuterArea()
        {
            Entities.IsExpanded = false;
            Settle();
            Entities.IsExpanded = true;
            Settle();

            var outer = AreaOf(Layer);
            var inner = AreaOf(Entities);
            Assert.Equal(outer.ZIndex, inner.ZIndex);
            Assert.True(view.Rows.Children.IndexOf(inner) > view.Rows.Children.IndexOf(outer));
        }

        [AvaloniaFact]
        public void Arrange_TheCardAreas_AreUnderTheRowsAndSpanTheirCards()
        {
            var area = AreaOf(Layer);
            var place = area.Place!;

            Assert.Equal(ListTreeLayers.CardArea, area.ZIndex);
            Assert.True(area.ZIndex < RowOf(Layer).ZIndex);
            Assert.Equal(place.Y, area.Bounds.Y);
            Assert.Equal(place.AreaHeight, area.Bounds.Height);
            Assert.Equal(view.Rows.Bounds.Width, area.Bounds.Width);
            Assert.False(area.IsHitTestVisible);
        }

        [AvaloniaFact]
        public void Click_ARow_SelectsItsNodeAndMovesTheCursor()
        {
            Click(BlankOf(RowOf(Entity)));

            Assert.Same(Entity, scene.Tree.SelectedNode);
            Assert.Same(Entity, scene.Tree.CursorNode);
            Assert.True(RowOf(Entity).IsSelected);
        }

        [AvaloniaFact]
        public void Click_TheIconOfAnItem_SelectsIt()
        {
            Click(CenterOf(RowOf(Entity).IconHit));

            Assert.Same(Entity, scene.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_TheIconOfACard_OnlyOpensOrClosesIt()
        {
            Click(CenterOf(RowOf(Layer).IconHit));

            Assert.False(Layer.IsExpanded);
            Assert.Null(scene.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_AToggle_OnlyDoesTheToggle()
        {
            var visibility = RowOf(Entity).Toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "visibilityToggle");
            var before = visibility.IsChecked;

            Click(CenterOf(visibility));

            Assert.NotEqual(before, visibility.IsChecked);
            Assert.Null(scene.Tree.SelectedNode);
        }
    }
}
