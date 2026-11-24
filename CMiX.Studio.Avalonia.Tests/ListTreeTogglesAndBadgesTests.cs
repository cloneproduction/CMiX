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
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;

namespace CMiX.Studio.Avalonia.Tests
{
    // The toggles and the badges of the list tree show the right buttons and squares for each kind of row, and the buttons change the model.
    public class ListTreeTogglesAndBadgesTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeTogglesAndBadgesTests()
        {
            scene.Tree.SelectedNode = null;
            scene.LayerNodeOfA(0).IsExpanded = false;
            (window, view) = ListTreeTestScreen.Show(scene.Tree, 500, 800);
        }

        public void Dispose()
        {
            window.Close();
        }

        private sealed record ToggleLook(string? Name, bool? IsChecked, string? IconKey, Color Icon, double Opacity);

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entity => scene.Group<EntitiesGroup>(0).Children[0];

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private BadgesKindCount BadgesOf(TreeNode node) =>
            RowOf(node).GetVisualDescendants().OfType<BadgesKindCount>().First(badges => badges.IsVisible);

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

        private static string[] NamesOf(TogglesRow toggles) =>
            LookOf(toggles).Select(look => look.Name!).ToArray();

        private static IEnumerable<string> SquareCountsOf(BadgesKindCount badges) =>
            badges.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("kindcount")).Select(square => CountOf(square).Text!);

        private static TextBlock CountOf(Border square) => square.GetVisualDescendants().OfType<TextBlock>().Single();

        private static ToggleButton ToggleNamed(TogglesRow toggles, string name) =>
            toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == name && toggle.IsEffectivelyVisible);

        [AvaloniaFact]
        public void Toggles_OfAComposableRow_AreTheMaskAndTheEye()
        {
            Assert.Equal(new[] { "maskToggle", "visibilityToggle" }, NamesOf(RowOf(scene.NodeA).Toggles));
            Assert.Equal(new[] { "maskToggle", "visibilityToggle" }, NamesOf(RowOf(Layer).Toggles));
        }

        [AvaloniaFact]
        public void Toggles_OfAnItemRow_AreTheEyeOnly()
        {
            Layer.IsExpanded = true;
            window.Settle();

            Assert.Equal(new[] { "visibilityToggle" }, NamesOf(RowOf(Entity).Toggles));
        }

        [AvaloniaFact]
        public void Toggles_OfAGroupRow_AreTheHideButtonOnly()
        {
            Assert.Equal(new[] { "groupHideToggle" }, NamesOf(RowOf(Scene.LayersOf(scene.NodeA)).Toggles));
        }

        [AvaloniaFact]
        public void Toggles_OfAHiddenRow_ShowTheEyeChecked()
        {
            scene.Layer.PrefabService.Visibility.Value = true;
            window.Settle();

            Assert.True(ToggleNamed(RowOf(Layer).Toggles, "visibilityToggle").IsChecked);
        }

        [AvaloniaFact]
        public void Toggles_UnderAHiddenParent_ShowTheEyeDimmed()
        {
            var before = LookOf(RowOf(Layer).Toggles).Single(look => look.Name == "visibilityToggle").Opacity;

            scene.A.PrefabService.Visibility.Value = true;
            window.Settle();

            var after = LookOf(RowOf(Layer).Toggles).Single(look => look.Name == "visibilityToggle").Opacity;
            Assert.True(Layer.IsUnderHiddenParent);
            Assert.True(after < before);
        }

        [AvaloniaFact]
        public void Toggles_OfAMaskedRow_ShowTheMaskChecked()
        {
            scene.Layer.Mask.IsMask.Value = true;
            window.Settle();

            Assert.True(ToggleNamed(RowOf(Layer).Toggles, "maskToggle").IsChecked);
        }

        [AvaloniaTheory]
        [InlineData("visibilityToggle")]
        [InlineData("maskToggle")]
        public void Click_AToggleOfALayer_ChangesTheModelAndBack(string name)
        {
            var before = ValueOf(name);

            window.Click(ToggleNamed(RowOf(Layer).Toggles, name));
            var after = ValueOf(name);
            window.Click(ToggleNamed(RowOf(Layer).Toggles, name));

            Assert.NotEqual(before, after);
            Assert.Equal(before, ValueOf(name));
        }

        [AvaloniaFact]
        public void Click_TheHideButtonOfAGroup_SetsHiddenOnTheListAndBack()
        {
            var group = Scene.LayersOf(scene.NodeA);
            var before = group.Group!.Manager.Hidden.Value;

            window.Click(ToggleNamed(RowOf(group).Toggles, "groupHideToggle"));
            var after = group.Group.Manager.Hidden.Value;
            window.Click(ToggleNamed(RowOf(group).Toggles, "groupHideToggle"));

            Assert.NotEqual(before, after);
            Assert.Equal(before, group.Group.Manager.Hidden.Value);
        }

        private bool ValueOf(string name) =>
            name == "maskToggle" ? scene.Layer.Mask.IsMask.Value : scene.Layer.PrefabService.Visibility.Value;
    }
}
