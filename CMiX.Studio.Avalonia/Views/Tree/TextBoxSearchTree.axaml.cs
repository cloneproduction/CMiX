// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views
{
    // The text stays when the focus goes out of the field, so the rows stay dimmed until the text is cleared.
    public partial class TextBoxSearchTree : UserControl
    {
        private ProjectTree? watched;

        internal const string FilteringClass = "filtering";

        public TextBoxSearchTree()
        {
            InitializeComponent();
            searchField.AddHandler(KeyDownEvent, OnFieldKeyDown, RoutingStrategies.Tunnel);
            searchField.PropertyChanged += (_, e) =>
            {
                if (e.Property == IsKeyboardFocusWithinProperty)
                    UpdateFiltering();
            };
        }

        private void UpdateFiltering() =>
            searchField.Classes.Set(FilteringClass, watched?.IsSearchActive == true && !searchField.IsKeyboardFocusWithin);

        private void OnWatchedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProjectTree.IsSearchActive))
                UpdateFiltering();
        }

        // For the tests.
        internal TextBoxSearch Field => searchField;
        internal Button Clear => searchField.GetVisualDescendants().OfType<Button>().First(button => button.Name == TextBoxSearch.ClearPartName);

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (watched != null)
            {
                watched.SearchFocusRequested -= OnSearchFocusRequested;
                watched.PropertyChanged -= OnWatchedPropertyChanged;
            }

            watched = DataContext as ProjectTree;
            if (watched != null)
            {
                watched.SearchFocusRequested += OnSearchFocusRequested;
                watched.PropertyChanged += OnWatchedPropertyChanged;
            }

            UpdateFiltering();
        }

        // The focus waits for the layout, in case the panel shows the tree only now.
        // With two panels, only the box of the panel of the active view answers.
        private void OnSearchFocusRequested(object? sender, EventArgs e)
        {
            if (TreeViewOfPanel() is { IsActiveView: false })
                return;

            Dispatcher.UIThread.Post(() =>
            {
                searchField.FocusInput();
                searchField.SelectAll();
            }, DispatcherPriority.Loaded);
        }

        private TreeViewProject? TreeViewOfPanel() =>
            this.FindAncestorOfType<PanelSide>()?.GetVisualDescendants().OfType<TreeViewProject>().FirstOrDefault();

        private void OnFieldKeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is not ProjectTree tree)
                return;

            if (tree.IsDragging)
            {
                if (e.Key == Key.Escape)
                {
                    tree.Views.CancelDrag();
                    e.Handled = true;
                }

                return;
            }

            switch (e.Key)
            {
                case Key.Escape when tree.SearchText.Length > 0:
                    tree.SearchText = string.Empty;
                    break;
                case Key.Escape:
                    tree.FocusTree();
                    break;
                case Key.Enter when e.KeyModifiers is KeyModifiers.None or KeyModifiers.Shift:
                    tree.MoveCursorToMatch(e.KeyModifiers == KeyModifiers.Shift ? -1 : 1);
                    break;
                case Key.Down when e.KeyModifiers == KeyModifiers.None:
                    tree.FocusTree();
                    break;
                default:
                    return;
            }

            e.Handled = true;
        }
    }
}
