// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The key hint strip under the rows of the tree, one set of hints per mode (TreeHintMode).
    internal sealed class ListTreeHints : ListTreeBehavior
    {
        private ProjectTree? tree;
        private TopLevel? window;
        private TreeHintMode? shownMode;

        public TreeHintMode Mode
        {
            get
            {
                if (Part<ListTreeGrab>().IsGrabbing)
                    return TreeHintMode.Grab;

                if (Context.IsMenuOpen)
                    return TreeHintMode.Menu;

                if (Context.Focus.IsSearchFocused())
                    return TreeHintMode.Search;

                return tree?.IsSearchActive == true && Context.Owner.IsKeyboardFocusWithin ? TreeHintMode.Filter : TreeHintMode.Idle;
            }
        }

        // Only a new mode or a new gesture changes the text, so the text never changes for nothing.
        public void Update()
        {
            if (tree is not { } shown)
                return;

            var mode = Mode;
            if (mode != shownMode)
                Show(shown, mode);
        }

        protected override void OnWatch(ProjectTree watched)
        {
            tree = watched;
            watched.PropertyChanged += OnTreeChanged;
            watched.Shortcuts.PropertyChanged += OnShortcutsChanged;
            Part<ListTreeGrab>().GrabbingChanged += OnModeSourceChanged;
            Context.AddPanel.Opened += OnModeSourceChanged;
            Context.AddPanel.Closed += OnModeSourceChanged;
            AttachWindow(TopLevel.GetTopLevel(Context.Owner));
            Show(watched, Mode);
        }

        protected override void OnUnwatch()
        {
            AttachWindow(null);
            if (tree == null)
                return;

            tree.PropertyChanged -= OnTreeChanged;
            tree.Shortcuts.PropertyChanged -= OnShortcutsChanged;
            Part<ListTreeGrab>().GrabbingChanged -= OnModeSourceChanged;
            Context.AddPanel.Opened -= OnModeSourceChanged;
            Context.AddPanel.Closed -= OnModeSourceChanged;
            tree = null;
        }

        private void AttachWindow(TopLevel? attached)
        {
            window?.RemoveHandler(InputElement.GotFocusEvent, OnWindowFocusChanged);
            window?.RemoveHandler(InputElement.LostFocusEvent, OnWindowFocusChanged);
            window = attached;
            window?.AddHandler(InputElement.GotFocusEvent, OnWindowFocusChanged, RoutingStrategies.Bubble, handledEventsToo: true);
            window?.AddHandler(InputElement.LostFocusEvent, OnWindowFocusChanged, RoutingStrategies.Bubble, handledEventsToo: true);
        }

        private void Show(ProjectTree shown, TreeHintMode mode)
        {
            shownMode = mode;
            Context.HintStrip.Hints = TreeKeyHints.For(mode, shown.Shortcuts);
        }

        // The focus moves before the next control is known, so the check runs after the event.
        private void OnWindowFocusChanged(object? sender, RoutedEventArgs e) => Context.Lifetime.Post(Update, DispatcherPriority.Input);

        private void OnModeSourceChanged(object? sender, EventArgs e) => Update();

        private void OnShortcutsChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (tree is { } shown)
                Show(shown, Mode);
        }

        private void OnTreeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProjectTree.IsSearchActive))
                Update();
        }
    }
}
