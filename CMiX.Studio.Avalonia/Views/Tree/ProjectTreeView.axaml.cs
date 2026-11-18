// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Parts;

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
        private readonly TreePointerInput pointerInput;

        // What this view tells the tree and does for it (see ProjectTree.AttachView).
        private readonly TreeViewHooks hooks;

        // The tooltips of the rows. A test reads it.
        internal TreeTooltips Tooltips => tooltips;

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
            pointerInput = new TreePointerInput(this, context, rowDrag, focusKeeper);
            hints = new TreeHints(context, hintStrip,
                new TreeHintInputs(() => grab.IsGrabbing, IsMenuOpen, focusKeeper.IsSearchFocused, () => IsKeyboardFocusWithin), PostWhileAttached);
            fileDrop = new TreeFileDropInput(context, marker);
            menus = new TreeMenus(context, new TreeMenuInputs(reveal.ShownItemOf, () => pointerInput.LastPointer, grab.TakeMenuBlock, focusKeeper.FocusRow),
                hints.Update, PostWhileAttached);
            requests = new TreeScreenRequests(context, new TreeRequestParts(menus, reveal, grab, scrollAnchor),
                new TreeRequestInputs(() => IsActiveView, () => pointerInput.LastPointer), PostWhileAttached);
            keyInput = new TreeKeyInput(context, rowDrag, menus, focusKeeper, PostWhileAttached);
            hooks = focusKeeper.BuildHooks(dragSession, rowDrag, grab, requests.CloseRowsInView);

            pointerInput.Attach();
            fileDrop.Attach(treeView);
            menus.Attach(treeView);
            treeView.AddHandler(ScrollViewer.ScrollChangedEvent, (_, _) => rowMetrics.SetEndRoom());
            reveal.Attach(treeView);
            keyInput.Attach(treeView);
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

        // True while the scroll of a drag runs.
        public bool IsAutoScrolling => context.ExistingAutoScroller?.IsRunning ?? false;

        public void ScrollByWheel(int steps) => context.ExistingAutoScroller?.ScrollByWheel(steps);

        public double AutoScrollStep() => context.ExistingAutoScroller?.AutoScrollStep() ?? 0;
    }
}
