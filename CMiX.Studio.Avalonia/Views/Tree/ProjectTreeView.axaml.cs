// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Controls.String;
using CMiX.UI.Panels;
using CMiX.UI.Popups;
using CMiX.UI.Behaviors;
using CompositionModel = CMiX.Core.Compositing.Composition;
using LayerModel = CMiX.Core.Compositing.Layer;

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

        // What this view tells the tree and does for it (see ProjectTree.AttachView).
        private readonly TreeViewHooks hooks;

        // The tooltips of the rows. A test reads it.
        internal TreeTooltips Tooltips => tooltips;

        // The panel of the Add row of the menus of the rows. One panel serves every menu.
        private readonly AddPanel menuAddPanel = new();

        // The node of the row that the left button pressed and that can be dragged. A drag clears it.
        private TreeNode? pressedNode;

        // The panel of the Add row of the menus. A test reads it.
        internal AddPanel MenuAddPanel => menuAddPanel;

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
            hints = new TreeHints(context, hintStrip,
                new TreeHintInputs(() => grab.IsGrabbing, IsMenuOpen, IsSearchFocused, () => IsKeyboardFocusWithin), PostWhileAttached);
            fileDrop = new TreeFileDropInput(context, marker);
            hooks = new TreeViewHooks
            {
                IsPointerOverTree = () => IsPointerOver,
                IsTreeFocused = () => IsKeyboardFocusWithin,
                IsTextBoxFocused = () => IsInTextBox(TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement()),
                IsFocusInOtherPanel = IsFocusInOtherPanel,
                IsPanelFocused = IsPanelFocused,
                ToggleDragRow = dragSession.ToggleHovered,
                CancelDrag = () =>
                {
                    rowDrag.Cancel();
                    grab.Cancel();
                },
                CloseRowsInView = OnCloseRowsInView
            };

            treeView.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            treeView.AddHandler(PointerPressedEvent, OnPointerPressedDone, RoutingStrategies.Bubble, handledEventsToo: true);
            treeView.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            treeView.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            treeView.AddHandler(PointerCaptureLostEvent, OnPointerCaptureLost);
            treeView.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
            fileDrop.Attach(treeView);
            treeView.AddHandler(ContextRequestedEvent, OnContextRequested, RoutingStrategies.Bubble);
            treeView.AddHandler(DoubleTappedEvent, OnDoubleTapped, RoutingStrategies.Bubble);
            treeView.AddHandler(ScrollViewer.ScrollChangedEvent, (_, _) => rowMetrics.SetEndRoom());
            reveal.Attach(treeView);
            treeView.AddHandler(KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, OnPointerTracked, RoutingStrategies.Tunnel, handledEventsToo: true);
            KeyBindingTextGuard.Install(this);
            AddHandler(GotFocusEvent, (_, _) => focusIsInTree = true, RoutingStrategies.Bubble, handledEventsToo: true);
            AddHandler(LostFocusEvent, OnLostFocusInTree, RoutingStrategies.Bubble, handledEventsToo: true);
            // A move makes the item of the row again, and the old item takes the focus away with it, with no event that reaches the
            // tree. The layout that follows shows that no control has the focus.
            treeView.LayoutUpdated += (_, _) => CheckTheLostFocus();
            tooltips = new TreeTooltips(treeView);
            menuAddPanel.Closed += (_, _) =>
            {
                hints.Update();
                // A panel opened beside the cursor row (Right, Ctrl+Right) is a leaf of the tree: when it closes, Left or Escape or a pick,
                // the cursor row takes the focus back, so the keys go on in the tree.
                if (addOpenedBesideTheCursor)
                {
                    addOpenedBesideTheCursor = false;
                    if (watched?.CursorNode is { } cursor)
                        PostWhileAttached(() => FocusRow(cursor), DispatcherPriority.Input);
                }
            };
        }

        // The hint strip under the rows. A test reads it.
        internal KeyHintStrip HintStrip => hintStrip;

        // The mode that the hint strip shows. A test reads it.
        internal TreeHintMode HintMode => hints.Mode;

        private bool IsMenuOpen() => treeView.ContextMenu?.IsOpen == true || menuAddPanel.IsOpen;

        // True when the focus is in the search field of the tree of this view, in the panel of this view.
        private bool IsSearchFocused()
        {
            if (FocusedControl() is not Visual focused || focused.FindAncestorOfType<TreeSearchBox>(true) is not { } box
                || !ReferenceEquals(box.DataContext, watched))
                return false;

            return this.FindAncestorOfType<SidePanelHost>() is not { } host || host.IsVisualAncestorOf(box);
        }

        // True while a check of the focus waits to run.
        private bool focusCheckPosted;

        private void CheckTheLostFocus()
        {
            if (!focusIsInTree || focusCheckPosted || TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() != null)
                return;

            focusCheckPosted = true;
            Dispatcher.UIThread.Post(() =>
            {
                focusCheckPosted = false;
                if (IsAttached)
                    GiveTheFocusBack(null);
            }, DispatcherPriority.Input);
        }

        // True while the keyboard focus is in the tree, and also when it went from the tree to no control: a name box that hides at the
        // end of a rename, or a row that a move makes again. The focus then goes back to the row (see GiveTheFocusBack).
        private bool focusIsInTree;

        // A name box loses the focus when a rename ends. The next focus is known only after the event.
        private void OnLostFocusInTree(object? sender, RoutedEventArgs e)
        {
            var node = TreeRows.ItemOf(e.Source)?.DataContext as TreeNode;
            PostWhileAttached(() => GiveTheFocusBack(node), DispatcherPriority.Input);
        }

        // When no control has the focus and the focus was in the tree, the row of the node gets it, else the selected row. A control
        // that the user clicked keeps the focus.
        private void GiveTheFocusBack(TreeNode? node)
        {
            var focused = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            if (focused != null)
            {
                focusIsInTree = focused is Visual visual && (ReferenceEquals(visual, this) || this.IsVisualAncestorOf(visual));
                return;
            }

            if (!focusIsInTree || (node ?? watched?.SelectedNode) is not { } target || rowIndex.FindItem(target) is not { } item)
                return;

            item.Focus(NavigationMethod.Tab);
        }

        private ProjectTree? watched;

        private bool IsAttached { get; set; }

        private void PostWhileAttached(Action action, DispatcherPriority priority) =>
            Dispatcher.UIThread.Post(() =>
            {
                if (IsAttached)
                    action();
            }, priority);

        // Escape ends a drag of a row without a drop. Else, while a search is active, Escape clears it (see ProjectTree.ClearSearch): the
        // focus and the cursor row stay. A grab, a menu and a name box take their Escape first. With no search, Escape does nothing here.
        // Avalonia runs the key bindings of the view (the gestures of the map) before this handler. The navigation keys move the cursor
        // only (see ProjectTree.CursorNode). The tree view does not see them, so they select nothing. A navigation key
        // with a modifier does nothing. A key in a name box stays in the name box. The keys of IsTreeViewKey do nothing.
        private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && rowDrag.IsDragging)
            {
                rowDrag.Cancel();
                e.Handled = true;
                return;
            }

            if (watched is not { } tree || IsInTextBox(e.Source))
                return;

            if (e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None && tree.IsSearchActive && !tree.IsDragging)
            {
                tree.ClearSearchText();
                e.Handled = true;
                return;
            }

            // Tree.Inspect (Space by default) is handled here, before the tree view takes Space for its own selection.
            if (tree.Shortcuts["Tree.Inspect"] is { } inspectGesture && inspectGesture.Matches(e))
            {
                if (tree.InspectCursorCommand.CanExecute(null))
                    tree.InspectCursorCommand.Execute(null);
                e.Handled = true;
                return;
            }

            // The tree view opens and closes rows on these keys by itself. The design does not use them.
            if (IsTreeViewKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            // The Add panel is the leaf after the last row of a branch: Right on a row with nothing under it opens it, at the right of the row,
            // its search field level with the row and a tip at the middle of the row. Ctrl+Right opens it on any row. The arrows then pick
            // an entry and Enter adds it, so a row deep in the tree is reached and filled with Right alone. Left in the panel comes back.
            if (e.Key == Key.Right && (e.KeyModifiers == KeyModifiers.Control
                                       || (e.KeyModifiers == KeyModifiers.None && tree.CursorNode is { Children.Count: 0 })))
            {
                OpenAddBesideTheCursor(tree);
                e.Handled = true;
                return;
            }

            if (!IsNavigationKey(e.Key))
                return;

            e.Handled = true;
            if (e.KeyModifiers != KeyModifiers.None)
                return;

            MoveCursor(tree, e.Key);
            if (tree.CursorNode is { } node)
                PostWhileAttached(() => FocusRow(node), DispatcherPriority.Loaded);
        }

        // True while the Add panel that opened beside the cursor row is open (see OpenAddBesideTheCursor).
        private bool addOpenedBesideTheCursor;

        private void OpenAddBesideTheCursor(ProjectTree tree)
        {
            if (tree.CursorNode is not { } node || reveal.ShownItemOf(node) is not { } item || TopLevel.GetTopLevel(this) is not { } window)
                return;

            var row = TreeRows.RowBorderOf(item);
            var anchor = row?.TranslatePoint(default, window) is { } topLeft ? new Rect(topLeft, row.Bounds.Size) : (Rect?)null;
            menuAddPanel.OpenWithTip(item, PopupSide.Right, tree.GetAddGroups(node), AddTitle, anchor);
            addOpenedBesideTheCursor = menuAddPanel.IsOpen;
            hints.Update();
        }

        private static bool IsNavigationKey(Key key) =>
            key is Key.Up or Key.Down or Key.PageUp or Key.PageDown or Key.Home or Key.End or Key.Left or Key.Right;

        // The numpad keys that TreeViewItem uses to open and close rows. Enter and the navigation keys are not in this list.
        internal static bool IsTreeViewKey(Key key) =>
            key is Key.Add or Key.Subtract or Key.Multiply or Key.Divide;

        private void MoveCursor(ProjectTree tree, Key key)
        {
            switch (key)
            {
                case Key.Up: tree.MoveCursor(-1); break;
                case Key.Down: tree.MoveCursor(1); break;
                case Key.PageUp: tree.MoveCursor(-RowsInView()); break;
                case Key.PageDown: tree.MoveCursor(RowsInView()); break;
                case Key.Home: tree.MoveCursorToFirst(); break;
                case Key.End: tree.MoveCursorToLast(); break;
                case Key.Left: tree.MoveCursorOut(); break;
                case Key.Right: tree.MoveCursorIn(); break;
            }
        }

        private int RowsInView() =>
            context.Scroller is { } scroller && rowMetrics.RowPitch > 0 ? Math.Max(1, (int)(scroller.Viewport.Height / rowMetrics.RowPitch)) : 1;

        // Moves the keyboard focus to the row of the node and brings the row into view. The focus shows the focus look of the row
        // (:focus-visible, see ProjectTree.axaml). It does not select the row.
        private void FocusRow(TreeNode node)
        {
            if (rowIndex.FindItem(node) is not { } item)
                return;

            item.Focus(NavigationMethod.Tab);
            reveal.RevealRow(node);
        }

        // The last place of the pointer over the tree, in the coordinates of the tree view. Tree.Add opens there when no target row is shown.
        private Point lastPointer;

        private void OnPointerTracked(object? sender, PointerEventArgs e) => lastPointer = e.GetPosition(treeView);

        // Tree.Add, a key: the Add panel of the target row (ProjectTree.TargetNode) opens with the middle of its search field on the middle
        // of that row, and the title Add. With no target, or a target row that is not shown, it opens under the pointer. With no target it
        // holds what the project takes, a composition. Nothing is added until the user picks an entry of the panel.
        private void OnAddRequested(object? sender, EventArgs e)
        {
            if (!IsActiveView || DataContext is not ProjectTree tree)
                return;

            var point = tree.TargetNode is { } node && reveal.ShownItemOf(node) is { } item ? TreeRowAnchor.CenterOf(item, treeView) : null;
            menuAddPanel.OpenAtPoint(treeView, point ?? lastPointer, tree.GetAddGroups(tree.TargetNode), AddTitle);
            hints.Update();
        }

        // Tree.Menu, a key: the menu of the target row opens under that row (see OpenMenuUnderRow). With no target, it is the menu of the project,
        // at the pointer. A target row that is not shown (its parent is collapsed) opens no menu.
        private void OnMenuRequested(object? sender, EventArgs e)
        {
            if (!IsActiveView || DataContext is not ProjectTree tree)
                return;

            if (tree.TargetNode is not { } node)
                OpenMenuAtPointer(null);
            else if (reveal.ShownItemOf(node) is { } item)
                OpenMenuUnderRow(item);
        }

        // Tree.Grab: the grab of the target row starts at the last place of the pointer over the tree (see TreeGrab). A row that is
        // not shown (its parent is collapsed) is not grabbed.
        private void OnGrabRequested(object? sender, TreeNode node)
        {
            if (IsActiveView && reveal.ShownItemOf(node) != null)
                grab.Start(node, lastPointer);
        }

        // The grab starts or ends: the frame of the grab and the hint strip follow it.
        private void OnGrabChanged()
        {
            Classes.Set(GrabbingClass, grab.IsGrabbing);
            hints.Update();
        }

        // Tree.CollapseOthers: every row closes but the path to the target row. A target row in view stays at its place on the screen,
        // also while the rows close (CollapsePanel closes over a few frames). The first row in view can not be the anchor: it can close
        // away under a parent. A target row out of view is brought into view after the close.
        private void OnCollapseOthersRequested(object? sender, EventArgs e)
        {
            if (!IsActiveView || DataContext is not ProjectTree tree || tree.TargetNode is not { } target)
                return;

            var anchor = scrollAnchor.CaptureRow(target);
            var closed = tree.CloseOtherRows();
            if (anchor != null)
            {
                scrollAnchor.RestoreAnchor(anchor);
                scrollAnchor.HoldWhileClosing(anchor, closed);
                return;
            }

            PostWhileAttached(() => reveal.RevealRow(target), DispatcherPriority.Loaded);
        }

        private void OnRevealRequested(object? sender, TreeNode node)
        {
            if (IsActiveView)
                reveal.RevealRowToMiddle(node);
        }

        // Rows of the search close (see ProjectTree.CloseRowsInView). The row of the node stays at its place on the screen when it is in
        // view, else the first row in view does, also while the rows close.
        private void OnCloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close)
        {
            var anchor = (node != null ? scrollAnchor.CaptureRow(node) : null) ?? scrollAnchor.CaptureAnchor(null);
            var closed = close();
            scrollAnchor.RestoreAnchor(anchor);
            scrollAnchor.HoldWhileClosing(anchor, closed);
        }

        // The title of the Add panel when no row opens it.
        private const string AddTitle = "Add";

        // Starts the rename of the row of the node: the name box of the row goes to edit mode.
        private void OnRenameRequested(object? sender, TreeNode node)
        {
            if (!IsActiveView || rowIndex.FindItem(node) is not { } item || TreeRows.RowBorderOf(item) is not { } row)
                return;

            if (row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault() is { } box)
                box.IsEditing = true;
        }

        private static bool IsInTextBox(object? source)
        {
            for (var visual = source as Visual; visual != null; visual = visual.GetVisualParent())
                if (visual is TextBox)
                    return true;

            return false;
        }

        // True when a control outside the panel of this view has the focus (see ProjectTree.IsFocusInOtherPanel). The panel is the
        // side panel that shows the view, with its bar. The focus on the window itself, or on no control, is in no panel.
        private bool IsFocusInOtherPanel() => FocusedControl() is { } focused && !IsInPanel(focused);

        // True when the focused control is in the panel of this view (see TreeViewHooks.IsPanelFocused).
        private bool IsPanelFocused() => FocusedControl() is { } focused && IsInPanel(focused);

        // The focused control. Null when the window itself or no control has the focus.
        private IInputElement? FocusedControl() =>
            TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() is { } focused and not TopLevel ? focused : null;

        private bool IsInPanel(IInputElement focused)
        {
            Visual panel = this.FindAncestorOfType<SidePanelHost>() is { } host ? host : this;
            return focused is Visual visual && (ReferenceEquals(visual, panel) || panel.IsVisualAncestorOf(visual));
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
            watched.RenameRequested += OnRenameRequested;
            watched.AddRequested += OnAddRequested;
            watched.MenuRequested += OnMenuRequested;
            watched.CollapseOthersRequested += OnCollapseOthersRequested;
            watched.RevealRequested += OnRevealRequested;
            watched.GrabRequested += OnGrabRequested;
            watched.TreeFocusRequested += OnTreeFocusRequested;
            watched.AttachView(hooks);
            hints.Watch(watched);
        }

        private void Unwatch()
        {
            if (watched == null)
                return;

            watched.PropertyChanged -= OnWatchedPropertyChanged;
            watched.RenameRequested -= OnRenameRequested;
            watched.AddRequested -= OnAddRequested;
            watched.MenuRequested -= OnMenuRequested;
            watched.CollapseOthersRequested -= OnCollapseOthersRequested;
            watched.RevealRequested -= OnRevealRequested;
            watched.GrabRequested -= OnGrabRequested;
            watched.TreeFocusRequested -= OnTreeFocusRequested;
            hints.Unwatch(watched);
            watched.DetachView(hooks);
        }

        // The focus goes to a row: the target row, else the first row (see ProjectTree.FocusTree).
        private void OnTreeFocusRequested(object? sender, EventArgs e)
        {
            if (!IsActiveView || watched is not { } tree)
                return;

            var node = tree.TargetNode ?? tree.VisibleNodes().FirstOrDefault();
            if (node != null && rowIndex.FindItem(node) is { } item && item.Focus(NavigationMethod.Tab))
                return;

            treeView.Focus();
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

        // The menu of the row under the pointer, built and opened when it is asked for. A menu that is set during the event
        // does not open on that event, so it is opened here.
        // The tree listens in the bubble phase, as the menu of a row of a list does. A name that is being edited
        // blocks the request in the tunnel phase (ContextMenuGuard), so a right click during a rename opens no menu.
        // The menu belongs to the tree and no row holds one. A row with a menu would open it for the right clicks on its
        // children too, because a request goes up through every parent row.
        // A right click has a place of the pointer: the menu opens at the pointer, and the cursor goes to the clicked row, also when the
        // row is already selected. The Menu key has no place: the menu of the focused row, else of the target row, opens
        // under that row.
        private void OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            // A click of the grab asks for no menu: the grab takes it (see TreeGrab).
            if (grab.TakeMenuBlock())
            {
                e.Handled = true;
                return;
            }

            if (e.TryGetPosition(treeView, out _))
            {
                if (TreeRows.ItemOf(e.Source)?.DataContext is TreeNode node && DataContext is ProjectTree tree)
                    tree.CursorNode = node;

                if (OpenMenuAtPointer(e.Source))
                    e.Handled = true;
                return;
            }

            var item = TreeRows.ItemOf(e.Source) ?? (watched?.TargetNode is { } target ? reveal.ShownItemOf(target) : null);
            if (OpenMenuUnderRow(item))
                e.Handled = true;
        }

        // On the space under the rows, the menu is the menu of the project, which has the one entry that the project takes, a composition.
        // Both menus add through the Add panel. Returns false when there is no menu.
        private bool OpenMenuAtPointer(object? source)
        {
            if (PrepareMenu(TreeRows.ItemOf(source)) is not { } menu)
                return false;

            menu.Open(treeView);
            return true;
        }

        // A source that is not a row opens the menu of the project at the pointer.
        private bool OpenMenuUnderRow(object? source)
        {
            var item = TreeRows.ItemOf(source);
            if (PrepareMenu(item) is not { } menu)
                return false;

            if (item != null && TreeRowAnchor.FromNameOf(item, treeView) is { } name)
            {
                // The panel goes under the row, with its left edge on the left edge of the name. The popup is larger than the panel by
                // the margin of the shadow.
                menu.PlacementTarget = treeView;
                menu.Placement = PlacementMode.BottomEdgeAlignedLeft;
                menu.PlacementRect = name;
                menu.HorizontalOffset = -PopupFrame.ShadowMargin.Left;
                menu.VerticalOffset = -PopupFrame.ShadowMargin.Top;
            }

            menu.Open(treeView);
            return true;
        }

        private ContextMenu? PrepareMenu(TreeViewItem? item)
        {
            treeView.ContextMenu?.Close();

            var menu = item?.DataContext is TreeNode node && DataContext is ProjectTree tree ? CreateMenu(item, node, tree)
                : item == null && DataContext is ProjectTree project ? CreateSpaceMenu(project)
                : null;
            treeView.ContextMenu = menu;
            if (menu == null)
                return null;

            menu.Opened += (_, _) => hints.Update();
            menu.Closed += (_, _) => hints.Update();
            return menu;
        }

        // The menu of the empty space of the tree: its Add row opens the panel of the project.
        private ContextMenu CreateSpaceMenu(ProjectTree tree) =>
            PrefabRowMenu.Create(treeView, RowMenuKind.Group, () => null, Array.Empty<RowMenuAdd>(), () => "Project",
                addRow: new AddMenuRow(menuAddPanel, () => tree.GetAddGroups(null)),
                gestureOf: command => tree.Shortcuts[command]);

        // The menu of a row. Its Add row opens the Add panel beside the menu, with what the row takes. The menu opens with the Add row
        // under the pointer, on every row, until the user picks another row of a menu: then that row is under the pointer (see MenuAnchor).
        private ContextMenu? CreateMenu(TreeViewItem row, TreeNode node, ProjectTree tree)
        {
            var kind = node.Item == null ? RowMenuKind.Group
                : node.Item is CompositionModel or LayerModel ? RowMenuKind.Composable
                : RowMenuKind.Slot;


            return PrefabRowMenu.Create(
                row,
                kind,
                () => node.Owner != null && node.Item != null ? (node.Owner, node.Item) : null,
                Array.Empty<RowMenuAdd>(),
                () => node.Header,
                addRow: new AddMenuRow(menuAddPanel, () => tree.GetAddGroups(node)),
                gestureOf: command => tree.Shortcuts[command]);
        }

        // The focused control before a press on a button of a row. Null when the press was not on such a button.
        private sealed record FocusBefore(IInputElement? Control);

        private FocusBefore? focusBeforeRowButton;

        // A toggle or the chevron of a row cannot take the focus, so a click on it focuses its row. The click acts on that row only:
        // a focused control of the panel of the tree keeps the focus. Else the focus goes to the cursor row. The cursor and the
        // selection do not move.
        private void OnPointerPressedDone(object? sender, PointerPressedEventArgs e)
        {
            if (focusBesideRows)
            {
                focusBesideRows = false;
                if (watched?.CursorNode is { } cursor)
                    PostWhileAttached(() => FocusRow(cursor), DispatcherPriority.Input);
                return;
            }

            if (focusBeforeRowButton is not { } before)
                return;

            focusBeforeRowButton = null;
            if (ReferenceEquals(FocusedControl(), before.Control))
                return;

            if (before.Control is InputElement { IsEffectivelyVisible: true } control && IsInPanel(control) && control.GetVisualRoot() != null)
                control.Focus(control is TreeViewItem ? NavigationMethod.Tab : NavigationMethod.Unspecified);
            else if (watched?.CursorNode is { } cursor && rowIndex.FindItem(cursor) is { } item)
                item.Focus(NavigationMethod.Tab);
        }

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

        // Set by a press beside the rows: the cursor row takes the focus once the press is done.
        private bool focusBesideRows;

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedNode = null;
            chosenNode = null;
            focusBeforeRowButton = null;
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
                    focusBeforeRowButton = new FocusBefore(FocusedControl());
                return;
            }

            var node = TreeRows.ItemOf(e.Source)?.DataContext as TreeNode;
            // A click beside the rows gives the tree the focus on its cursor row, so the keys work right away. A tree without a cursor
            // gets one first: the selected row, else the first row. The focus moves after the press is done (OnPointerPressedDone): the
            // default handling of the press would take it back.
            if (node is null && DataContext is ProjectTree beside)
            {
                beside.CursorNode ??= beside.SelectedNode ?? beside.Roots.FirstOrDefault();
                focusBesideRows = beside.CursorNode != null;
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
