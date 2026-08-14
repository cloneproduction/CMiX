// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Core.Undo;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly UndoManager _undoManager;

        public MainWindow(UndoManager undoManager)
        {
            InitializeComponent();
            _undoManager = undoManager;
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
