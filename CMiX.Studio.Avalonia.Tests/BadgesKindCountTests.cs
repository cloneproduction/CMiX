// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A collapsed row of the list tree shows what it holds: the icon of each kind, in the color of the kind, with how many.
    public class BadgesKindCountTests : IDisposable
    {
        private readonly CategoryPalette palette = CategoryPalette.FromApplication();
        private Window? window;
        private ListTreeProject? view;
        private Scene? scene;

        public void Dispose() => window?.Close();

        // A composition with two layers. The first layer has two entities, one filter and three modifiers. Nothing is selected.
        private void Show(Action<Scene>? more = null)
        {
            scene = CreateScene([2], s =>
            {
                AddKinds(s.Layer);
                more?.Invoke(s);
            });
            scene.Tree.SelectedNode = null;
            (window, view) = ListTreeTestScreen.Show(scene.Tree, 500, 800);
        }

        private TreeNode Composition => scene!.NodeA;

        private TreeNode Layer => scene!.LayerNodeOfA(0);

        private TreeNode Empty => scene!.LayerNodeOfA(1);

        private void Close(TreeNode node)
        {
            node.IsExpanded = false;
            window!.Settle();
        }

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view!.Rows.RowOf(ListTreeKeys.Of(node)));

        // A row has two badges: one after the name box of an item, one after the name of a group. This gets the one for the node.
        private BadgesKindCount BadgesOf(TreeNode node) =>
            RowOf(node).GetVisualDescendants().OfType<BadgesKindCount>()
                .First(badges => (badges.FindAncestorOfType<TextBoxEditable>() != null) == (node.Item != null));

        private static Color FillOf(Panel badge) => ColorOf(IconOf(badge).Foreground);

        private static AppIcon IconOf(Panel badge) => badge.GetVisualDescendants().OfType<AppIcon>().First();

        private static TextOutlined CountOf(Panel badge) => badge.GetVisualDescendants().OfType<TextOutlined>().Single();

        private static List<Panel> KindBadgesOf(BadgesKindCount badges) =>
            badges.GetVisualDescendants().OfType<Panel>().Where(panel => panel.Classes.Contains("kindcount")).ToList();

        private static string TextIn(Panel badge) => CountOf(badge).Text!;

        private static string TextIn(Border square) => square.GetVisualDescendants().OfType<TextBlock>().Single().Text!;

        private static Color CountColorOf(Panel badge) => ColorOf(CountOf(badge).Foreground);

        private static Border NamedIn(BadgesKindCount badges, string name) =>
            badges.GetVisualDescendants().OfType<Border>().Single(border => border.Name == name);

        [AvaloniaFact]
        public void KindCount_ACategory_GivesItsIcon()
        {
            Assert.Equal(TreeIcons.CompositionKey, new KindCount(TreeCategory.Composable, 1).IconKey);
            Assert.Equal(TreeIcons.LightKey, new KindCount(TreeCategory.Light, 1).IconKey);
            Assert.Equal("Entity", new KindCount(TreeCategory.Entity, 2).IconKey);
            Assert.Equal("Filter", new KindCount(TreeCategory.Filter, 1).IconKey);
        }

        [AvaloniaFact]
        public void Show_ACollapsedRow_ShowsABadgeWithTheCountForEachKind()
        {
            Show();
            Close(Layer);

            var badges = BadgesOf(Layer);
            var kinds = KindBadgesOf(badges);

            Assert.True(badges.IsEffectivelyVisible);
            Assert.Equal(new[] { "2", "1", "3" }, kinds.Select(TextIn));
            Assert.Equal(new[] { "2 Entities", "1 Filter", "3 Modifiers" }, kinds.Select(badge => (string)ToolTip.GetTip(badge)!));
            Assert.Equal(
                new[] { TreeCategory.Entity, TreeCategory.Filter, TreeCategory.Modifier }.Select(category => palette.For(category).KindSelected),
                kinds.Select(FillOf));
        }

        [AvaloniaFact]
        public void Show_AnOpenOrEmptyRow_ShowsNoBadges()
        {
            Show();

            Assert.False(BadgesOf(Layer).IsEffectivelyVisible);

            Close(Empty);
            Assert.False(BadgesOf(Empty).IsEffectivelyVisible);
            Assert.Empty(KindBadgesOf(BadgesOf(Empty)));
        }

        [AvaloniaFact]
        public void Show_AClosedCompositionCard_ShowsItsKindBadges()
        {
            Show();
            Close(Composition);

            var badges = BadgesOf(Composition);

            Assert.True(badges.IsEffectivelyVisible);
            Assert.NotEmpty(KindBadgesOf(badges));
        }

        [AvaloniaFact]
        public void Show_AnOpenCompositionCard_ShowsNoKindBadges()
        {
            Show();

            Assert.False(BadgesOf(Composition).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Expand_ARow_HidesOrShowsTheBadges()
        {
            Show();

            Close(Layer);
            Assert.True(BadgesOf(Layer).IsEffectivelyVisible);

            Layer.IsExpanded = true;
            window!.Settle();
            Assert.False(BadgesOf(Layer).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_ACollapsedGroup_ShowsItsCountInTheColorOfItsKind()
        {
            Show();
            var entities = Layer.Children.Single(child => child.Group is EntitiesGroup);
            Close(entities);

            var badge = Assert.Single(KindBadgesOf(BadgesOf(entities)));

            Assert.Equal("2", TextIn(badge));
            Assert.Equal("2 Entities", ToolTip.GetTip(badge));
            Assert.Equal(palette.For(TreeCategory.Entity).KindSelected, FillOf(badge));
        }

        [AvaloniaFact]
        public void Hover_TheToggleOfARow_HoversItsBadges()
        {
            Show();
            Close(Layer);

            window!.Hover(RowOf(Layer).Toggles.GetVisualDescendants().OfType<ToggleButton>()
                .First(toggle => toggle.Name == "visibilityToggle" && toggle.IsEffectivelyVisible));

            Assert.True(BadgesOf(Layer).IsHovered);
        }

        [AvaloniaFact]
        public void Select_ARow_KeepsTheBadgesTheSame()
        {
            Show();
            Close(Layer);
            var badges = BadgesOf(Layer);
            var tone = palette.For(TreeCategory.Entity);

            Assert.Equal(tone.KindSelected, FillOf(KindBadgesOf(badges).First()));

            // The pointer over the row and the selection do not change the badges.
            badges.IsHovered = true;
            Assert.Equal(tone.KindSelected, FillOf(KindBadgesOf(badges).First()));

            scene!.Tree.SelectedNode = Layer;
            window!.Settle();
            Assert.Equal(tone.KindSelected, FillOf(KindBadgesOf(badges).First()));
        }

        // The count is always white: at rest, under the pointer and on a selected row.
        [AvaloniaFact]
        public void Select_ARow_KeepsTheCountsWhite()
        {
            Show();
            Close(Layer);
            var badges = BadgesOf(Layer);

            Assert.All(KindBadgesOf(badges), badge => Assert.Equal(Token("ActiveForegroundBrush"), CountColorOf(badge)));

            badges.IsHovered = true;
            window!.Settle();
            Assert.All(KindBadgesOf(badges), badge => Assert.Equal(Token("ActiveForegroundBrush"), CountColorOf(badge)));

            badges.IsHovered = false;
            scene!.Tree.SelectedNode = Layer;
            window!.Settle();
            Assert.All(KindBadgesOf(badges), badge => Assert.Equal(Token("ActiveForegroundBrush"), CountColorOf(badge)));
        }

        [AvaloniaFact]
        public void Select_ARow_SelectsItsBadgesAndMakesItsTogglesWhite()
        {
            Show();
            Close(Layer);

            scene!.Tree.SelectedNode = Layer;
            window!.Settle();

            Assert.True(BadgesOf(Layer).IsSelected);
            var icons = RowOf(Layer).Toggles.GetVisualDescendants().OfType<ToggleButton>().Where(toggle => toggle.IsEffectivelyVisible)
                .Select(toggle => toggle.GetVisualDescendants().OfType<AppIcon>().First()).ToList();
            Assert.NotEmpty(icons);
            Assert.All(icons, icon => Assert.Equal(Token("ActiveForegroundBrush"), ColorOf(icon.Foreground)));
        }

        [AvaloniaFact]
        public void Show_ABadge_IsSixteenHighInTheMiddleOfTheRow()
        {
            Show();
            Close(Layer);
            var row = RowOf(Layer).Band;
            var badge = KindBadgesOf(BadgesOf(Layer)).First();

            var top = badge.TranslatePoint(new Point(0, 0), row)!.Value.Y;
            var bottom = row.Bounds.Height - (top + badge.Bounds.Height);

            Assert.Equal(16, badge.Bounds.Height);
            Assert.Equal(top, bottom, 1);
            Assert.True(top > 0);
        }

        [AvaloniaFact]
        public void Badge_AKindCount_ShowsTheIconAndTheCountAtItsBottomRight()
        {
            Show();
            Close(Layer);
            var badge = KindBadgesOf(BadgesOf(Layer)).First();
            var icon = IconOf(badge);
            var count = CountOf(badge);
            var offset = (Thickness)Application.Current!.FindResource("TreeKindCountOffset")!;

            var iconCorner = icon.TranslatePoint(new Point(icon.Bounds.Width, icon.Bounds.Height), badge)!.Value;
            var countCorner = count.TranslatePoint(new Point(count.Bounds.Width, count.Bounds.Height), badge)!.Value;

            Assert.Equal("Entity", icon.IconKey);
            Assert.Equal(new Point(16, 16), iconCorner);
            Assert.Equal(16 - offset.Right, countCorner.X, 1);
            Assert.Equal(16 - offset.Bottom, countCorner.Y, 1);
            Assert.Equal(9, count.FontSize);
            Assert.Equal(FontWeight.Bold, count.FontWeight);
            Assert.Equal(2.5, count.OutlineThickness);
            Assert.Equal(Token("TreeBackgroundBrush"), ColorOf(count.OutlineBrush));
        }

        [AvaloniaFact]
        public void Badge_AKindCount_HasNoSquareBehindIt()
        {
            Show();
            Close(Layer);

            Assert.All(KindBadgesOf(BadgesOf(Layer)), badge =>
            {
                Assert.Equal(Colors.Transparent, ColorOf(badge.Background));
                Assert.Empty(badge.GetVisualDescendants().OfType<Border>().Where(border => border.Background is ISolidColorBrush { Color.A: > 0 }));
            });
        }

        [AvaloniaFact]
        public void Show_ABadgeCount_IsNotClippedByAnyAncestor()
        {
            Show();
            Close(Layer);
            var row = RowOf(Layer);

            var counts = KindBadgesOf(BadgesOf(Layer)).Select(CountOf).ToList();

            Assert.NotEmpty(counts);
            foreach (var count in counts)
            {
                foreach (var ancestor in count.GetVisualAncestors().TakeWhile(visual => visual != row).OfType<Visual>())
                {
                    if (!ancestor.ClipToBounds)
                        continue;

                    var corner = count.TranslatePoint(new Point(0, 0), ancestor)!.Value;
                    var rect = new Rect(corner, count.Bounds.Size);
                    Assert.True(new Rect(ancestor.Bounds.Size).Contains(rect), $"{ancestor.GetType().Name} clips the count");
                }
            }
        }

        [AvaloniaFact]
        public void Show_TwoKindBadges_AreEightApart()
        {
            Show();
            Close(Layer);
            var badges = KindBadgesOf(BadgesOf(Layer));

            var gap = badges[1].TranslatePoint(new Point(0, 0), badges[0])!.Value.X - badges[0].Bounds.Width;

            Assert.Equal(8, gap, 1);
        }

        [AvaloniaFact]
        public void Show_ARowWithFiveKinds_ShowsThreeBadgesAndPlusTwo()
        {
            Show(s =>
            {
                s.Layer.CameraManager.AddItem(typeof(CMiX.Core.Rendering.Cameras.Camera));
                s.Layer.LightManager.AddItem(typeof(CMiX.Core.Rendering.Lights.LightEntity));
            });
            Close(Layer);

            var badges = BadgesOf(Layer);
            var more = NamedIn(badges, "MoreKinds");

            Assert.Equal(new[] { "2", "1", "3" }, KindBadgesOf(badges).Select(TextIn));
            Assert.True(more.IsEffectivelyVisible);
            Assert.Equal("+2", TextIn(more));
            Assert.Equal("1 Camera" + Environment.NewLine + "1 Light", ToolTip.GetTip(more));
        }

        [AvaloniaFact]
        public void Show_ARowWithThreeKinds_ShowsNoPlusSquare()
        {
            Show();
            Close(Layer);

            Assert.False(NamedIn(BadgesOf(Layer), "MoreKinds").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_ARowWithoutMissingAssets_ShowsNoMissingSign()
        {
            Show();
            Close(Layer);

            var sign = NamedIn(BadgesOf(Layer), "MissingSign");

            Assert.False(Layer.HasMissingAssets);
            Assert.False(sign.IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_TheMissingSign_IsTheOrangeWarningIcon()
        {
            Show();
            Close(Layer);

            var sign = NamedIn(BadgesOf(Layer), "MissingSign");
            var icon = sign.GetVisualDescendants().OfType<AppIcon>().Single();

            Assert.Equal("Warning", icon.IconKey);
            Assert.Equal(Token("PopupCardWarningBrush"), ((ISolidColorBrush)icon.Foreground!).Color);
            Assert.Empty(sign.GetVisualDescendants().OfType<global::Avalonia.Controls.Shapes.Path>());
        }

        [AvaloniaFact]
        public void Show_ACollapsedRow_PutsTheBadgesRightAfterTheName()
        {
            Show();
            Close(Layer);
            var name = RowOf(Layer).GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "textDisplay");
            var badges = BadgesOf(Layer);

            var nameRight = name.TranslatePoint(new Point(name.Bounds.Width, 0), window!)!.Value.X;
            var badgesLeft = badges.TranslatePoint(new Point(0, 0), window!)!.Value.X;

            Assert.InRange(badgesLeft - nameRight, 0, 12);
        }
    }
}
