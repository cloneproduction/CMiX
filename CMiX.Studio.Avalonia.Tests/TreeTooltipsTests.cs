using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tooltips of the rows of the tree open and close by the tree itself, with the show delay of the control.
    public class TreeTooltipsTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        private (ListTreeProject View, ProjectTree Tree) Show()
        {
            var scene = CreateScene([1], s => AddKinds(s.Layer));
            var screen = ListTreeTestScreen.Show(scene.Tree, 500, 600);
            window = screen.Window;
            return (screen.View, scene.Tree);
        }

        private static ToggleButton MaskOf(ListTreeProject view, TreeNode node) =>
            view.Rows.RowOf(node)!.Toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => Equals(ToolTip.GetTip(toggle), "Mask") && toggle.IsEffectivelyVisible);

        [AvaloniaFact]
        public void Hover_AControlWithADelay_WaitsForTheTimerBeforeItOpens()
        {
            var (view, tree) = Show();
            var mask = MaskOf(view, tree.CompositionNodes()[0]);

            view.Tooltips.Hover(mask);
            Assert.False(ToolTip.GetIsOpen(mask));
            Assert.Same(mask, view.Tooltips.Hovered);

            view.Tooltips.Show();
            Assert.True(ToolTip.GetIsOpen(mask));
        }

        [AvaloniaFact]
        public void Hover_AnotherControlOrNone_ClosesTheOpenTooltip()
        {
            var (view, tree) = Show();
            var first = MaskOf(view, tree.CompositionNodes()[0]);
            var second = view.Rows.RowOf(tree.CompositionNodes()[0].Children[0])!.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle != first && ToolTip.GetTip(toggle) != null && toggle.IsEffectivelyVisible);
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

        // The middle of the control in the coordinates of the window, after a render so that the hit test finds it.
        private Point MiddleOf(Control control)
        {
            TreeDrag.Render(window!);
            return window!.CenterOf(control);
        }

        private void MoveTo(Point point)
        {
            window!.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void KeyPress_WhileATooltipWaits_StopsTheWait()
        {
            var (view, tree) = Show();
            var mask = MaskOf(view, tree.CompositionNodes()[0]);
            MoveTo(MiddleOf(mask));
            Assert.Same(mask, view.Tooltips.Hovered);

            window!.KeyPressQwerty(PhysicalKey.A, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            Assert.Null(view.Tooltips.Hovered);
        }

        [AvaloniaFact]
        public void MouseMove_AfterAKeyPress_OpensNoTooltipUntilTheMoveIsLongerThanTheWakeDistance()
        {
            var (view, tree) = Show();
            var mask = MaskOf(view, tree.CompositionNodes()[0]);
            ToolTip.SetShowDelay(mask, 0);
            Assert.True(mask.Bounds.Width > 2 * TreeTooltips.WakeDistance + 2);
            var middle = MiddleOf(mask);
            var start = middle - new Vector(TreeTooltips.WakeDistance / 2 + 1, 0);
            MoveTo(start);
            window!.KeyPressQwerty(PhysicalKey.A, RawInputModifiers.None);
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
            var (view, tree) = Show();
            var mask = MaskOf(view, tree.CompositionNodes()[0]);
            var inside = mask.GetVisualDescendants().OfType<Visual>().Last();

            Assert.Same(mask, TreeTooltips.HostOf(inside));
        }

        [AvaloniaFact]
        public void HostOf_ADisabledControl_IsSkippedUnlessItShowsOnDisabled()
        {
            var (view, tree) = Show();
            var mask = MaskOf(view, tree.CompositionNodes()[0]);
            mask.IsEnabled = false;
            var parentHost = TreeTooltips.HostOf(mask);

            Assert.NotSame(mask, parentHost);

            ToolTip.SetShowOnDisabled(mask, true);
            Assert.Same(mask, TreeTooltips.HostOf(mask));
        }
    }
}
