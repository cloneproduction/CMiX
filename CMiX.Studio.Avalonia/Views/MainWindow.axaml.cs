// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using CMiX.Core.Undo;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly UndoManager? _undoManager;
        public MainWindow(UndoManager undoManager) : this()
        {
            _undoManager = undoManager;
        }

        public MainWindow()
        {
            InitializeComponent();
            titleBar.PointerPressed += TitleBar_PointerPressed;
        }

        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.Source is ILogical source &&
                (source.FindLogicalAncestorOfType<Menus.MainMenu>(true) != null ||
                 source.FindLogicalAncestorOfType<Windows.MainWindowController>(true) != null))
                return;

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                BeginMoveDrag(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_undoManager is null)
                return; // design-time instance, nothing to undo/redo

            if (e.Key == Key.Z && e.KeyModifiers == KeyModifiers.Control)
            {
                _undoManager.Undo();
                e.Handled = true;
            }
            else if (e.Key == Key.Z && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
            {
                _undoManager.Redo();
                e.Handled = true;
            }
            else if (e.Key == Key.Y && e.KeyModifiers == KeyModifiers.Control)
            {
                _undoManager.Redo();
                e.Handled = true;
            }
        }
    }
}
