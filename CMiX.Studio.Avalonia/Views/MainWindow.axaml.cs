// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using CMiX.Core.Undo;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly UndoManager _undoManager;

        // The DataContext is assigned by App after construction, matching WPF. With
        // the context already present during construction every deferred child
        // binding transiently evaluates against the window context before its local
        // DataContext applies, which logs far more binding noise than the null case.
        public MainWindow(UndoManager undoManager)
        {
            InitializeComponent();
            _undoManager = undoManager;
            titleBar.PointerPressed += TitleBar_PointerPressed;
        }

        private void TitleBar_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            // The WPF chrome excluded the menu and the window buttons from the drag
            // area with IsHitTestVisibleInChrome. Starting a move drag here would
            // steal their pointer release and no click would ever complete, so
            // presses inside those interactive children never begin a drag.
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
        }
    }
}
