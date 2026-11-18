// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Behaviors;

namespace CMiX.Studio.Avalonia.Views
{
    // The tree of the project. A row is dragged with the pointer to reorder it in its list (see TreeRowDrag).
    // A layer can also go to another composition. TreeDropService holds the rules. Files from the file manager drop on the tree too.
    public partial class ProjectTreeView : UserControl
    {
        // The space between two rows at a screen of 100% (see TreeRowMetrics).
        public const double RowGap = TreeRowMetrics.RowGap;

        private readonly TreeRowMetrics rowMetrics;
        private readonly TreeRowIndex rowIndex;
        private readonly TreeViewContext context;
        private readonly TreeRowShadow rowShadow;
        private readonly TreeDropMarker marker;
        private readonly TreeScrollAnchor scrollAnchor;
        private readonly TreeDragSession dragSession;
        private readonly TreeRowDrag rowDrag;
        private readonly TreeGrab grab;
        private readonly TreeTooltips tooltips;
        private readonly TreeHints hints;
        private readonly TreeFileDropInput fileDrop;
        private readonly TreeRowReveal reveal;
        private readonly TreeMenus menus;
        private readonly TreeScreenRequests requests;
        private readonly TreeFocusKeeper focusKeeper;
        private readonly TreeKeyInput keyInput;

        // What this view tells the tree and does for it (see ProjectTree.AttachView).
        private readonly TreeViewHooks hooks;

        // The tooltips of the rows. A test reads it.
        internal TreeTooltips Tooltips => tooltips;

        // The node of the row that the left button pressed and that can be dragged. A drag clears it.
        private TreeNode? pressedNode;

        // The panel of the Add row of the menus. A test reads it.
        internal AddPanel MenuAddPanel => menus.AddPanel;

        // The item of each node. A test reads it.
        internal TreeRowIndex RowIndex => rowIndex;

        // The mark of the drop. A test reads it.
        internal TreeDropMarker Marker => marker;

        // The drag of a row, for every way to start it. A test reads it.
        internal TreeDragSession DragSession => dragSession;

        // The drag of a row with a pressed button. A test reads it.
        internal TreeRowDrag RowDrag => rowDrag;

        // The grab of a row with the key (Tree.Grab). A test reads it.
        internal TreeGrab Grab => grab;

        // The class of the view while a grab runs: the view shows the accent frame (GrabFrame).
        internal const string GrabbingClass = "grabbing";

        public ProjectTreeView()
        {
            InitializeComponent();

            // The bands start at the top of the rows, which is the panel of the items of the tree.
            treeView.Loaded += (_, _) => stripes.Rows = treeView.ItemsPanelRoot;

            rowIndex = new TreeRowIndex(treeView);
            context = new TreeViewContext(this, treeView, rowIndex);
            rowMetrics = new TreeRowMetrics(treeView, Resources, context);
            rowShadow = new TreeRowShadow(treeView, rowShadowLayer, rowShadowBox);
            marker = new TreeDropMarker(rowIndex);
            scrollAnchor = new TreeScrollAnchor(treeView, rowIndex, context);
            dragSession = new TreeDragSession(treeView, context, marker, scrollAnchor);
            rowDrag = new TreeRowDrag(treeView, dragSession);
            reveal = new TreeRowReveal(context, () => IsKeyboardFocusWithin);
            grab = new TreeGrab(this, treeView, dragSession, context, reveal.RevealRow, OnGrabChanged);
            focusKeeper = new TreeFocusKeeper(this, context, reveal, PostWhileAttached, () => IsAttached);
            hints = new TreeHints(context, hintStrip,
                new TreeHintInputs(() => grab.IsGrabbing, IsMenuOpen, focusKeeper.IsSearchFocused, () => IsKeyboardFocusWithin), PostWhileAttached);
            fileDrop = new TreeFileDropInput(context, marker);
            menus = new TreeMenus(context, new TreeMenuInputs(reveal.ShownItemOf, () => lastPointer, grab.TakeMenuBlock, focusKeeper.FocusRow),
                hints.Update, PostWhileAttached);
            requests = new TreeScreenRequests(context, new TreeRequestParts(menus, reveal, grab, scrollAnchor),
                new TreeRequestInputs(() => IsActiveView, () => lastPointer), PostWhileAttached);
            keyInput = new TreeKeyInput(context, rowDrag, menus, focusKeeper, PostWhileAttached);
            hooks = focusKeeper.BuildHooks(dragSession, rowDrag, grab, requests.CloseRowsInView);

            treeView.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            treeView.AddHandler(PointerPressedEvent, OnPointerPressedDone, RoutingStrategies.Bubble, handledEventsToo: true);
            treeView.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            treeView.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            treeView.AddHandler(PointerCaptureLostEvent, OnPointerCaptureLost);
            treeView.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
            fileDrop.Attach(treeView);
            menus.Attach(treeView);
            treeView.AddHandler(DoubleTappedEvent, OnDoubleTapped, RoutingStrategies.Bubble);
            treeView.AddHandler(ScrollViewer.ScrollChangedEvent, (_, _) => rowMetrics.SetEndRoom());
            reveal.Attach(treeView);
            keyInput.Attach(treeView);
            AddHandler(PointerMovedEvent, OnPointerTracked, RoutingStrategies.Tunnel, handledEventsToo: true);
            KeyBindingTextGuard.Install(this);
            tooltips = new TreeTooltips(treeView);
        }

