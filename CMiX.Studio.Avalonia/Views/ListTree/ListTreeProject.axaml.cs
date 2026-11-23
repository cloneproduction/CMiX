// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.Behaviors.ListTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    // The screen hub of the new tree. It builds its parts and attaches them to the tree while the view is in the visual tree.
    public partial class ListTreeProject : UserControl
    {
        private readonly ListTreeViewStateInput stateInput;
        private readonly ListTreeFocusKeeper focusKeeper;
        private readonly ListTreePressFocus pressFocus;
        private readonly ListTreeRowClick rowClick;
        private readonly ListTreeAddBeside addBeside;
        private readonly ListTreeRowReveal reveal;
        private readonly ListTreeScrollAnchor scrollAnchor;
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

            Context = BuildContext();
            var lifetime = Context.Lifetime;
            focusKeeper = new ListTreeFocusKeeper(this, rows, lifetime);
            pressFocus = new ListTreePressFocus(rows, Context.Focus, focusKeeper, lifetime);
            stateInput = new ListTreeViewStateInput(this, Context.Focus, Context.State);
            rowClick = new ListTreeRowClick(scroller, rows, pressFocus);
            hints = new ListTreeHints(this, hintStrip, Context.Focus, Context.MenuOpen, lifetime);
            addBeside = new ListTreeAddBeside(rows, focusKeeper, Context.AddPanel, hints.Update, lifetime);
            reveal = new ListTreeRowReveal(this, rows, scroller, lifetime);
            scrollAnchor = new ListTreeScrollAnchor(rows, scroller);
            menus = new ListTreeMenus(scroller, reveal, addBeside, Context.Part<ListTreeLastPointer>(), hints.Update);
            keyInput = new ListTreeKeyInput(scroller, rows, focusKeeper, addBeside, lifetime);
            fileDrop = new ListTreeFileDropInput(scroller, rows);
            requests = new ListTreeScreenRequests(rows, menus, reveal, scrollAnchor, () => IsActiveView);
            hooks = BuildHooks();
            KeyBindingTextGuard.Install(this);
            tooltips = new TreeTooltips(rows);
        }

        private ListTreeContext BuildContext()
        {
            var addPanel = new AddPanel();
            return new ListTreeContext(this, rows, scroller, hintStrip, endRoomSpace, new TreeViewLifetime(), new ListTreeViewState(),
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
                CloseRowsInView = requests.CloseRowsInView
            };

        internal ListTreeContext Context { get; }

        internal bool IsActiveView => watched?.IsActiveView(hooks) == true;

        internal ListTreeViewState State => Context.State;

        internal PanelListTreeRows Rows => rows;

        internal TreeTooltips Tooltips => tooltips;

        internal AddPanel MenuAddPanel => addBeside.AddPanel;

        internal ContextMenu? Menu => scroller.ContextMenu;

        internal KeyHintStrip HintStrip => hintStrip;

        internal TreeHintMode HintMode => hints.Mode;

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (Context.Lifetime.IsAttached)
                Unwatch();

            watched = DataContext as ProjectTree;
            if (Context.Lifetime.IsAttached)
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
            menus.Attach(watched);
            hints.Watch(watched);
            fileDrop.Attach(watched);
            reveal.Watch(watched);
        }

        private void Unwatch()
        {
            if (watched == null)
                return;

            reveal.Unwatch();
            fileDrop.Detach();
            hints.Unwatch();
            menus.Detach();
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
            Context.Lifetime.Attach();
            Watch();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            Unwatch();
            Context.Lifetime.Detach();
            base.OnDetachedFromVisualTree(e);
        }
    }
}
