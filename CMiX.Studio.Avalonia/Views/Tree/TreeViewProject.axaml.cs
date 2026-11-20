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
    public partial class TreeViewProject : UserControl
    {
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

        private readonly TreeViewHooks hooks;

        private readonly TreeViewLifetime lifetime = new();

        // For the tests.
        internal TreeTooltips Tooltips => tooltips;

        internal AddPanel MenuAddPanel => menus.AddPanel;

        internal TreeRowIndex RowIndex => rowIndex;

        internal TreeDropMarker Marker => marker;

        internal TreeDragSession DragSession => dragSession;

        internal TreeRowDrag RowDrag => rowDrag;

        internal TreeGrab Grab => grab;

        // While a grab runs, the view shows the accent frame (grabFrame).
        internal const string GrabbingClass = "grabbing";

        public TreeViewProject()
        {
            InitializeComponent();

            // The bands start at the top of the rows.
            treeView.Loaded += (_, _) => stripes.Rows = treeView.ItemsPanelRoot;

            rowIndex = new TreeRowIndex(treeView);
            context = new TreeViewContext(this, treeView, rowIndex);
            rowMetrics = new TreeRowMetrics(Resources, context);
            rowShadow = new TreeRowShadow(treeView, rowShadowLayer, rowShadowBox);
            marker = new TreeDropMarker(rowIndex);
            scrollAnchor = new TreeScrollAnchor(context);
            dragSession = new TreeDragSession(context, marker, scrollAnchor);
            rowDrag = new TreeRowDrag(treeView, dragSession);
            reveal = new TreeRowReveal(context);
            grab = new TreeGrab(this, dragSession, context, reveal.RevealRow, OnGrabChanged);
            focusKeeper = new TreeFocusKeeper(this, context, reveal, lifetime);
            pointerInput = new TreePointerInput(this, context, rowDrag, focusKeeper);
            // The menus come later, so the lambda reads the field at the call.
            hints = new TreeHints(context, hintStrip, grab, focusKeeper, () => menus.IsMenuOpen(), lifetime);
            fileDrop = new TreeFileDropInput(context, marker);
            menus = new TreeMenus(context, reveal, pointerInput, grab, focusKeeper, hints.Update, lifetime);
            requests = new TreeScreenRequests(context, menus, reveal, grab, scrollAnchor, pointerInput, () => IsActiveView, lifetime);
            keyInput = new TreeKeyInput(context, rowDrag, menus, focusKeeper, lifetime);
            hooks = BuildHooks();

            pointerInput.Attach();
            fileDrop.Attach();
            menus.Attach();
            treeView.AddHandler(ScrollViewer.ScrollChangedEvent, (_, _) => rowMetrics.SetEndRoom());
            reveal.Attach();
            keyInput.Attach();
            KeyBindingTextGuard.Install(this);
            tooltips = new TreeTooltips(treeView);
        }

        private TreeViewHooks BuildHooks() =>
            new()
            {
                IsPointerOverTree = () => IsPointerOver,
                IsTreeFocused = () => IsKeyboardFocusWithin,
                IsTextBoxFocused = focusKeeper.IsTextBoxFocused,
                IsFocusInOtherPanel = focusKeeper.IsFocusInOtherPanel,
                IsPanelFocused = focusKeeper.IsPanelFocused,
                ToggleDragRow = dragSession.ToggleHovered,
                CancelDrag = () =>
                {
                    rowDrag.Cancel();
                    grab.Cancel();
                },
                CloseRowsInView = requests.CloseRowsInView
            };

        internal KeyHintStrip HintStrip => hintStrip;

        internal TreeHintMode HintMode => hints.Mode;

        private ProjectTree? watched;

        private void OnGrabChanged()
        {
            Classes.Set(GrabbingClass, grab.IsGrabbing);
            hints.Update();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (lifetime.IsAttached)
                Unwatch();

            watched = DataContext as ProjectTree;
            if (lifetime.IsAttached)
                Watch();
        }

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

            lifetime.Post(() => reveal.RevealRow(node), DispatcherPriority.Loaded);
        }

        public static RowSizes SnapToPixels(double height, double spacing, double scale) =>
            TreeRowMetrics.SnapToPixels(height, spacing, scale);

        private TopLevel? scaledTopLevel;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // The tree view model listens only while the view is attached.
            lifetime.Attach();
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

            lifetime.Detach();
            if (scaledTopLevel != null)
            {
                scaledTopLevel.ScalingChanged -= OnScalingChanged;
                hints.DetachWindow(scaledTopLevel);
            }

            scaledTopLevel = null;
            rowDrag.Cancel();
            grab.Detach();
            context.DisposeAutoScroller();

            // So no frame request outlives the view.
            reveal.EndReveal();
            scrollAnchor.EndHold();
            base.OnDetachedFromVisualTree(e);
        }

        private void OnScalingChanged(object? sender, EventArgs e) => rowMetrics.SetRowMetrics(scaledTopLevel?.RenderScaling ?? 1);

        // A left click on a row chooses the node, on the release when no drag started.
        // A selection by a program, a list or a peer raises no NodeChosen.
        public static readonly RoutedEvent<NodeChosenEventArgs> NodeChosenEvent =
            RoutedEvent.Register<TreeViewProject, NodeChosenEventArgs>(nameof(NodeChosen), RoutingStrategies.Bubble);

        public event EventHandler<NodeChosenEventArgs> NodeChosen
        {
            add => AddHandler(NodeChosenEvent, value);
            remove => RemoveHandler(NodeChosenEvent, value);
        }

        public bool IsAutoScrolling => context.ExistingAutoScroller?.IsRunning ?? false;

        public void ScrollByWheel(int steps) => context.ExistingAutoScroller?.ScrollByWheel(steps);

        public double AutoScrollStep() => context.ExistingAutoScroller?.AutoScrollStep() ?? 0;
    }
}