        // The hint strip under the rows. A test reads it.
        internal KeyHintStrip HintStrip => hintStrip;

        // The mode that the hint strip shows. A test reads it.
        internal TreeHintMode HintMode => hints.Mode;

        private bool IsMenuOpen() => menus.IsMenuOpen();

        private ProjectTree? watched;

        private bool IsAttached { get; set; }

        private void PostWhileAttached(Action action, DispatcherPriority priority) =>
            Dispatcher.UIThread.Post(() =>
            {
                if (IsAttached)
                    action();
            }, priority);

        // The last place of the pointer over the tree, in the coordinates of the tree view. Tree.Add opens there when no target row is shown.
        private Point lastPointer;

        private void OnPointerTracked(object? sender, PointerEventArgs e) => lastPointer = e.GetPosition(treeView);

        // The grab starts or ends: the frame of the grab and the hint strip follow it.
        private void OnGrabChanged()
        {
            Classes.Set(GrabbingClass, grab.IsGrabbing);
            hints.Update();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (IsAttached)
                Unwatch();

            watched = DataContext as ProjectTree;
            if (IsAttached)
                Watch();
        }

        // True when this view answers the requests of the tree (see ProjectTree.IsActiveView). Both side panels can show the tree, and
        // one request must have one answer. The search box of the panel asks it too.
        internal bool IsActiveView => watched?.IsActiveView(hooks) == true;

        private void Watch()
        {
            if (watched == null)
                return;

            watched.PropertyChanged += OnWatchedPropertyChanged;
            requests.Watch(watched);
            watched.AttachView(hooks);
            hints.Watch(watched);
        }

        private void Unwatch()
        {
            if (watched == null)
                return;

            watched.PropertyChanged -= OnWatchedPropertyChanged;
            requests.Unwatch(watched);
            hints.Unwatch(watched);
            watched.DetachView(hooks);
        }

