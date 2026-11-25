// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Animation;
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
    public class PanelListTreeRowsTests : IDisposable
    {
        private readonly Scene scene = CreateScene();
        private readonly Window window;
        private readonly ListTreeProject view;

        public PanelListTreeRowsTests()
        {
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private PanelListTreeRows Panel => view.Rows;

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(Panel.RowOf(ListTreeKeys.Of(node)));

        private RowListTree RowOf(ListTreePlace place) => Assert.IsType<RowListTree>(Panel.RowOf(place.Key));

        private static T Named<T>(Control root, string name) where T : Control =>
            root.GetVisualDescendants().OfType<T>().First(control => control.Name == name);

        private static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

        private static Color Token(string key) => ColorOf((IBrush)Application.Current!.FindResource(key)!);

        private static double TokenSize(string key) => (double)Application.Current!.FindResource(key)!;

        private static Thickness TokenThickness(string key) => (Thickness)Application.Current!.FindResource(key)!;

        [AvaloniaFact]
        public void Arrange_VisibleRows_PutsEachRowAtItsIndexTimesThePitch()
        {
            var places = Panel.Layout.Places;

            Assert.True(places.Count > 3);
            for (var index = 0; index < places.Count; index++)
                Assert.Equal(index * ListTreeSizes.Default.Pitch, RowOf(places[index]).Bounds.Y);
        }

        [AvaloniaFact]
        public void Arrange_EveryRow_IsOneRowHighAndAsWideAsThePanel()
        {
            Assert.All(Panel.Layout.Places, place =>
            {
                var bounds = RowOf(place).Bounds;
                Assert.Equal(ListTreeSizes.Default.RowHeight, bounds.Height);
                Assert.Equal(0, bounds.X);
                Assert.Equal(Panel.Bounds.Width, bounds.Width);
            });
        }

        [AvaloniaFact]
        public void Arrange_RowsAtSeveralDepths_IndentTheBandSixteenPerDepth()
        {
            Assert.Contains(Panel.Layout.Places, place => place.Depth >= 2);
            Assert.All(Panel.Layout.Places, place =>
            {
                var row = RowOf(place);
                var band = Named<Border>(row, "band");
                Assert.Equal(place.Depth * 16, band.TranslatePoint(new Point(0, 0), row)!.Value.X);
            });
        }

        [AvaloniaFact]
        public void Arrange_EveryRow_HasTheRowLayer()
        {
            Assert.All(Panel.Layout.Places, place => Assert.Equal(ListTreeLayers.Row, RowOf(place).ZIndex));
        }

        [AvaloniaFact]
        public void IsExpanded_AClosedCard_HidesItsRowsAndMovesTheNextRowsUp()
        {
            var layer = scene.LayerNodeOfA(0);
            var topOfB = RowOf(scene.NodeB).Bounds.Y;

            scene.NodeA.IsExpanded = false;
            window.Settle();

            Assert.Null(Panel.RowOf(ListTreeKeys.Of(layer)));
            var indexOfB = Panel.Layout.Places.ToList().FindIndex(place => place.Key == ListTreeKeys.Of(scene.NodeB));
            Assert.True(RowOf(scene.NodeB).Bounds.Y < topOfB);
            Assert.Equal(indexOfB * ListTreeSizes.Default.Pitch, RowOf(scene.NodeB).Bounds.Y);
        }

        [AvaloniaFact]
        public void IsExpanded_ACardClosesAndOpens_KeepsTheRowControlOfEachShownRow()
        {
            var rowB = RowOf(scene.NodeB);

            scene.NodeA.IsExpanded = false;
            window.Settle();
            scene.NodeA.IsExpanded = true;
            window.Settle();

            Assert.Same(rowB, RowOf(scene.NodeB));
        }

        [AvaloniaFact]
        public void OnIconTapped_TheIconOfACard_ClosesAndOpensTheCard()
        {
            var iconHit = RowOf(scene.NodeA).IconHit;

            window.Click(iconHit);
            Assert.False(scene.NodeA.IsExpanded);

            window.Click(RowOf(scene.NodeA).IconHit);
            Assert.True(scene.NodeA.IsExpanded);
        }

        [AvaloniaFact]
        public void OnIconTapped_TheIconOfAnItem_ChangesNothing()
        {
            var place = Panel.Layout.Places.First(candidate => !candidate.CanToggle);
            var row = RowOf(place);
            var node = Assert.IsType<TreeNode>(row.DataContext);
            var wasExpanded = node.IsExpanded;
            var rowCount = Panel.Layout.Places.Count;

            window.Click(row.IconHit);

            Assert.Equal(wasExpanded, node.IsExpanded);
            Assert.Equal(rowCount, Panel.Layout.Places.Count);
        }

        [AvaloniaFact]
        public void Arrange_TheTogglesOfALayer_SitAtTheRightEdgeWithTheTokenGap()
        {
            var row = RowOf(scene.LayerNodeOfA(0));
            var slots = row.Toggles.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot")).ToList();
            var visibility = slots.Single(slot => slot.Classes.Contains("visibility"));
            var mask = slots.Single(slot => slot.Classes.Contains("mask"));

            var visibilityLeft = visibility.TranslatePoint(new Point(0, 0), row)!.Value.X;
            var maskLeft = mask.TranslatePoint(new Point(0, 0), row)!.Value.X;

            Assert.Equal(TokenSize("ListTreeToggleSize"), visibility.Bounds.Width);
            Assert.Equal(TokenSize("ListTreeToggleSize"), mask.Bounds.Width);
            Assert.Equal(row.Bounds.Width, visibilityLeft + visibility.Bounds.Width, 0.5);
            Assert.Equal(TokenSize("ListTreeToggleGap"), visibilityLeft - (maskLeft + mask.Bounds.Width), 0.5);
        }

        [AvaloniaFact]
        public void Arrange_TheTogglesAtEveryDepth_LineUp()
        {
            var rights = Panel.Layout.Places.Select(place => RowOf(place).Toggles)
                .Select(toggles => toggles.TranslatePoint(new Point(toggles.Bounds.Width, 0), Panel)!.Value.X).Distinct().ToList();

            Assert.Single(rights);
        }

        [AvaloniaFact]
        public void Show_ARowThatMovesWithTheDefaultTime_SlidesItsRenderIn160MillisecondsAndNotItsTop()
        {
            var moved = CreateScene([1, 1]);
            var other = new ListTreeProject { DataContext = moved.Tree };
            var screen = new Window { Content = other, Width = 400, Height = 600 };
            screen.Show();
            ListTreeTestScreen.Settle(screen);
            var row = Assert.IsType<RowListTree>(other.Rows.RowOf(moved.Tree.CompositionNodes()[1]));
            var before = PanelListTreeRows.GetTop(row);

            moved.Tree.CompositionNodes()[0].IsExpanded = false;
            other.Rows.ApplyTreeChanges();

            var transition = Assert.Single(row.Transitions!.OfType<TransformOperationsTransition>());
            var top = PanelListTreeRows.GetTop(row);
            var shown = ListTreeRowSlide.ShownTopOf(row);
            screen.Close();

            Assert.Same(Visual.RenderTransformProperty, transition.Property);
            Assert.Equal(TimeSpan.FromMilliseconds(160), transition.Duration);
            Assert.True(top < before);
            Assert.True(shown > top);
        }

        [AvaloniaFact]
        public void Show_ARow_HasTheRestBandAndTheRestText()
        {
            var row = RowOf(scene.LayerNodeOfA(0));

            Assert.Equal(Token("ListTreeRowRestBandBrush"), ColorOf(Named<Border>(row, "band").Background));
            Assert.Equal(Token("ListTreeTextRestBrush"), ColorOf(Named<TextBoxEditable>(row, "nameBox").Foreground));
        }

        [AvaloniaFact]
        public void Show_TheIconOfARow_HasTheKindColourAndSixteenPixels()
        {
            var node = scene.NodeA;
            var icon = Named<AppIcon>(RowOf(node), "kindIcon");
            var expected = CategoryPalette.FromApplication().For(node.Category!.Value, node.ComposableDepth, node.KindDepth).Of(ToneRole.KindSelected);

            Assert.Equal(expected, ColorOf(icon.Foreground));
            Assert.Equal(16, icon.Size);
            Assert.Equal(TokenThickness("ListTreeIconMargin").Left, icon.TranslatePoint(new Point(0, 0), Named<Border>(RowOf(node), "iconHit"))!.Value.X);
        }

        [AvaloniaFact]
        public void Row_TheKindIcon_HasTheSameSpaceOnEachSide()
        {
            var row = RowOf(scene.LayerNodeOfA(0));
            var icon = Named<AppIcon>(row, "kindIcon");
            var cell = Named<Border>(row, "iconHit");
            var band = Named<Border>(row, "band");
            var origin = icon.TranslatePoint(new Point(0, 0), row)!.Value;

            Assert.Equal(26, row.Bounds.Height);
            Assert.Equal(5, origin.X - band.TranslatePoint(new Point(0, 0), row)!.Value.X, 0.5);
            Assert.Equal(5, cell.TranslatePoint(new Point(cell.Bounds.Width, 0), row)!.Value.X - (origin.X + icon.Bounds.Width), 0.5);
            Assert.Equal(5, origin.Y, 0.5);
            Assert.Equal(5, row.Bounds.Height - (origin.Y + icon.Bounds.Height), 0.5);
        }

        [AvaloniaFact]
        public void Row_TheTwoToggles_Touch()
        {
            var row = RowOf(scene.LayerNodeOfA(0));
            var slots = row.Toggles.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot")).ToList();
            var mask = ToggleOf(slots.Single(slot => slot.Classes.Contains("mask")));
            var eye = ToggleOf(slots.Single(slot => slot.Classes.Contains("visibility")));

            var maskRight = mask.TranslatePoint(new Point(mask.Bounds.Width, 0), row)!.Value.X;
            var eyeLeft = eye.TranslatePoint(new Point(0, 0), row)!.Value.X;
            var eyeRight = eye.TranslatePoint(new Point(eye.Bounds.Width, 0), row)!.Value.X;

            Assert.Equal(16, mask.Bounds.Width, 0.5);
            Assert.Equal(16, eye.Bounds.Width, 0.5);
            Assert.Equal(10, eyeLeft - maskRight, 0.5);
            Assert.Equal(5, row.Bounds.Width - eyeRight, 0.5);
        }

        private static ToggleButton ToggleOf(Border slot) =>
            slot.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.IsVisible);

        [AvaloniaFact]
        public void Row_AReusedRow_TakesTheCursorClassOfItsNewNode()
        {
            scene.NodeB.IsExpanded = false;
            window.Settle();
            var layerOfA = scene.LayerNodeOfA(0);
            scene.Tree.CursorNode = layerOfA;
            window.Settle();
            var rowsOfA = Panel.Children.OfType<RowListTree>().Where(row => row.DataContext is TreeNode node && node.IsOrIsUnder(scene.NodeA)).ToList();
            Assert.Contains("cursor", RowOf(layerOfA).Classes);

            scene.NodeA.IsExpanded = false;
            window.Settle();
            scene.NodeB.IsExpanded = true;
            window.Settle();
            var layerOfB = scene.LayerNodeOfB(0);
            scene.Tree.CursorNode = layerOfB;
            window.Settle();

            var reused = RowOf(layerOfB);
            Assert.Contains(reused, rowsOfA);
            Assert.Contains("cursor", reused.Classes);
            Assert.All(Panel.Children.OfType<RowListTree>().Where(row => row.IsVisible),
                row => Assert.Equal(((TreeNode)row.DataContext!).IsCursor, row.Classes.Contains("cursor")));
        }

        [AvaloniaFact]
        public void Row_TheInspectedNode_HasTheInspectedClass()
        {
            var inspected = scene.LayerNodeOfA(0);

            scene.Tree.Inspect(inspected);
            window.Settle();

            Assert.Contains("inspected", RowOf(inspected).Classes);
            Assert.DoesNotContain("inspected", RowOf(scene.LayerNodeOfA(1)).Classes);
        }

        [AvaloniaFact]
        public void Show_AGroupRow_HasTheFolderIcon()
        {
            var group = Scene.LayersOf(scene.NodeA);

            Assert.Equal("Folder", RowOf(group).IconKey);
        }

        [AvaloniaFact]
        public void Show_TheNameOfACardAndOfAnItem_IsTwelvePixelsSemiBoldAndNormal()
        {
            var cardName = Named<TextBoxEditable>(RowOf(scene.NodeA), "nameBox");
            var itemPlace = Panel.Layout.Places.First(place => !place.CanToggle);
            var itemName = Named<TextBoxEditable>(RowOf(itemPlace), "nameBox");

            Assert.Equal(12, cardName.FontSize);
            Assert.Equal(FontWeight.SemiBold, cardName.FontWeight);
            Assert.Equal(FontWeight.Normal, itemName.FontWeight);
        }

        [AvaloniaFact]
        public void Roots_OfThePanel_AreTheRootsOfTheTree()
        {
            Assert.Same(scene.Tree.Roots, Panel.Roots);
        }

        [AvaloniaFact]
        public void Text_TheNameBoxOfALayer_ShowsAndWritesTheNameOfTheLayer()
        {
            var nameBox = Named<TextBoxEditable>(RowOf(scene.LayerNodeOfA(0)), "nameBox");

            Assert.Equal(scene.Layer.PrefabService.Name.Value, nameBox.Text);
            nameBox.Text = "Renamed";
            Assert.Equal("Renamed", scene.Layer.PrefabService.Name.Value);
        }
    }
}
