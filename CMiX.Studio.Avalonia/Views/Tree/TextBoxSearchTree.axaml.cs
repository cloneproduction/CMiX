// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using System.Windows.Input;
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
        }

        // For the tests.
        internal TextBoxSearch Field => searchField;
        internal Button Clear => searchField.GetVisualDescendants().OfType<Button>().First(button => button.Name == TextBoxSearch.ClearPartName);

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (watched != null)
                watched.SearchFocusRequested -= OnSearchFocusRequested;

            watched = DataContext as ProjectTree;
            if (watched != null)
                watched.SearchFocusRequested += OnSearchFocusRequested;
        }

        // The focus waits for the layout, in case the panel shows the tree only now.
        // With two panels, only the box of the panel of the active view answers.
        private void OnSearchFocusRequested(object? sender, EventArgs e)
        {
            if (TreeOfPanel() is { IsActiveView: false })
                return;

            Dispatcher.UIThread.Post(() =>
            {
                searchField.FocusInput();
                searchField.SelectAll();
            }, DispatcherPriority.Loaded);
        }

        private ListTreeProject? TreeOfPanel() =>
            this.FindAncestorOfType<PanelSide>()?.GetVisualDescendants().OfType<ListTreeProject>().FirstOrDefault();

        private void OnFieldKeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is not ProjectTree tree)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    Run(tree.LeaveSearchCommand, e);
                    break;
                case Key.Enter when e.KeyModifiers == KeyModifiers.None:
                    Run(tree.MoveCursorToNextMatchCommand, e);
                    break;
                case Key.Enter when e.KeyModifiers == KeyModifiers.Shift:
                    Run(tree.MoveCursorToPreviousMatchCommand, e);
                    break;
                case Key.Down when e.KeyModifiers == KeyModifiers.None:
                    Run(tree.FocusTreeCommand, e);
                    break;
            }
        }

        private static void Run(ICommand command, KeyEventArgs e)
        {
            if (!command.CanExecute(null))
                return;

            command.Execute(null);
            e.Handled = true;
        }
    }
}
