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
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The keys of the list tree move the cursor and the focus, and open the add panel, on one tree.
    public class ListTreeKeysTests : IDisposable
    {
        public enum Start
        {
            OpenEntities,
            ClosedEntities,
            FirstEntity
        }

        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject newView;

        public ListTreeKeysTests()
        {
            (window, newView) = ListTreeTestScreen.Show(scene.Tree, 900, 900);
        }

        public void Dispose()
        {
            newView.MenuAddPanel.Close();
            window.Close();
        }

        private ProjectTree Tree => scene.Tree;

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(newView.Rows.RowOf(node));

        private IInputElement? Focused() => window.FocusManager!.GetFocusedElement();

        // Every card is closed but the path to the entities of the first layer. The first layer is selected, the cursor is on the start row.
        private void Begin(Start start)
        {
            foreach (var node in ListTreeTestScreen.All(Tree.Roots))
                node.IsExpanded = false;
            foreach (var parent in TreeWalk.ParentsOf(Entity))
                parent.IsExpanded = true;
            Entities.IsExpanded = start != Start.ClosedEntities;
            Tree.SelectedNode = Layer;
            Tree.Inspect(null);
            Tree.CursorNode = start == Start.FirstEntity ? Entity : Entities;
            window.Settle();
        }

        public enum Row
        {
            Compositions,
            Layer,
            Entities,
            Entity,
            SecondEntity,
            None
        }

        private TreeNode? NodeOf(Row row) => row switch
        {
            Row.Compositions => Tree.Roots[0],
            Row.Layer => Layer,
            Row.Entities => Entities,
            Row.Entity => Entity,
            Row.SecondEntity => Entities.Children[1],
            _ => null
        };

        // The state after a key: the cursor, the selection, the inspected row and whether the entities are open. The rest stays open.
        [AvaloniaTheory]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowUp, Row.Layer, Row.Layer, Row.None, true)]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowDown, Row.Entity, Row.Layer, Row.None, true)]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowLeft, Row.Entities, Row.Layer, Row.None, false)]
        [InlineData(Start.OpenEntities, PhysicalKey.ArrowRight, Row.Entity, Row.Layer, Row.None, true)]
        [InlineData(Start.OpenEntities, PhysicalKey.Home, Row.Compositions, Row.Layer, Row.None, true)]
        [InlineData(Start.OpenEntities, PhysicalKey.Enter, Row.Entities, Row.Entities, Row.None, true)]
        [InlineData(Start.OpenEntities, PhysicalKey.Space, Row.Entities, Row.Layer, Row.Entities, true)]
        [InlineData(Start.ClosedEntities, PhysicalKey.ArrowLeft, Row.Layer, Row.Layer, Row.None, false)]
        [InlineData(Start.ClosedEntities, PhysicalKey.ArrowRight, Row.Entities, Row.Layer, Row.None, true)]
        [InlineData(Start.FirstEntity, PhysicalKey.ArrowUp, Row.Entities, Row.Layer, Row.None, true)]
        [InlineData(Start.FirstEntity, PhysicalKey.ArrowDown, Row.SecondEntity, Row.Layer, Row.None, true)]
        [InlineData(Start.FirstEntity, PhysicalKey.ArrowLeft, Row.Entities, Row.Layer, Row.None, true)]
        [InlineData(Start.FirstEntity, PhysicalKey.Enter, Row.Entity, Row.Entity, Row.None, true)]
        public void Press_AKey_GivesTheCursorTheSelectionAndTheOpenRows(Start start, PhysicalKey key, Row cursor, Row selected, Row inspected, bool entitiesOpen)
        {
            Begin(start);
            RowOf(Tree.CursorNode!).Focus();
            window.Settle();

            window.Press(key);

            Assert.Same(NodeOf(cursor), Tree.CursorNode);
            Assert.Same(NodeOf(selected), Tree.SelectedNode);
            Assert.Same(NodeOf(inspected), Tree.InspectedNode);
            Assert.Equal(entitiesOpen, Entities.IsExpanded);
            Assert.All(TreeWalk.ParentsOf(Entities), parent => Assert.True(parent.IsExpanded));
        }

        [AvaloniaFact]
        public void Press_AKey_LeavesTheFocusOnTheCursorRow()
        {
            Begin(Start.OpenEntities);
            RowOf(Entities).Focus();
            window.Settle();

            window.Press(PhysicalKey.ArrowDown);

            Assert.Same(Entity, Tree.CursorNode);
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Press_End_PutsTheCursorOnTheLastShownRow()
        {
            Begin(Start.OpenEntities);
            RowOf(Entities).Focus();
            window.Settle();

            window.Press(PhysicalKey.End);

            var last = ListTreeSnapshot.Of(Tree.Roots).VisibleRows()[^1].Node;
            Assert.Same(last, Tree.CursorNode);
            Assert.NotNull(newView.Rows.RowOf(last!));
        }

        // The tree does not show the empty lists of a row. So the cursor never goes to a row that the tree does not show.
        [AvaloniaFact]
        public void Press_Down_SkipsTheEmptyListsThatItDoesNotShow()
        {
            Begin(Start.OpenEntities);
            var shown = ListTreeSnapshot.Of(Tree.Roots).VisibleRows().Select(row => row.Node).ToArray();
            RowOf(Entities).Focus();
            window.Settle();

            for (var i = 0; i < shown.Length; i++)
            {
                window.Press(PhysicalKey.ArrowDown);
                Assert.Contains(Tree.CursorNode, shown);
            }
        }

        [AvaloniaFact]
        public void Press_RightOnARowWithNoShownRow_OpensTheAddPanelBesideIt()
        {
            Begin(Start.FirstEntity);
            RowOf(Entity).Focus();
            window.Settle();

            window.Press(PhysicalKey.ArrowRight);

            Assert.True(newView.MenuAddPanel.IsOpen);
            Assert.Same(Entity, Tree.CursorNode);
        }

        [AvaloniaFact]
        public void Press_EscapeDuringASearch_ClearsTheSearch()
        {
            Begin(Start.OpenEntities);
            Tree.SearchText = "Entity";
            RowOf(Entities).Focus();
            window.Settle();
            Assert.True(Tree.IsSearchActive);

            window.Press(PhysicalKey.Escape);

            Assert.False(Tree.IsSearchActive);
        }
    }
}