        private void OnWatchedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProjectTree.IsSearchActive))
                hints.Update();

            if (e.PropertyName != nameof(ProjectTree.SelectedNode) || dragSession.IsDropping || watched?.SelectedNode is not { } node
                || watched.Origin is not (SelectionOrigin.Program or SelectionOrigin.Peer))
                return;

            PostWhileAttached(() => reveal.RevealRow(node), DispatcherPriority.Loaded);
        }

        // See TreeRowMetrics.SnapToPixels.
        public static RowSizes SnapToPixels(double height, double spacing, double scale) =>
            TreeRowMetrics.SnapToPixels(height, spacing, scale);

        private TopLevel? scaledTopLevel;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // The tree view model listens only while the view is attached.
            IsAttached = true;
            Watch();
            focusKeeper.Attach();

            scaledTopLevel = TopLevel.GetTopLevel(this);
            if (scaledTopLevel == null)
                return;

            hints.AttachWindow(scaledTopLevel);
            rowMetrics.SetRowMetrics(scaledTopLevel.RenderScaling);
            scaledTopLevel.ScalingChanged += OnScalingChanged;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            Unwatch();
            focusKeeper.Detach();

            IsAttached = false;
            if (scaledTopLevel != null)
            {
                scaledTopLevel.ScalingChanged -= OnScalingChanged;
                hints.DetachWindow(scaledTopLevel);
            }

            scaledTopLevel = null;
            rowDrag.Cancel();
            grab.Detach();
            context.DisposeAutoScroller();

            // Ends the reveal and the scroll hold. Their next step and their next frame then do nothing, so no frame request outlives the view.
            reveal.EndReveal();
            scrollAnchor.EndHold();
            base.OnDetachedFromVisualTree(e);
        }

        private void OnScalingChanged(object? sender, EventArgs e) => rowMetrics.SetRowMetrics(scaledTopLevel?.RenderScaling ?? 1);

        // A double tap on a row, away from its name, expands or collapses the row. The name takes its own double tap (it starts
        // the rename and handles the tap), so that tap never comes here. A toggle or an input keeps its double tap too.
        // TreeViewItem does this by itself for the part PART_HeaderPresenter. The template has no such part (see ProjectTree.axaml),
        // so the tree does it here, and only where it is wanted.
        private void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (IsEditor(e.Source) || TreeRows.ItemOf(e.Source)?.DataContext is not TreeNode { Children.Count: > 0 } node)
                return;

            node.IsExpanded = !node.IsExpanded;
            e.Handled = true;
        }

        private void OnPointerPressedDone(object? sender, PointerPressedEventArgs e) => focusKeeper.EndPress();

        // A ToggleButton of a row that cannot take the focus: a toggle or the chevron.
        private static bool IsRowButton(object? source) =>
            (source as Visual)?.FindAncestorOfType<ToggleButton>(true) is { Focusable: false } button && TreeRows.ItemOf(button) != null;

        private static bool IsInToggles(object? source) =>
            (source as Visual)?.FindAncestorOfType<RowToggles>(true) != null;

        private static bool IsEditor(object? source)
        {
            for (var visual = source as Visual; visual != null; visual = visual.GetVisualParent())
                if (visual is ToggleButton or TextBox)
                    return true;

            return false;
        }

        // A left click on a row, away from its buttons and its name box: the user chooses the node. It comes on the release, when no drag
        // started and the pointer is still on the row. It does not come when a node is selected by a program, a list or a peer.
        // The main window uses it to show the inspector.
        public static readonly RoutedEvent<NodeChosenEventArgs> NodeChosenEvent =
            RoutedEvent.Register<ProjectTreeView, NodeChosenEventArgs>(nameof(NodeChosen), RoutingStrategies.Bubble);

        // A routed event: it goes up to the window, whichever panel the tree is in.
        public event EventHandler<NodeChosenEventArgs> NodeChosen
        {
            add => AddHandler(NodeChosenEvent, value);
            remove => RemoveHandler(NodeChosenEvent, value);
        }

        // The node of the row that the left button pressed. A drag clears it.
        private TreeNode? chosenNode;

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedNode = null;
            chosenNode = null;
            focusKeeper.BeginPress();
            // A middle click on a row selects it and sends it to the inspector, and starts no drag. The chevron, the toggles
            // and the name box take no middle click.
            if (e.GetCurrentPoint(treeView).Properties.IsMiddleButtonPressed)
            {
                if (!rowDrag.IsDragging && !IsEditor(e.Source) && TreeRows.ItemOf(e.Source)?.DataContext is TreeNode middle && DataContext is ProjectTree inspecting)
                {
                    inspecting.SelectByClick(middle);
                    inspecting.Inspect(middle);
                    e.Handled = true;
                }
                return;
            }

            if (rowDrag.IsDragging || !e.GetCurrentPoint(treeView).Properties.IsLeftButtonPressed)
                return;

            if (IsEditor(e.Source))
            {
                if (IsRowButton(e.Source))
                    focusKeeper.RememberFocusBeforeRowButton();
                return;
            }

            var node = TreeRows.ItemOf(e.Source)?.DataContext as TreeNode;
            // A click beside the rows gives the tree the focus on its cursor row, so the keys work right away. A tree without a cursor
            // gets one first: the selected row, else the first row. The focus moves after the press is done (OnPointerPressedDone): the
            // default handling of the press would take it back.
            if (node is null && DataContext is ProjectTree beside)
            {
                beside.CursorNode ??= beside.SelectedNode ?? beside.Roots.FirstOrDefault();
                if (beside.CursorNode != null)
                    focusKeeper.FocusCursorRowAfterPress();
            }

            // The tree view toggles the selection on a Ctrl click. The tree has one selection, so a Ctrl click on a row does nothing.
            if (node != null && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                e.Handled = true;
                return;
            }

            // The chevron and the toggles are ToggleButtons. IsEditor stops a click on them above, so it does not move the cursor.
            if (node != null && DataContext is ProjectTree clicked)
                clicked.SelectByClick(node);

            // The toggles at the right of a row never start a drag, not even the space around them.
            pressedNode = node != null && !IsInToggles(e.Source) && DataContext is ProjectTree tree && tree.CanDrag(node) ? node : null;
            chosenNode = node;
            rowDrag.Press(pressedNode, e.GetPosition(treeView));
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(treeView).Properties.IsLeftButtonPressed)
                return;

            rowDrag.Move(e.GetPosition(treeView), e);
            if (rowDrag.IsDragging)
            {
                pressedNode = null;
                chosenNode = null;
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var node = chosenNode;
            pressedNode = null;
            chosenNode = null;
            rowDrag.Release(e.GetPosition(treeView));
            if (node == null || e.InitialPressMouseButton != MouseButton.Left || !IsOverRow(node, e))
                return;

            RaiseEvent(new NodeChosenEventArgs(NodeChosenEvent, this, node));
        }

        // The capture goes when the window loses the pointer or the tree leaves the window: the drag ends without a drop.
        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => rowDrag.Cancel();

        // The wheel during a drag of a row scrolls the tree by rows (see DragAutoScroller). The scroll viewer does not see it.
        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (!rowDrag.IsDragging)
                return;

            var steps = (int)Math.Round(e.Delta.Y);
            rowDrag.Wheel(steps != 0 ? steps : Math.Sign(e.Delta.Y));
            e.Handled = true;
        }

        // True when the pointer is on the row border of the node.
        private bool IsOverRow(TreeNode node, PointerEventArgs e) =>
            rowIndex.FindItem(node) is { } item
            && TreeRows.RowBorderOf(item) is { } border
            && new Rect(border.Bounds.Size).Contains(e.GetPosition(border));

        // True while the scroll of a drag runs.
        public bool IsAutoScrolling => context.ExistingAutoScroller?.IsRunning ?? false;

        public void ScrollByWheel(int steps) => context.ExistingAutoScroller?.ScrollByWheel(steps);

        public double AutoScrollStep() => context.ExistingAutoScroller?.AutoScrollStep() ?? 0;
    }
}
