// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The screen hub of the new tree. It builds its parts and attaches them to the tree while the view is in the visual tree.
    public partial class ListTreeProject : UserControl
    {
        private readonly TreeViewLifetime lifetime = new();
        private readonly ListTreeFocus focus;
        private readonly ListTreeRowClick rowClick;
        private readonly TreeViewHooks hooks;

        private ProjectTree? watched;

        public ListTreeProject()
        {
            InitializeComponent();

            focus = new ListTreeFocus(this);
            rowClick = new ListTreeRowClick(rows);
            hooks = BuildHooks();
        }

        private TreeViewHooks BuildHooks() =>
            new()
            {
                IsPointerOverTree = () => IsPointerOver,
                IsTreeFocused = () => IsKeyboardFocusWithin,
                IsTextBoxFocused = focus.IsTextBoxFocused,
                IsFocusInOtherPanel = focus.IsFocusInOtherPanel,
                IsPanelFocused = focus.IsPanelFocused
            };

        internal bool IsActiveView => watched?.IsActiveView(hooks) == true;

        internal PanelListTreeRows Rows => rows;

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

            watched.AttachView(hooks);
            rowClick.Attach(watched);
        }

        private void Unwatch()
        {
            if (watched == null)
                return;

            rowClick.Detach();
            watched.DetachView(hooks);
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
