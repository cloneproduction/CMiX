using System.Windows;
using System.Windows.Input;
using CMiX.Core;
using CMiX.Core.Undo;

namespace CMiX.Studio.Views
{
    public partial class MainWindow : Window
    {
        private readonly UndoManager _undoManager;

        public MainWindow(UndoManager undoManager)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _undoManager = undoManager;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _undoManager.Undo();
                e.Handled = true;
            }
            else if (e.Key == Key.Z && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                _undoManager.Redo();
                e.Handled = true;
            }
        }
    }
}
