// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tooltips of the rows of the list tree: TreeTooltips opens and closes them with the show delay of the control,
    // a key closes them, and each tip shows its text with the hover fill of the kind of its row.
    public class TreeTooltipsTests : IDisposable
    {
        // A composition with a layer that has two entities, a filter and three modifiers.
        private readonly Scene scene = CreateScene([1], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public TreeTooltipsTests()
        {
            (window, view) = ListTreeTestScreen.Show(scene.Tree, 900, 700);
        }

        public void Dispose() => window.Close();

        private ProjectTree Tree => scene.Tree;

        private TreeNode Composition => Tree.CompositionNodes()[0];

        private TreeNode Layer => Composition.Children[0].Children[0];

        private RowListTree RowOf(TreeNode node) => view.Rows.RowOf(node)!;

        private static bool IsMask(ToggleButton toggle) => Equals(ToolTip.GetTip(toggle), "Mask") && toggle.IsEffectivelyVisible;

        private ToggleButton MaskOf(TreeNode node) => RowOf(node).GetVisualDescendants().OfType<ToggleButton>().First(IsMask);

        private bool HasMask(TreeNode node) =>
            view.Rows.RowOf(node) is { } row && row.GetVisualDescendants().OfType<ToggleButton>().Any(IsMask);

        // A row has two badges: one after the name box of an item, one after the name of a group. Only one shows.
        private BadgesKindCount BadgesOf(TreeNode node) =>
            RowOf(node).GetVisualDescendants().OfType<BadgesKindCount>().First(badges => badges.IsEffectivelyVisible);

        private List<Panel> KindBadgesOf(TreeNode node) =>
            BadgesOf(node).GetVisualDescendants().OfType<Panel>().Where(panel => panel.Classes.Contains("kindcount")).ToList();

        private Point MiddleOf(Control control)
        {
            window.Settle();
            return window.CenterOf(control);
        }

        private void MoveTo(Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        // The tooltip control is an internal value of the control, so the test reads it by its property.
        private ToolTip OpenTooltip(Control control)
        {
            ToolTip.SetIsOpen(control, true);
            window.Settle();
            var property = (AvaloniaProperty)typeof(ToolTip).GetField("ToolTipProperty",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!.GetValue(null)!;
            return (ToolTip)control.GetValue(property)!;
        }

        // Opens the tooltip of a control and returns the texts that the user sees.
        private List<string> TextOfTooltip(Control control)
        {
            var texts = OpenTooltip(control).GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text ?? string.Empty).ToList();
            ToolTip.SetIsOpen(control, false);
            return texts;
        }

        [AvaloniaFact]
        public void Create_TheTree_SwitchesTheTooltipServiceOffForItsRows()
        {
            Assert.False(ToolTip.GetServiceEnabled(view.Rows));
            Assert.False(ToolTip.GetServiceEnabled(MaskOf(Composition)));
        }

        [AvaloniaFact]
        public void Hover_AControlWithADelay_WaitsForTheTimerBeforeItOpens()
        {
            var mask = MaskOf(Composition);

            view.Tooltips.Hover(mask);
            Assert.False(ToolTip.GetIsOpen(mask));
            Assert.Same(mask, view.Tooltips.Hovered);

            view.Tooltips.Show();
            Assert.True(ToolTip.GetIsOpen(mask));
        }

        [AvaloniaFact]
        public void Hover_AnotherControlOrNone_ClosesTheOpenTooltip()
        {
            var first = MaskOf(Composition);
            var second = RowOf(Composition.Children[0]).GetVisualDescendants().OfType<ToggleButton>()
                .First(toggle => toggle != first && ToolTip.GetTip(toggle) != null && toggle.IsEffectivelyVisible);
            ToolTip.SetShowDelay(first, 0);
            ToolTip.SetShowDelay(second, 0);
            view.Tooltips.Hover(first);

            view.Tooltips.Hover(second);
            Assert.False(ToolTip.GetIsOpen(first));
            Assert.True(ToolTip.GetIsOpen(second));

            view.Tooltips.Hover(null);
            Assert.False(ToolTip.GetIsOpen(second));
            Assert.Null(view.Tooltips.Open);
        }

        [AvaloniaFact]
        public void MouseMove_OntoTheMaskWithNoDelay_OpensItsTooltip()
        {
            var mask = MaskOf(Composition);
            ToolTip.SetShowDelay(mask, 0);

            MoveTo(MiddleOf(mask));

            Assert.True(ToolTip.GetIsOpen(mask));
            Assert.Same(mask, view.Tooltips.Open);
        }

        [AvaloniaFact]
        public void KeyPress_WhileATooltipWaits_StopsTheWait()
        {
            var mask = MaskOf(Composition);
            MoveTo(MiddleOf(mask));
            Assert.Same(mask, view.Tooltips.Hovered);

            window.KeyPressQwerty(PhysicalKey.A, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            Assert.Null(view.Tooltips.Hovered);
        }

        [AvaloniaFact]
        public void KeyPress_WithAnOpenTooltip_ClosesItAndMutesTheTooltips()
        {
            var mask = MaskOf(Composition);
            ToolTip.SetShowDelay(mask, 0);
            MoveTo(MiddleOf(mask));
            Assert.True(ToolTip.GetIsOpen(mask));

            window.KeyPressQwerty(PhysicalKey.A, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            Assert.False(ToolTip.GetIsOpen(mask));
            Assert.Null(view.Tooltips.Open);
            Assert.True(view.Tooltips.IsMuted);
        }

        [AvaloniaFact]
        public void MouseMove_AfterAKeyPress_OpensNoTooltipUntilTheMoveIsLongerThanTheWakeDistance()
        {
            var mask = MaskOf(Composition);
            ToolTip.SetShowDelay(mask, 0);
            Assert.True(mask.Bounds.Width > 2 * TreeTooltips.WakeDistance + 2);
            var middle = MiddleOf(mask);
            var start = middle - new Vector(TreeTooltips.WakeDistance / 2 + 1, 0);
            MoveTo(start);
            window.KeyPressQwerty(PhysicalKey.A, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            MoveTo(start + new Vector(TreeTooltips.WakeDistance - 1, 0));
            Assert.False(ToolTip.GetIsOpen(mask));
            Assert.True(view.Tooltips.IsMuted);

            MoveTo(start + new Vector(TreeTooltips.WakeDistance + 1, 0));
            Assert.True(ToolTip.GetIsOpen(mask));
            Assert.False(view.Tooltips.IsMuted);
        }

        [AvaloniaFact]
        public void HostOf_AChildOfAControlWithATip_IsThatControl()
        {
            var mask = MaskOf(Composition);
            var inside = mask.GetVisualDescendants().OfType<Visual>().Last();

            Assert.Same(mask, TreeTooltips.HostOf(inside));
        }

        [AvaloniaFact]
        public void HostOf_ADisabledControl_IsSkippedUnlessItShowsOnDisabled()
        {
            var mask = MaskOf(Composition);
            mask.IsEnabled = false;
            var parentHost = TreeTooltips.HostOf(mask);

            Assert.NotSame(mask, parentHost);

            ToolTip.SetShowOnDisabled(mask, true);
            Assert.Same(mask, TreeTooltips.HostOf(mask));
        }

        [AvaloniaFact]
        public void Show_TheMaskOfEachRow_HasTheHoverFillOfTheKindOfItsNode()
        {
            var masked = ListTreeTestScreen.All(Tree.Roots).Where(HasMask).ToList();
            Assert.NotEmpty(masked);
            var palette = CategoryPalette.FromApplication();
            foreach (var node in masked)
            {
                var expected = palette.For(node.Category!.Value, node.ComposableDepth, node.KindDepth).Of(ToneRole.FillHover);

                var fill = ToolTipCard.GetFill(MaskOf(node));

                Assert.Equal(expected, Assert.IsAssignableFrom<ISolidColorBrush>(fill).Color);
            }
        }

        [AvaloniaFact]
        public void Show_TheMaskToggle_HasItsTooltipForACompositionAndALayer()
        {
            foreach (var node in new[] { Composition, Layer })
                Assert.Equal(new[] { "Mask" }, TextOfTooltip(MaskOf(node)));
        }

        [AvaloniaFact]
        public void Show_TheVisibilityToggle_HasTheVisibleTip()
        {
            var toggle = RowOf(Composition).Toggles.GetVisualDescendants().OfType<ToggleButton>()
                .First(button => ReferenceEquals(button.Theme, Application.Current!.FindResource("ToggleButtonVisibility")));

            Assert.Equal("Visible", ToolTip.GetTip(toggle));
            Assert.Equal(new[] { "Visible" }, TextOfTooltip(toggle));
        }

        [AvaloniaFact]
        public void Show_AKindBadge_TellsHowManyOfWhichKindInItsTooltip()
        {
            Layer.IsExpanded = false;
            window.Settle();

            var texts = KindBadgesOf(Layer).Select(badge => TextOfTooltip(badge).Single()).ToList();

            Assert.Equal(new[] { "2 Entities", "1 Filter", "3 Modifiers" }, texts);
        }

        // The style of the count of a hovered row must not reach the text of its tooltip, on the dark card.
        [AvaloniaFact]
        public void Show_AKindBadgeOfAHoveredRow_KeepsTheTextOfItsTooltipBright()
        {
            Layer.IsExpanded = false;
            window.Settle();
            window.Hover(RowOf(Layer));
            Assert.True(BadgesOf(Layer).IsHovered);
            var badge = KindBadgesOf(Layer).First();

            var text = OpenTooltip(badge).GetVisualDescendants().OfType<TextBlock>().Single();
            var source = global::Avalonia.Diagnostics.AvaloniaObjectExtensions.GetDiagnostic(text, TextBlock.ForegroundProperty).Priority;
            ToolTip.SetIsOpen(badge, false);

            Assert.Equal("2 Entities", text.Text);
            // The color of the text comes from the template of the tooltip (ActiveForegroundBrush), not from a style of the badges.
            Assert.Equal(global::Avalonia.Data.BindingPriority.Template, source);
        }

        [AvaloniaFact]
        public void Show_TheCountOfAGroup_NamesItsKindInItsTooltip()
        {
            var entities = Layer.Children.Single(child => child.Group is EntitiesGroup);
            entities.IsExpanded = false;
            window.Settle();

            var badge = Assert.Single(KindBadgesOf(entities));

            Assert.Equal("2 Entities", TextOfTooltip(badge).Single());
        }

        // A layer cannot hold a modifier of an entity: during its drag the layer row gives the reason in its tooltip.
        [AvaloniaFact]
        public void BeginDrag_ARowThatBlocksTheDrop_ShowsTheReasonInItsTooltip()
        {
            var entity = Layer.Children.Single(child => child.Group is EntitiesGroup).Children[0];
            ((Entity)entity.Item!).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            window.Settle();
            var modifier = entity.Children.Single(child => child.Group is ModifiersGroup).Children[0];
            var row = RowOf(Layer).Band;
            Assert.Null(ToolTip.GetTip(row));

            Tree.BeginDrag(modifier);
            window.Settle();

            Assert.Equal(new[] { "A Translate cannot go to a layer" }, TextOfTooltip(row));

            Tree.EndDrag();
            window.Settle();
            Assert.Null(ToolTip.GetTip(row));
        }
    }
}
