// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // The keys of the list tree move the cursor and the focus, open the add panel and run the Tree keys of the main window.
    // The arrow keys move the cursor only. The key bindings come from the shortcut map.
    public class ListTreeKeysTests : IDisposable
    {
        public enum Start
        {
            OpenEntities,
            ClosedEntities,
            FirstEntity
        }

        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private Window? window;
        private ListTreeProject? newView;

        public void Dispose()
        {
            foreach (var view in window?.GetVisualDescendants().OfType<ListTreeProject>() ?? Enumerable.Empty<ListTreeProject>())
                view.MenuAddPanel.Close();
            window?.Close();
        }

        private ProjectTree Tree => scene.Tree;

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(newView!.Rows.RowOf(node));

        private IInputElement? Focused() => window!.FocusManager!.GetFocusedElement();

        // Every card is closed but the path to the entities of the first layer. The first layer is selected, the cursor is on the start row.
        private void Begin(Start start)
        {
            (window, newView) = ListTreeTestScreen.Show(Tree, 900, 900);
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
            window!.Settle();

            window!.Press(key);

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
            window!.Settle();

            window!.Press(PhysicalKey.ArrowDown);

            Assert.Same(Entity, Tree.CursorNode);
            Assert.Same(RowOf(Entity), Focused());
        }

        [AvaloniaFact]
        public void Press_End_PutsTheCursorOnTheLastShownRow()
        {
            Begin(Start.OpenEntities);
            RowOf(Entities).Focus();
            window!.Settle();

            window!.Press(PhysicalKey.End);

            var last = ListTreeSnapshot.Of(Tree.Roots).VisibleRows()[^1].Node;
            Assert.Same(last, Tree.CursorNode);
            Assert.NotNull(newView!.Rows.RowOf(last!));
        }

        // The tree does not show the empty lists of a row. So the cursor never goes to a row that the tree does not show.
        [AvaloniaFact]
        public void Press_Down_SkipsTheEmptyListsThatItDoesNotShow()
        {
            Begin(Start.OpenEntities);
            var shown = ListTreeSnapshot.Of(Tree.Roots).VisibleRows().Select(row => row.Node).ToArray();
            RowOf(Entities).Focus();
            window!.Settle();

            for (var i = 0; i < shown.Length; i++)
            {
                window!.Press(PhysicalKey.ArrowDown);
                Assert.Contains(Tree.CursorNode, shown);
            }
        }

        [AvaloniaFact]
        public void Press_RightOnARowWithNoShownRow_OpensTheAddPanelBesideIt()
        {
            Begin(Start.FirstEntity);
            RowOf(Entity).Focus();
            window!.Settle();

            window!.Press(PhysicalKey.ArrowRight);

            Assert.True(newView!.MenuAddPanel.IsOpen);
            Assert.Same(Entity, Tree.CursorNode);
        }

        [AvaloniaFact]
        public void Press_EscapeDuringASearch_ClearsTheSearch()
        {
            Begin(Start.OpenEntities);
            Tree.SearchText = "Entity";
            RowOf(Entities).Focus();
            window!.Settle();
            Assert.True(Tree.IsSearchActive);

            window!.Press(PhysicalKey.Escape);

            Assert.False(Tree.IsSearchActive);
        }

        private sealed record LayersScreen(ListTreeProject View, ProjectTree Tree, UndoManager Undo, Composition Composition);

        // One composition with three closed layers. The first layer is selected and its row has the focus, the undo history is empty.
        private LayersScreen ShowLayers()
        {
            var layers = CreateScene([3]);
            var (tree, undo, composition) = (layers.Tree, layers.Undo, layers.A);
            (window, var view) = ListTreeTestScreen.Show(tree, 500, 700);
            tree.CompositionNodes()[0].IsExpanded = true;
            tree.CompositionNodes()[0].Children[0].IsExpanded = true;
            foreach (var layer in tree.CompositionNodes()[0].Children[0].Children)
                layer.IsExpanded = false;
            window.Settle();

            tree.SelectedNode = tree.CompositionNodes()[0].Children[0].Children[0];
            window.Settle();
            view.Rows.RowOf(tree.CompositionNodes()[0].Children[0].Children[0])!.Focus();
            window.Settle();
            undo.Clear();
            return new LayersScreen(view, tree, undo, composition);
        }

        [AvaloniaFact]
        public void ArrowDown_MovesTheCursor_WithNoUndoStep()
        {
            var layers = ShowLayers();
            var first = layers.Tree.CompositionNodes()[0].Children[0].Children[0];
            var second = layers.Tree.CompositionNodes()[0].Children[0].Children[1];
            var selectedLayer = layers.Composition.LayerManager.SelectedItem;

            window!.Press(PhysicalKey.ArrowDown);

            Assert.Same(second, layers.Tree.CursorNode);
            Assert.Same(first, layers.Tree.SelectedNode);
            Assert.Same(selectedLayer, layers.Composition.LayerManager.SelectedItem);
            Assert.False(layers.Undo.CanUndo);
        }

        [AvaloniaFact]
        public void Press_Enter_SelectsTheChain()
        {
            var layers = ShowLayers();
            var second = layers.Tree.CompositionNodes()[0].Children[0].Children[1];
            window!.Press(PhysicalKey.ArrowDown);

            window!.Press(PhysicalKey.Enter);

            Assert.Same(second, layers.Tree.SelectedNode);
            Assert.Equal(SelectionOrigin.Keyboard, layers.Tree.Origin);
            Assert.Same(second.Item, layers.Composition.LayerManager.SelectedItem);
        }

        [AvaloniaFact]
        public void KeyBindings_WhileANameBoxIsEdited_ArePaused()
        {
            var layers = ShowLayers();

            // The key of the rename is a binding of the main window (see the window key tests below), so the test starts the rename itself.
            layers.Tree.RenameSelectedCommand.Execute(null);
            window!.Settle();
            Assert.Empty(layers.View.KeyBindings);

            window!.Press(PhysicalKey.Escape);
            layers.View.Rows.RowOf(layers.Tree.CompositionNodes()[0].Children[0].Children[0])!.Focus();
            window!.Settle();
            Assert.Equal(2, layers.View.KeyBindings.Count);
        }

        private static bool InTree(IInputElement? element, ListTreeProject view) => element is Visual visual && view.Rows.GetVisualDescendants().Contains(visual);

        [AvaloniaFact]
        public void ShiftTab_FromOutsideTheTree_ComesBackToTheTreeInOneStep()
        {
            var layers = ShowLayers();
            window!.Press(PhysicalKey.Tab);
            Assert.False(InTree(window!.FocusManager!.GetFocusedElement(), layers.View));

            window!.Press(PhysicalKey.Tab, RawInputModifiers.Shift);

            Assert.IsType<RowListTree>(window.FocusManager!.GetFocusedElement());
        }

        // The main window with one composition with three layers. The first layer is selected, and its row has the focus.
        // The Tree keys of the window run through CanRunTreeKey: with the focus in the list tree, its hooks let them run.
        private WindowScreen ShowMainWindow()
        {
            var screen = new WindowScreen(ListTreeMainScreen.Show(panel: PanelChoice.Tree));
            window = screen.Window;
            screen.Tree.SelectedNode = screen.LayerNode(0);
            screen.Window.Settle();
            screen.View.Rows.RowOf(screen.LayerNode(0))!.Focus();
            screen.Window.Settle();
            Assert.True(screen.View.IsKeyboardFocusWithin);
            Assert.True(screen.View.IsActiveView);
            return screen;
        }

        private sealed record WindowScreen : ListTreeMainScreen
        {
            public WindowScreen(ListTreeMainScreen shown) : base(shown) { }

            public IList<IControl> Layers => Composition.LayerManager.ManagerData.Items;
        }

        [AvaloniaFact]
        public void Press_ShiftDWithTheFocusInTheNewView_DuplicatesTheSelectedRow()
        {
            var screen = ShowMainWindow();

            screen.Window.Press(PhysicalKey.D, RawInputModifiers.Shift);

            Assert.Equal(4, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Press_AltDownWithTheFocusInTheNewView_MovesTheRowAndKeepsTheFocusOnIt()
        {
            var screen = ShowMainWindow();
            var first = screen.Layers[0];

            screen.Window.Press(PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Same(first, screen.Layers[1]);
            Assert.Same(screen.View.Rows.RowOf(screen.Tree.SelectedNode!), screen.Window.FocusManager!.GetFocusedElement());
        }

        [AvaloniaFact]
        public void Press_EWithTheFocusInTheNewView_ClosesTheTargetRow()
        {
            var screen = ShowMainWindow();
            var composition = screen.Tree.CompositionNodes()[0];
            screen.Tree.CursorNode = composition;

            screen.Window.Press(PhysicalKey.E);

            Assert.False(composition.IsExpanded);
        }

        [AvaloniaFact]
        public void Press_AltEWithTheFocusInTheNewView_ClosesTheOtherRows()
        {
            var screen = ShowMainWindow();
            var target = screen.LayerNode(0);
            var other = screen.LayerNode(1);
            Assert.True(other.IsExpanded);

            screen.Window.Press(PhysicalKey.E, RawInputModifiers.Alt);

            Assert.False(other.IsExpanded);
            Assert.True(target.IsExpanded);
        }
    }
}
