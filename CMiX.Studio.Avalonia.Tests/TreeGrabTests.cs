using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.Grab (G) grabs the target row as G in Blender: the drag starts with no pressed button. The pointer or the arrow keys move
    // the mark, a left click or Enter puts the row down as one undo step, and a right click, Escape or a focus in another panel ends the
    // grab with no move. The grab takes every click, so no row is chosen and no menu opens.
    public class TreeGrabTests : IDisposable
    {
        private Views.MainWindow? window;
        private TreeViewProject? view;

        // A grab that a test leaves running would take the input of the next tests, so it ends here first.
        public void Dispose()
        {
            view?.Grab.Detach();
            window?.Close();
        }

        private sealed record Screen(IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel, TreeViewProject View)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public UndoManager Undo => Provider.GetRequiredService<UndoManager>();
            public Composition Composition => (Composition)ViewModel.Project.CompositionManager.ManagerData.Items[0];
            public TreeNode LayersNode => Tree.CompositionNodes()[0].Children[0];
            public TreeNode LayerNode(int index) => LayersNode.Children[index];
            public Layer Layer(int index) => (Layer)Composition.LayerManager.ManagerData.Items[index];

            public int UndoSteps =>
                ((LinkedList<IUndoCommand>)typeof(UndoManager).GetField("_undoStack", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(Undo)!).Count;
        }

        // One composition with three layers. Every row is open. The composition is selected. The undo history is empty.
        private Screen Show()
        {
            var (provider, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 3; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var view = mainWindow.GetVisualDescendants().OfType<TreeViewProject>().Single();
            this.view = view;
            var screen = new Screen(provider, mainWindow, viewModel, view);
            // The rows open and close with no animation, so no row moves while a test reads its place.
            foreach (var panel in view.GetVisualDescendants().OfType<CMiX.UI.Panels.PanelCollapse>())
                panel.Duration = TimeSpan.Zero;
            // The layer rows are closed, so the place under a layer row is the place after that layer.
            for (var i = 0; i < 3; i++)
                screen.LayerNode(i).IsExpanded = false;
            screen.Tree.SelectedNode = screen.Tree.CompositionNodes()[0];
            Settle(mainWindow);
            screen.Undo.Clear();
            return screen;
        }

        // Puts the cursor on the node and the focus on its row.
        private static void Focus(Screen screen, TreeNode node)
        {
            screen.Tree.CursorNode = node;
            Settle(screen.Window);
            ItemOf(screen.View, node).Focus();
            Settle(screen.Window);
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            Settle(screen.Window);
        }

        // A point on the row of the node, at the fraction of its height, in the window.
        private static Point On(Screen screen, TreeNode node, double fraction = 0.5)
        {
            TreeDrag.Render(screen.Window);
            var row = RowOf(screen.View, node);
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height * fraction), screen.Window)!.Value;
        }

        // Grabs the first layer with the key, with the focus on its row. The pointer rests on the composition row first, so the grab
        // starts at a known place and not at the place that an earlier test left.
        private static void GrabFirstLayer(Screen screen)
        {
            // A row under a resting pointer opens after a real delay during a drag. The tests do not wait for it, so it never comes.
            screen.View.DragSession.RestDelay = TimeSpan.FromHours(1);
            screen.Window.MouseMove(On(screen, screen.Tree.CompositionNodes()[0]));
            Focus(screen, screen.LayerNode(0));
            Press(screen, PhysicalKey.G);
            Assert.True(screen.View.Grab.IsGrabbing);
        }

        private Layer[] Layers(Screen screen) => Enumerable.Range(0, 3).Select(screen.Layer).ToArray();

        [AvaloniaFact]
        public void G_OnALayer_StartsTheGrabWithTheFrameAndTheHints()
        {
            var screen = Show();

            GrabFirstLayer(screen);

            Assert.True(screen.Tree.IsDragging);
            Assert.True(screen.View.Classes.Contains(TreeViewProject.GrabbingClass));
            Assert.True(screen.View.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "grabFrame").IsEffectivelyVisible);
            Assert.Equal(TreeHintMode.Grab, screen.View.HintMode);
        }

        [AvaloniaFact]
        public void Escape_DuringTheGrab_EndsItAndMovesNothing()
        {
            var screen = Show();
            var before = Layers(screen);
            GrabFirstLayer(screen);
            Press(screen, PhysicalKey.ArrowDown);

            Press(screen, PhysicalKey.Escape);

            Assert.False(screen.View.Grab.IsGrabbing);
            Assert.False(screen.Tree.IsDragging);
            Assert.Equal(before, Layers(screen));
            Assert.Equal(0, screen.UndoSteps);
            Assert.False(screen.View.Classes.Contains(TreeViewProject.GrabbingClass));
            Assert.Equal(TreeHintMode.Idle, screen.View.HintMode);
        }

        [AvaloniaFact]
        public void Down_DuringTheGrab_MarksTheNextValidPlaceAndHidesTheLabel()
        {
            var screen = Show();
            GrabFirstLayer(screen);
            var slots = new TreeDropSlots(screen.Tree, screen.LayerNode(0));
            var next = slots.Slots[slots.Next(slots.StartIndex)];

            Press(screen, PhysicalKey.ArrowDown);

            Assert.Equal(new DropPlace(next.Target, next.Position), screen.View.DragSession.Slot);
            Assert.False(screen.View.DragSession.Ghost.Ghost.IsVisible);
        }

        [AvaloniaFact]
        public void Enter_AfterDown_PutsTheLayerAtTheMarkAsOneUndoStep()
        {
            var screen = Show();
            var before = Layers(screen);
            GrabFirstLayer(screen);
            Press(screen, PhysicalKey.ArrowDown);
            Press(screen, PhysicalKey.ArrowDown);

            Press(screen, PhysicalKey.Enter);

            Assert.False(screen.View.Grab.IsGrabbing);
            Assert.NotEqual(before, Layers(screen));
            Assert.Equal(1, screen.UndoSteps);
            screen.Undo.Undo();
            Assert.Equal(before, Layers(screen));
        }

        [AvaloniaFact]
        public void Enter_AtTheStartPlace_EndsTheGrabWithNoMove()
        {
            var screen = Show();
            var before = Layers(screen);
            GrabFirstLayer(screen);

            Press(screen, PhysicalKey.Enter);

            Assert.False(screen.View.Grab.IsGrabbing);
            Assert.Equal(before, Layers(screen));
            Assert.Equal(0, screen.UndoSteps);
            // Enter ended the grab: it did not select the cursor row (Tree.Open).
            Assert.Same(screen.Tree.CompositionNodes()[0], screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_AfterAMoveOfThePointer_PutsTheLayerUnderThePointerAndChoosesNoRow()
        {
            var screen = Show();
            var before = Layers(screen);
            var chosen = new List<TreeNode>();
            screen.View.NodeChosen += (_, e) => chosen.Add(e.Node);
            GrabFirstLayer(screen);
            // A first move past the take over distance gives the mark to the pointer. The rows can change their look then, so the
            // target is read after it, and the mark is checked before the click.
            screen.Window.MouseMove(On(screen, screen.LayerNode(1)));
            Settle(screen.Window);
            var target = On(screen, screen.LayerNode(2), 0.15);
            screen.Window.MouseMove(target);
            Settle(screen.Window);
            Assert.Equal(new DropPlace(screen.LayerNode(2), DropPosition.Before), screen.View.DragSession.Slot);

            screen.Window.MouseDown(target, MouseButton.Left);
            screen.Window.MouseUp(target, MouseButton.Left);
            Settle(screen.Window);

            Assert.False(screen.View.Grab.IsGrabbing);
            // The first layer is now before the last one: the order is 1, 0, 2.
            Assert.Equal(new[] { before[1], before[0], before[2] }, Layers(screen));
            Assert.Equal(1, screen.UndoSteps);
            Assert.Empty(chosen);
        }

        [AvaloniaFact]
        public void Click_WhereNoDropIsOffered_DoesNothingAndTheGrabGoesOn()
        {
            var screen = Show();
            var before = Layers(screen);
            GrabFirstLayer(screen);
            var onItself = On(screen, screen.LayerNode(0));

            screen.Window.MouseMove(onItself + new Vector(0, MoveFar));
            screen.Window.MouseMove(onItself);
            screen.Window.MouseDown(onItself, MouseButton.Left);
            screen.Window.MouseUp(onItself, MouseButton.Left);
            Settle(screen.Window);

            Assert.True(screen.View.Grab.IsGrabbing);
            Assert.Equal(before, Layers(screen));
            Assert.Equal(0, screen.UndoSteps);
        }

        [AvaloniaFact]
        public void RightClick_DuringTheGrab_EndsItAndOpensNoMenu()
        {
            var screen = Show();
            var before = Layers(screen);
            GrabFirstLayer(screen);
            var point = On(screen, screen.LayerNode(2), 0.9);
            screen.Window.MouseMove(point);

            screen.Window.MouseDown(point, MouseButton.Right);
            screen.Window.MouseUp(point, MouseButton.Right);
            Settle(screen.Window);

            Assert.False(screen.View.Grab.IsGrabbing);
            Assert.Equal(before, Layers(screen));
            Assert.NotEqual(true, screen.View.GetVisualDescendants().OfType<TreeView>().Single().ContextMenu?.IsOpen);
        }

        // A move of the pointer past the take over distance.
        private const double MoveFar = 3 * TreeGrab.MouseTakeOver;

        [AvaloniaFact]
        public void ASmallMoveOfThePointer_AfterAKey_KeepsTheMarkOfTheKey()
        {
            var screen = Show();
            GrabFirstLayer(screen);
            var start = On(screen, screen.LayerNode(0));
            screen.Window.MouseMove(start);
            Press(screen, PhysicalKey.ArrowDown);
            var marked = screen.View.DragSession.Slot;

            screen.Window.MouseMove(start + new Vector(0, TreeGrab.MouseTakeOver / 2));
            Settle(screen.Window);

            Assert.Equal(marked, screen.View.DragSession.Slot);
        }

        [AvaloniaFact]
        public void ALongMoveOfThePointer_AfterAKey_TakesTheMarkBack()
        {
            var screen = Show();
            GrabFirstLayer(screen);
            var start = On(screen, screen.LayerNode(0));
            screen.Window.MouseMove(start);
            Press(screen, PhysicalKey.ArrowDown);
            Press(screen, PhysicalKey.ArrowDown);
            var keySlot = screen.View.DragSession.Slot;
            var target = On(screen, screen.LayerNode(2), 0.15);

            screen.Window.MouseMove(target);
            Settle(screen.Window);
            Assert.NotEqual(keySlot, screen.View.DragSession.Slot);
            Assert.Equal(new DropPlace(screen.LayerNode(2), DropPosition.Before), screen.View.DragSession.Slot);
            Assert.True(screen.View.DragSession.Ghost.Ghost.IsVisible);
        }

        [AvaloniaFact]
        public void Focus_InAnotherPanel_EndsTheGrab()
        {
            var screen = Show();
            GrabFirstLayer(screen);
            var other = screen.Window.GetVisualDescendants().OfType<PanelSide>()
                .First(host => !host.IsVisualAncestorOf(screen.View))
                .GetVisualDescendants().OfType<InputElement>().First(element => element.Focusable && element.IsEffectivelyVisible);

            other.Focus();
            Settle(screen.Window);

            Assert.False(screen.View.Grab.IsGrabbing);
            Assert.False(screen.Tree.IsDragging);
        }

        [AvaloniaFact]
        public void G_InATextBox_GrabsNothing()
        {
            var screen = Show();
            screen.Tree.CursorNode = screen.LayerNode(0);
            var box = screen.Window.GetVisualDescendants().OfType<TextBoxSearchTree>().First().GetVisualDescendants().OfType<TextBox>().First();
            box.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.G);

            Assert.False(screen.View.Grab.IsGrabbing);
        }

        [AvaloniaFact]
        public void Detach_DuringTheGrab_EndsItAndGivesTheKeysBack()
        {
            var screen = Show();
            GrabFirstLayer(screen);

            screen.View.Grab.Detach();
            Press(screen, PhysicalKey.ArrowDown);

            Assert.False(screen.View.Grab.IsGrabbing);
            Assert.False(screen.Tree.IsDragging);
            // Down moves the cursor again: the grab left no handler on the window.
            Assert.Same(screen.LayerNode(1), screen.Tree.CursorNode);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheGrabCommandOnG()
        {
            var screen = Show();

            var binding = screen.Window.KeyBindings.Single(binding => Equals(binding.Gesture, screen.ViewModel.Shortcuts["Tree.Grab"]));

            Assert.Same(screen.Tree.GrabTargetCommand, binding.Command);
            Assert.Equal(new KeyGesture(Key.G), screen.ViewModel.Shortcuts["Tree.Grab"]);
        }
    }
}
