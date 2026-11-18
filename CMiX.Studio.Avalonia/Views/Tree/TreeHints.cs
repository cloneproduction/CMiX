// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    internal sealed record TreeHintInputs(Func<bool> IsGrabbing, Func<bool> IsMenuOpen, Func<bool> IsSearchFocused, Func<bool> IsTreeFocused);

    internal sealed class TreeHints
    {
        private readonly TreeViewContext context;
        private readonly KeyHintStrip strip;
        private readonly TreeHintInputs inputs;
        private readonly Action<Action, DispatcherPriority> postWhileAttached;

        private TreeHintMode? shownMode;

        internal TreeHints(TreeViewContext context, KeyHintStrip strip, TreeHintInputs inputs, Action<Action, DispatcherPriority> postWhileAttached)
        {
            this.context = context;
            this.strip = strip;
            this.inputs = inputs;
            this.postWhileAttached = postWhileAttached;
        }

        internal TreeHintMode Mode
        {
            get
            {
                if (inputs.IsGrabbing())
                    return TreeHintMode.Grab;

                if (inputs.IsMenuOpen())
                    return TreeHintMode.Menu;

                if (inputs.IsSearchFocused())
                    return TreeHintMode.Search;

                return context.Tree?.IsSearchActive == true && inputs.IsTreeFocused() ? TreeHintMode.Filter : TreeHintMode.Idle;
            }
        }

        // Only a new mode or a new gesture changes the text, so the text never changes for nothing.
        internal void Update()
        {
            if (context.Tree is not { } tree)
                return;

            var mode = Mode;
            if (mode != shownMode)
                Show(tree, mode);
        }

        internal void Watch(ProjectTree tree)
        {
            tree.Shortcuts.PropertyChanged += OnShortcutsChanged;
            RefreshGestures();
        }

        internal void Unwatch(ProjectTree tree) => tree.Shortcuts.PropertyChanged -= OnShortcutsChanged;

        internal void AttachWindow(TopLevel window)
        {
            window.AddHandler(InputElement.GotFocusEvent, OnWindowFocusChanged, RoutingStrategies.Bubble, handledEventsToo: true);
            window.AddHandler(InputElement.LostFocusEvent, OnWindowFocusChanged, RoutingStrategies.Bubble, handledEventsToo: true);
        }

        internal void DetachWindow(TopLevel window)
        {
            window.RemoveHandler(InputElement.GotFocusEvent, OnWindowFocusChanged);
            window.RemoveHandler(InputElement.LostFocusEvent, OnWindowFocusChanged);
        }

        private void RefreshGestures()
        {
            if (context.Tree is { } tree)
                Show(tree, Mode);
        }

        private void Show(ProjectTree tree, TreeHintMode mode)
        {
            shownMode = mode;
            strip.Hints = TreeKeyHints.For(mode, tree.Shortcuts);
        }

        // The focus moves before the next control is known, so the check runs after the event.
        private void OnWindowFocusChanged(object? sender, RoutedEventArgs e) => postWhileAttached(Update, DispatcherPriority.Input);

        private void OnShortcutsChanged(object? sender, PropertyChangedEventArgs e) => RefreshGestures();
    }
}
