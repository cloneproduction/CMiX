// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
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

        private void Settle() => TreeTestScreen.Settle(window);

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(Panel.RowOf(ListTreeKeys.Of(node)));

        private RowListTree RowOf(ListTreePlace place) => Assert.IsType<RowListTree>(Panel.RowOf(place.Key));

        private static T Named<T>(Control root, string name) where T : Control =>
            root.GetVisualDescendants().OfType<T>().First(control => control.Name == name);

        private BadgesKindCount BadgesOf(TreeNode node) =>
            Named<TextBoxEditable>(RowOf(node), "nameBox").GetVisualDescendants().OfType<BadgesKindCount>().Single();

        private static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

        private static Color Token(string key) => ColorOf((IBrush)Application.Current!.FindResource(key)!);

        private void Click(Control control)
        {
            var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
            TreeDrag.Render(window);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Settle();
        }

        [AvaloniaFact]
        public void Arrange_VisibleRows_PutsEachRowAtItsIndexTimesThirtyFive()
        {
            var places = Panel.Layout.Places;

            Assert.True(places.Count > 3);
            for (var index = 0; index < places.Count; index++)
                Assert.Equal(index * 35, RowOf(places[index]).Bounds.Y);
        }

        [AvaloniaFact]
        public void Arrange_EveryRow_IsThirtyTwoHighAndAsWideAsThePanel()
        {
            Assert.All(Panel.Layout.Places, place =>
            {
                var bounds = RowOf(place).Bounds;
                Assert.Equal(32, bounds.Height);
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
        public void IsExpanded_AClosedCard_HidesItsRowsAndShowsItsKindBadges()
        {
            var layer = scene.LayerNodeOfA(0);
            var topOfB = RowOf(scene.NodeB).Bounds.Y;

            scene.NodeA.IsExpanded = false;
            Settle();

            Assert.Null(Panel.RowOf(ListTreeKeys.Of(layer)));
            var badges = BadgesOf(scene.NodeA);
            Assert.True(badges.IsEffectivelyVisible);
            Assert.NotEmpty(badges.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("kindcount")));
            var indexOfB = Panel.Layout.Places.ToList().FindIndex(place => place.Key == ListTreeKeys.Of(scene.NodeB));
            Assert.True(RowOf(scene.NodeB).Bounds.Y < topOfB);
            Assert.Equal(indexOfB * 35, RowOf(scene.NodeB).Bounds.Y);
        }

        [AvaloniaFact]
        public void IsExpanded_AnOpenCard_ShowsNoKindBadges()
        {
            Assert.False(BadgesOf(scene.NodeA).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void IsExpanded_ACardClosesAndOpens_KeepsTheRowControlOfEachShownRow()
        {
            var rowB = RowOf(scene.NodeB);

            scene.NodeA.IsExpanded = false;
            Settle();
            scene.NodeA.IsExpanded = true;
            Settle();

            Assert.Same(rowB, RowOf(scene.NodeB));
        }

        [AvaloniaFact]
        public void OnIconTapped_TheIconOfACard_ClosesAndOpensTheCard()
        {
            var iconHit = RowOf(scene.NodeA).IconHit;

            Click(iconHit);
            Assert.False(scene.NodeA.IsExpanded);

            Click(RowOf(scene.NodeA).IconHit);
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

            Click(row.IconHit);

            Assert.Equal(wasExpanded, node.IsExpanded);
            Assert.Equal(rowCount, Panel.Layout.Places.Count);
        }

        [AvaloniaFact]
        public void Arrange_TheTogglesOfALayer_SitAtTheRightEdgeWithAGapOfEight()
        {
            var row = RowOf(scene.LayerNodeOfA(0));
            var slots = row.Toggles.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot")).ToList();
            var visibility = slots.Single(slot => slot.Classes.Contains("visibility"));
            var mask = slots.Single(slot => slot.Classes.Contains("mask"));

            var visibilityLeft = visibility.TranslatePoint(new Point(0, 0), row)!.Value.X;
            var maskLeft = mask.TranslatePoint(new Point(0, 0), row)!.Value.X;

            Assert.Equal(32, visibility.Bounds.Width);
            Assert.Equal(32, mask.Bounds.Width);
            Assert.Equal(row.Bounds.Width, visibilityLeft + visibility.Bounds.Width, 0.5);
            Assert.Equal(8, visibilityLeft - (maskLeft + mask.Bounds.Width), 0.5);
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
            TreeTestScreen.Settle(screen);
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
            Assert.Equal(8, icon.TranslatePoint(new Point(0, 0), Named<Border>(RowOf(node), "iconHit"))!.Value.X);
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
