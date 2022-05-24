using System.Windows;

namespace CMiX.Studio.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        //private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        //private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    var root = DataContext as MainViewModel;
        //    if (root.Mementor.CanUndo)
        //        root.Mementor.Undo();
        //}

        //private void RedoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        //private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    var root = DataContext as MainViewModel;
        //    if (root.Mementor.CanRedo)
        //        root.Mementor.Redo();
        //}
    }
}
