// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The keys of the new list tree give the cursor, the selection and the open state that the keys of the old tree give, on one tree.
    // The two views stand side by side. Each key runs once in each view from the same start.
    public class ListTreeKeysTests : IDisposable
    {
        public enum Start
        {
            OpenEntities,
            ClosedEntities,
            FirstEntity
        }

        private sealed record State(TreeNode? Cursor, TreeNode? Selected, TreeNode? Inspected, bool[] Open);

        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly TreeViewProject oldView;
        private readonly ListTreeProject newView;

        public ListTreeKeysTests()
        {
            oldView = new TreeViewProject { DataContext = scene.Tree };
            newView = new ListTreeProject { DataContext = scene.Tree };
            newView.Rows.MoveDuration = TimeSpan.Zero;
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            Grid.SetColumn(newView, 1);
            grid.Children.Add(oldView);
            grid.Children.Add(newView);
            window = new Window { Content = grid, Width = 900, Height = 900 };
            window.Show();
            foreach (var panel in oldView.GetVisualDescendants().OfType<PanelCollapse>())
                panel.Duration = TimeSpan.Zero;
            Settle();
        }

        public void Dispose()
        {
            oldView.MenuAddPanel.Close();
            newView.MenuAddPanel.Close();
            window.Close();
        }

        private ProjectTree Tree => scene.Tree;

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private void Settle() => TreeTestScreen.Settle(window);

        private void Press(PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            window.KeyPressQwerty(key, modifiers);
            Settle();
        }

        private State Capture() => new(Tree.CursorNode, Tree.SelectedNode, Tree.InspectedNode, TreeTestScreen.All(Tree.Roots).Select(node => node.IsExpanded).ToArray());

        private static bool IsSame(State a, State b) =>
            a.Cursor == b.Cursor && a.Selected == b.Selected && a.Inspected == b.Inspected && a.Open.SequenceEqual(b.Open);

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(newView.Rows.RowOf(node));

        private IInputElement? Focused() => window.FocusManager!.GetFocusedElement();

        // Every card is closed but the path to the entities of the first layer. The first layer is selected, the cursor is on the start row.
        private void Begin(Start start)
        {
            foreach (var node in TreeTestScreen.All(Tree.Roots))
                node.IsExpanded = false;
            foreach (var parent in TreeWalk.ParentsOf(Entity))
                parent.IsExpanded = true;
            Entities.IsExpanded = start != Start.ClosedEntities;
            Tree.SelectedNode = Layer;
            Tree.Inspect(null);
            Tree.CursorNode = start == Start.FirstEntity ? Entity : Entities;
            Settle();
        }

        private State PressInTheOldView(Start start, PhysicalKey key)
        {
            Begin(start);
            TreeTestScreen.ItemOf(oldView, Tree.CursorNode!).Focus();
            Settle();
            Press(key);
            return Capture();
        }

        private State PressInTheNewView(Start start, PhysicalKey key)
        {
            Begin(start);
            RowOf(Tree.CursorNode!).Focus();
            Settle();
            Press(key);
            return Capture();
        }

        [AvaloniaTheory]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowUp)]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowDown)]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowLeft)]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowRight)]
        [InlineData(Start.OpenEntities, PhysicalKey.Home)]
        [InlineData(Start.OpenEntities, PhysicalKey.Enter)]
        [InlineData(Start.OpenEntities, PhysicalKey.Space)]
        [InlineData(Start.ClosedEntities, PhysicalKey.ArrowLeft)]
        [InlineData(Start.ClosedEntities, PhysicalKey.ArrowRight)]
        [InlineData(Start.FirstEntity, PhysicalKey.ArrowUp)]
        [InlineData(Start.FirstEntity, PhysicalKey.ArrowDown)]
        [InlineData(Start.FirstEntity, PhysicalKey.ArrowLeft)]
        [InlineData(Start.FirstEntity, PhysicalKey.Enter)]
        public void Press_AKeyInTheNewView_GivesTheStateOfTheOldView(Start start, PhysicalKey key)
        {
            Begin(start);
            var before = Capture();
            var expected = PressInTheOldView(start, key);

            var actual = PressInTheNewView(start, key);

            Assert.False(IsSame(before, actual));
            Assert.Same(expected.Cursor, actual.Cursor);
            Assert.Same(expected.Selected, actual.Selected);
            Assert.Same(expected.Inspected, actual.Inspected);
            Assert.Equal(expected.Open, actual.Open);
        }

        [AvaloniaFact]
        public void Press_AKeyInTheNewView_LeavesTheFocusOnTheCursorRow()
        {
            Begin(Start.OpenEntities);
            RowOf(Entities).Focus();
            Settle();

            Press(PhysicalKey.ArrowDown);

            Assert.Same(Entity, Tree.CursorNode);
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Press_EndInTheNewView_PutsTheCursorOnTheLastShownRow()
        {
            Begin(Start.OpenEntities);
            RowOf(Entities).Focus();
            Settle();

            Press(PhysicalKey.End);

            var last = ListTreeSnapshot.Of(Tree.Roots).VisibleRows()[^1].Node;
            Assert.Same(last, Tree.CursorNode);
            Assert.NotNull(newView.Rows.RowOf(last!));
        }

        // The old tree shows the empty lists of a row, the new tree does not. So the cursor never goes to a row that the new tree does not show.
        [AvaloniaFact]
        public void Press_DownInTheNewView_SkipsTheEmptyListsThatItDoesNotShow()
        {
            Begin(Start.OpenEntities);
            var shown = ListTreeSnapshot.Of(Tree.Roots).VisibleRows().Select(row => row.Node).ToArray();
            RowOf(Entities).Focus();
            Settle();

            for (var i = 0; i < shown.Length; i++)
            {
                Press(PhysicalKey.ArrowDown);
                Assert.Contains(Tree.CursorNode, shown);
            }
        }

        [AvaloniaFact]
        public void Press_RightOnARowWithNoShownRowInTheNewView_OpensTheAddPanelBesideIt()
        {
            Begin(Start.FirstEntity);
            RowOf(Entity).Focus();
            Settle();

            Press(PhysicalKey.ArrowRight);

            Assert.True(newView.MenuAddPanel.IsOpen);
            Assert.Same(Entity, Tree.CursorNode);
        }

        [AvaloniaFact]
        public void Press_EscapeInTheNewViewDuringASearch_ClearsTheSearch()
        {
            Begin(Start.OpenEntities);
            Tree.SearchText = "Entity";
            RowOf(Entities).Focus();
            Settle();
            Assert.True(Tree.IsSearchActive);

            Press(PhysicalKey.Escape);

            Assert.False(Tree.IsSearchActive);
        }
    }
}
