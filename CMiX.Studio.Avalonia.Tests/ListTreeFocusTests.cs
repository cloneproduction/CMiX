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
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;

namespace CMiX.Studio.Avalonia.Tests
{
    // The focus of the list tree stays in its rows after a click and a row button. The cursor has a frame of its own.
    public class ListTreeFocusTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;
        private readonly Button outside = new() { Content = "Outside" };

        public ListTreeFocusTests()
        {
            (window, view) = ListTreeTestScreen.Show(scene.Tree, 400, 900, outside);
            scene.Tree.SelectedNode = Layer;
            window.Settle();
        }

        public void Dispose() => window.Close();

        private ProjectTree Tree => scene.Tree;

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(node));

        private IInputElement? Focused() => window.FocusManager!.GetFocusedElement();

        private static ToggleButton VisibilityToggleOf(RowListTree row) =>
            row.Toggles.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "visibilityToggle" && toggle.IsEffectivelyVisible);

        private void FocusRow(TreeNode node)
        {
            RowOf(node).Focus();
            window.Settle();
            Assert.Same(RowOf(node), Focused());
        }

        [AvaloniaFact]
        public void Click_ARow_SelectsItAndGivesItTheFocus()
        {
            window.Click(Named(RowOf(Entity), "band"));

            Assert.Same(Entity, Tree.SelectedNode);
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Click_TheToggleOfTheFocusedRow_TogglesAndKeepsTheFocusOnTheRow()
        {
            FocusRow(Entity);
            var toggle = VisibilityToggleOf(RowOf(Entity));
            var before = toggle.IsChecked;

            window.Click(toggle);

            Assert.NotEqual(before, toggle.IsChecked);
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Click_TheToggleOfAnotherRow_GivesTheFocusBackToTheFocusedRow()
        {
            FocusRow(Entity);
            var other = RowOf(Entities.Children[1]);

            window.Click(VisibilityToggleOf(other));

            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Click_BesideTheRows_GivesTheFocusToTheCursorRow()
        {
            outside.Focus();
            window.Settle();
            var bottom = view.Rows.TranslatePoint(new Point(view.Rows.Bounds.Width / 2, view.Rows.Bounds.Height - 1), window)!.Value;
            var gap = new Point(bottom.X, bottom.Y + 2);

            window.Click(gap);

            Assert.Same(RowOf(Tree.CursorNode!), Focused());
        }

        [AvaloniaFact]
        public void Toggles_OfARow_AreNoTabStops()
        {
            var toggles = RowOf(Entity).Toggles.GetVisualDescendants().OfType<ToggleButton>().ToArray();

            Assert.NotEmpty(toggles);
            Assert.All(toggles, toggle => Assert.False(toggle.Focusable && toggle.IsTabStop));
        }

        [AvaloniaFact]
        public void Tab_FromARow_LeavesTheTreeInOneStep()
        {
            FocusRow(Entity);

            window.Press(PhysicalKey.Tab);

            Assert.Same(outside, Focused());
        }

        [AvaloniaFact]
        public void MiddleClick_OnARow_SelectsAndInspectsIt()
        {
            window.Click(Named(RowOf(Entity), "band"), MouseButton.Middle);

            Assert.Same(Entity, Tree.SelectedNode);
            Assert.Same(Entity, Tree.InspectedNode);
            Assert.Same(Entity, Tree.CursorNode);
        }

        [AvaloniaFact]
        public void TreeFocusRequested_TheNewViewIsActive_FocusesTheTargetRow()
        {
            outside.Focus();
            window.Settle();
            Tree.CursorNode = Entity;

            Tree.FocusTree();
            window.Settle();

            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void CursorFrame_TheCursorRow_IsAThinFrameAtFullWidth()
        {
            Tree.CursorNode = Entity;
            window.Settle();

            var frame = Named(RowOf(Entity), "cursorFrame");

            Assert.True(frame.IsEffectivelyVisible);
            Assert.Equal((Thickness)view.FindResource("TreeSelectionThickness")!, frame.BorderThickness);
            Assert.Equal(0, frame.Bounds.X);
            Assert.Equal(RowOf(Entity).Bounds.Width, frame.Bounds.Width);
        }

        [AvaloniaFact]
        public void CursorFrame_TheBrush_IsTheCursorFrameToken()
        {
            Tree.CursorNode = Entity;
            window.Settle();

            Assert.Equal(Token("ListTreeRowCursorFrameBrush"), ColorOf(Named(RowOf(Entity), "cursorFrame").BorderBrush));
        }

        [AvaloniaFact]
        public void CursorFrame_ASelectedCursorRow_ShowsTheFrame()
        {
            Tree.SelectByClick(Entity);
            Tree.CursorNode = Entity;
            window.Settle();

            var frame = Named(RowOf(Entity), "cursorFrame");

            Assert.True(RowOf(Entity).IsSelected);
            Assert.True(frame.IsEffectivelyVisible);
            Assert.Equal(1, frame.Opacity);
        }

        [AvaloniaFact]
        public void CursorFrame_ARowThatIsNotTheCursor_IsHidden()
        {
            Tree.CursorNode = Entity;
            window.Settle();

            Assert.False(Named(RowOf(Entities.Children[1]), "cursorFrame").IsEffectivelyVisible);
        }

        // The view leaves the window with the focus in a row and comes back: the focus does not jump back into the rows.
        [AvaloniaFact]
        public void Attach_AgainAfterADetachWithTheFocusInARow_TakesNoFocus()
        {
            var panel = (DockPanel)window.Content!;
            RowOf(Entity).Focus();
            window.Settle();

            panel.Children.Remove(view);
            window.Settle();
            panel.Children.Add(view);
            window.Settle();

            Assert.False(Focused() is RowListTree, $"focused: {Focused()?.GetType().Name ?? "nothing"}");
        }
    }
}
