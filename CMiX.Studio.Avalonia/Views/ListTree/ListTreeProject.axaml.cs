// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    // The screen hub of the new tree. Its behaviors do the work. It gives them the context and attaches its hooks to the tree while the view is in the visual tree.
    public partial class ListTreeProject : UserControl
    {
        private readonly TreeViewHooks hooks;

        private ProjectTree? watched;

        public ListTreeProject()
        {
            InitializeComponent();

            Context = BuildContext();
            hooks = BuildHooks();
            KeyBindingTextGuard.Install(this);
        }

        private ListTreeContext BuildContext()
        {
            var addPanel = new AddPanel();
            return new ListTreeContext(this, rows, scroller, hintStrip, endRoomSpace, dragNote, new TreeViewLifetime(), new ListTreeViewState(),
                new ListTreeFocus(this), addPanel, new ListTreeMenuOpen(scroller, addPanel));
        }

        private TreeViewHooks BuildHooks() =>
            new()
            {
                IsPointerOverTree = () => Context.State.IsPointerOverTree,
                IsTreeFocused = () => Context.State.IsTreeFocused,
                IsTextBoxFocused = () => Context.State.IsTextBoxFocused,
                IsFocusInOtherPanel = () => Context.State.IsFocusInOtherPanel,
                IsPanelFocused = () => Context.State.IsPanelFocused,
                ToggleDragRow = () => Context.Part<ListTreeDragSession>().OpenNearCard(),
                CancelDrag = CancelDrag,
                CloseRowsInView = CloseRowsInView
            };

        private void CancelDrag()
        {
            Context.Part<ListTreeRowDrag>().Cancel();
            Context.Part<ListTreeGrab>().Cancel();
        }

        private void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close) =>
            Context.Part<ListTreeScreenRequests>().CloseRowsInView(node, close);

        internal ListTreeContext Context { get; }

        internal bool IsActiveView => watched?.IsActiveView(hooks) == true;

        internal ListTreeViewState State => Context.State;

        internal PanelListTreeRows Rows => rows;

        internal TreeTooltips Tooltips => Context.Part<ListTreeTooltips>().Tooltips;

        internal AddPanel MenuAddPanel => Context.AddPanel;

        internal ContextMenu? Menu => scroller.ContextMenu;

        internal KeyHintStrip HintStrip => hintStrip;

        internal TextBlock DragNote => dragNote;

        internal ListTreeRowDrag RowDrag => Context.Part<ListTreeRowDrag>();

        internal ListTreeGrab Grab => Context.Part<ListTreeGrab>();

        internal ListTreeAutoScroll AutoScroll => Context.Part<ListTreeAutoScroll>();

        internal TreeHintMode HintMode => Context.Part<ListTreeHints>().Mode;

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (Context.Lifetime.IsAttached)
                watched?.DetachView(hooks);

            watched = DataContext as ProjectTree;
            if (Context.Lifetime.IsAttached)
                watched?.AttachView(hooks);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // The tree view model listens only while the view is attached.
            Context.Lifetime.Attach();
            watched?.AttachView(hooks);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            watched?.DetachView(hooks);
            Context.Lifetime.Detach();
            base.OnDetachedFromVisualTree(e);
        }
    }
}
