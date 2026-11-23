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
    // The key hint strip under the rows of the new tree, with the modes of TreeHints. The grab mode comes with the drag.
    internal sealed class ListTreeHints
    {
        private readonly Control owner;
        private readonly KeyHintStrip strip;
        private readonly ListTreeFocus focus;
        private readonly Func<bool> isMenuOpen;
        private readonly TreeViewLifetime lifetime;

        private ProjectTree? tree;
        private TopLevel? window;
        private TreeHintMode? shownMode;

        public ListTreeHints(Control owner, KeyHintStrip strip, ListTreeFocus focus, Func<bool> isMenuOpen, TreeViewLifetime lifetime)
        {
            this.owner = owner;
            this.strip = strip;
            this.focus = focus;
            this.isMenuOpen = isMenuOpen;
            this.lifetime = lifetime;
        }

        public TreeHintMode Mode
        {
            get
            {
                if (isMenuOpen())
                    return TreeHintMode.Menu;

                if (focus.IsSearchFocused())
                    return TreeHintMode.Search;

                return tree?.IsSearchActive == true && owner.IsKeyboardFocusWithin ? TreeHintMode.Filter : TreeHintMode.Idle;
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

        public void Watch(ProjectTree watched)
        {
            tree = watched;
            watched.PropertyChanged += OnTreeChanged;
            watched.Shortcuts.PropertyChanged += OnShortcutsChanged;
            AttachWindow(TopLevel.GetTopLevel(owner));
            Show(watched, Mode);
        }

        public void Unwatch()
        {
            AttachWindow(null);
            if (tree == null)
                return;

            tree.PropertyChanged -= OnTreeChanged;
            tree.Shortcuts.PropertyChanged -= OnShortcutsChanged;
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
            strip.Hints = TreeKeyHints.For(mode, shown.Shortcuts);
        }

        // The focus moves before the next control is known, so the check runs after the event.
        private void OnWindowFocusChanged(object? sender, RoutedEventArgs e) => lifetime.Post(Update, DispatcherPriority.Input);

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
