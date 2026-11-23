// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    // The screen hub of the new tree. It builds its parts and attaches them to the tree while the view is in the visual tree.
    public partial class ListTreeProject : UserControl
    {
        private readonly TreeViewLifetime lifetime = new();
        private readonly ListTreeViewState state = new();
        private readonly ListTreeFocus focus;
        private readonly ListTreeViewStateInput stateInput;
        private readonly ListTreeFocusKeeper focusKeeper;
        private readonly ListTreePressFocus pressFocus;
        private readonly ListTreeRowClick rowClick;
        private readonly ListTreeAddBeside addBeside;
        private readonly ListTreeRowReveal reveal;
        private readonly ListTreeScrollAnchor scrollAnchor;
        private readonly ListTreeEndRoom endRoom;
        private readonly ListTreeLastPointer pointer;
        private readonly ListTreeHints hints;
        private readonly ListTreeMenus menus;
        private readonly ListTreeKeyInput keyInput;
        private readonly ListTreeFileDropInput fileDrop;
        private readonly ListTreeScreenRequests requests;
        private readonly TreeTooltips tooltips;
        private readonly TreeViewHooks hooks;

        private ProjectTree? watched;

        public ListTreeProject()
        {
            InitializeComponent();

            focus = new ListTreeFocus(this);
            focusKeeper = new ListTreeFocusKeeper(this, rows, lifetime);
            pressFocus = new ListTreePressFocus(rows, focus, focusKeeper, lifetime);
            stateInput = new ListTreeViewStateInput(this, focus, state);
            rowClick = new ListTreeRowClick(scroller, rows, pressFocus);
            var addPanel = new AddPanel();
            hints = new ListTreeHints(this, hintStrip, focus, new ListTreeMenuOpen(scroller, addPanel), lifetime);
            addBeside = new ListTreeAddBeside(rows, focusKeeper, addPanel, hints.Update, lifetime);
            reveal = new ListTreeRowReveal(this, rows, scroller, lifetime);
            scrollAnchor = new ListTreeScrollAnchor(rows, scroller);
            endRoom = new ListTreeEndRoom(rows, scroller, endRoomSpace);
            pointer = new ListTreeLastPointer(this, scroller);
            menus = new ListTreeMenus(scroller, reveal, addBeside, pointer, hints.Update);
            keyInput = new ListTreeKeyInput(scroller, rows, focusKeeper, addBeside, lifetime);
            fileDrop = new ListTreeFileDropInput(scroller, rows);
            requests = new ListTreeScreenRequests(rows, menus, reveal, scrollAnchor, () => IsActiveView);
            hooks = BuildHooks();
            KeyBindingTextGuard.Install(this);
            tooltips = new TreeTooltips(rows);
        }

        private TreeViewHooks BuildHooks() =>
            new()
            {
                IsPointerOverTree = () => state.IsPointerOverTree,
                IsTreeFocused = () => state.IsTreeFocused,
                IsTextBoxFocused = () => state.IsTextBoxFocused,
                IsFocusInOtherPanel = () => state.IsFocusInOtherPanel,
                IsPanelFocused = () => state.IsPanelFocused,
                CloseRowsInView = requests.CloseRowsInView
            };

        internal bool IsActiveView => watched?.IsActiveView(hooks) == true;

        internal ListTreeViewState State => state;

        internal PanelListTreeRows Rows => rows;

        internal TreeTooltips Tooltips => tooltips;

        internal AddPanel MenuAddPanel => addBeside.AddPanel;

        internal ContextMenu? Menu => scroller.ContextMenu;

        internal KeyHintStrip HintStrip => hintStrip;

        internal TreeHintMode HintMode => hints.Mode;

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (lifetime.IsAttached)
                Unwatch();

            watched = DataContext as ProjectTree;
            if (lifetime.IsAttached)
                Watch();
        }

        private void Watch()
        {
            if (watched == null)
                return;

            stateInput.Attach();
            watched.AttachView(hooks);
            requests.Watch(watched);
            focusKeeper.Attach(watched);
            pressFocus.Attach(watched);
            rowClick.Attach(watched);
            keyInput.Attach(watched);
            addBeside.Attach(watched);
            pointer.Attach();
            menus.Attach(watched);
            hints.Watch(watched);
            fileDrop.Attach(watched);
            reveal.Watch(watched);
            endRoom.Attach();
        }

        private void Unwatch()
        {
            if (watched == null)
                return;

            endRoom.Detach();
            reveal.Unwatch();
            fileDrop.Detach();
            hints.Unwatch();
            menus.Detach();
            pointer.Detach();
            addBeside.Detach();
            keyInput.Detach();
            rowClick.Detach();
            pressFocus.Detach();
            focusKeeper.Detach();
            requests.Unwatch();
            watched.DetachView(hooks);
            stateInput.Detach();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // The tree view model listens only while the view is attached.
            lifetime.Attach();
            Watch();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            Unwatch();
            lifetime.Detach();
            base.OnDetachedFromVisualTree(e);
        }
    }
}
