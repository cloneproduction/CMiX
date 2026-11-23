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
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The toggles and the badges of the new list tree look and click as the ones of the old tree view, for the same node of one tree.
    public class ListTreeTogglesAndBadgesTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly TreeTestScreen oldScreen;
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeTogglesAndBadgesTests()
        {
            scene.Tree.SelectedNode = null;
            scene.LayerNodeOfA(0).IsExpanded = false;
            oldScreen = TreeTestScreen.Show(scene.Tree, 500, 800);
            view = new ListTreeProject { DataContext = scene.Tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            Settle();
        }

        public void Dispose()
        {
            window.Close();
            oldScreen.Dispose();
        }

        private sealed record ToggleLook(string? Name, bool? IsChecked, string? IconKey, Color Icon, double Opacity);

        private sealed record BadgesLook(bool IsVisible, bool IsHovered, bool IsSelected, string Squares, bool HasMoreKinds, bool HasMissingSign);

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private IEnumerable<TreeNode> Measured => [scene.NodeA, Scene.LayersOf(scene.NodeA), scene.LayerNodeOfA(0), scene.LayerNodeOfA(1)];

        private void Settle()
        {
            oldScreen.Settle();
            TreeTestScreen.Settle(window);
        }

        private RowListTree NewRowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private TogglesRow OldTogglesOf(TreeNode node) =>
            TreeTestScreen.ItemOf(oldScreen.View, node).GetVisualDescendants().OfType<TogglesRow>().First();

        private BadgesKindCount OldBadgesOf(TreeNode node) =>
            TreeTestScreen.ItemOf(oldScreen.View, node).GetVisualDescendants().OfType<BadgesKindCount>().First(badges => badges.IsVisible);

        private BadgesKindCount NewBadgesOf(TreeNode node) =>
            NewRowOf(node).GetVisualDescendants().OfType<BadgesKindCount>().First(badges => badges.IsVisible);

        private static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

        private static Color Token(string key) => ColorOf((IBrush)Application.Current!.FindResource(key)!);

        private static IReadOnlyList<ToggleLook> LookOf(TogglesRow toggles) =>
            toggles.GetVisualDescendants().OfType<ToggleButton>().Where(toggle => toggle.IsEffectivelyVisible)
                .OrderBy(toggle => toggle.Name).Select(toggle => LookOf(toggle, toggles)).ToList();

        private static ToggleLook LookOf(ToggleButton toggle, Visual top)
        {
            var icon = toggle.GetVisualDescendants().OfType<AppIcon>().First();
            return new ToggleLook(toggle.Name, toggle.IsChecked, icon.IconKey, ColorOf(icon.Foreground), OpacityOf(icon, top));
        }

        private static double OpacityOf(Visual visual, Visual top)
        {
            var opacity = 1.0;
            for (var current = visual; current != null && !ReferenceEquals(current, top.GetVisualParent()); current = current.GetVisualParent())
                opacity *= current.Opacity;
            return opacity;
        }

        private static BadgesLook LookOf(BadgesKindCount badges)
        {
            var squares = badges.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("kindcount"))
                .Select(square => $"{ColorOf(square.Background)} {CountOf(square).Text} {ColorOf(CountOf(square).Foreground)}");
            return new BadgesLook(badges.IsEffectivelyVisible, badges.IsHovered, badges.IsSelected, string.Join(";", squares),
                Named(badges, "MoreKinds").IsEffectivelyVisible, Named(badges, "MissingSign").IsEffectivelyVisible);
        }

        private static TextBlock CountOf(Border square) => square.GetVisualDescendants().OfType<TextBlock>().Single();

        private static Border Named(Control root, string name) => root.GetVisualDescendants().OfType<Border>().First(border => border.Name == name);

        private static ToggleButton ToggleNamed(TogglesRow toggles, string name) =>
            toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == name && toggle.IsEffectivelyVisible);

        private void AssertTogglesAndBadgesMatch()
        {
            Assert.All(Measured, node => Assert.Equal(LookOf(OldTogglesOf(node)), LookOf(NewRowOf(node).Toggles)));
            Assert.Equal(LookOf(OldBadgesOf(Layer)), LookOf(NewBadgesOf(Layer)));
        }

        private static void Hover(Window screen, Control control)
        {
            TreeDrag.Render(screen);
            screen.MouseMove(CenterOf(control, screen));
            TreeTestScreen.Settle(screen);
        }

        private static void Click(Window screen, Control control)
        {
            var point = CenterOf(control, screen);
            TreeDrag.Render(screen);
            screen.MouseDown(point, MouseButton.Left);
            screen.MouseUp(point, MouseButton.Left);
            TreeTestScreen.Settle(screen);
        }

        private static Point CenterOf(Control control, Window screen) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), screen)!.Value;

        [AvaloniaFact]
        public void TogglesAndBadges_AtRest_MatchTheOldTree()
        {
            Assert.True(NewBadgesOf(Layer).IsEffectivelyVisible);

            AssertTogglesAndBadgesMatch();
        }

        [AvaloniaFact]
        public void TogglesAndBadges_UnderThePointer_MatchTheOldTree()
        {
            Hover(oldScreen.Window, ToggleNamed(OldTogglesOf(Layer), "visibilityToggle"));
            Hover(window, ToggleNamed(NewRowOf(Layer).Toggles, "visibilityToggle"));

            Assert.True(NewBadgesOf(Layer).IsHovered);
            AssertTogglesAndBadgesMatch();
        }

        [AvaloniaFact]
        public void TogglesAndBadges_OfASelectedRow_MatchTheOldTreeAndAreWhite()
        {
            scene.Tree.SelectedNode = Layer;
            Settle();

            Assert.True(NewBadgesOf(Layer).IsSelected);
            Assert.All(LookOf(NewRowOf(Layer).Toggles), look => Assert.Equal(Token("ActiveForegroundBrush"), look.Icon));
            AssertTogglesAndBadgesMatch();
        }

        [AvaloniaFact]
        public void TogglesAndBadges_OfAHiddenRow_MatchTheOldTree()
        {
            scene.Layer.PrefabService.Visibility.Value = true;
            Settle();

            AssertTogglesAndBadgesMatch();
        }

        [AvaloniaFact]
        public void TogglesAndBadges_UnderAHiddenParent_MatchTheOldTree()
        {
            scene.A.PrefabService.Visibility.Value = true;
            Settle();

            Assert.True(Layer.IsUnderHiddenParent);
            AssertTogglesAndBadgesMatch();
        }

        [AvaloniaFact]
        public void TogglesAndBadges_OfAMaskedRow_MatchTheOldTree()
        {
            scene.Layer.Mask.IsMask.Value = true;
            Settle();

            AssertTogglesAndBadgesMatch();
        }

        [AvaloniaTheory]
        [InlineData("visibilityToggle")]
        [InlineData("maskToggle")]
        public void Click_AToggleOfALayer_DoesWhatTheOldTreeDoes(string name)
        {
            var before = Outcome();
            Click(oldScreen.Window, ToggleNamed(OldTogglesOf(Layer), name));
            var oldOutcome = Outcome();
            Click(oldScreen.Window, ToggleNamed(OldTogglesOf(Layer), name));
            Assert.Equal(before, Outcome());

            Click(window, ToggleNamed(NewRowOf(Layer).Toggles, name));

            Assert.NotEqual(before, oldOutcome);
            Assert.Equal(oldOutcome, Outcome());
        }

        [AvaloniaFact]
        public void Click_TheHideButtonOfAGroup_DoesWhatTheOldTreeDoes()
        {
            var group = Scene.LayersOf(scene.NodeA);
            var before = group.Group!.Manager.Hidden.Value;
            Click(oldScreen.Window, ToggleNamed(OldTogglesOf(group), "groupHideToggle"));
            var oldOutcome = (group.Group.Manager.Hidden.Value, scene.Tree.SelectedNode);
            Click(oldScreen.Window, ToggleNamed(OldTogglesOf(group), "groupHideToggle"));
            Assert.Equal(before, group.Group.Manager.Hidden.Value);

            Click(window, ToggleNamed(NewRowOf(group).Toggles, "groupHideToggle"));

            Assert.NotEqual(before, oldOutcome.Value);
            Assert.Equal(oldOutcome, (group.Group.Manager.Hidden.Value, scene.Tree.SelectedNode));
        }

        private string Outcome() =>
            $"{scene.Layer.PrefabService.Visibility.Value} {scene.Layer.Mask.IsMask.Value} {scene.Tree.SelectedNode?.Header} {Layer.IsExpanded}";
    }
}
