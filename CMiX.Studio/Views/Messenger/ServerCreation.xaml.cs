using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MvvmDialogs;

namespace CMiX.Studio.Views
{
    public partial class ServerCreation : UserControl
    {
        public ServerCreation()
        {
            InitializeComponent();
            DialogService = new DialogService();
        }

        private IDialogService DialogService { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogService.Show<ServerCreationWindow>((INotifyPropertyChanged)this.DataContext, ViewModel);
        }

        public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(INotifyPropertyChanged), typeof(ServerCreation), new FrameworkPropertyMetadata(null));
        public INotifyPropertyChanged ViewModel
        {
            get { return (INotifyPropertyChanged)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
