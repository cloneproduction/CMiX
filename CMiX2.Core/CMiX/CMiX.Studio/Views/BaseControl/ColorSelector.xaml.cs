using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MvvmDialogs;
using CMiX.Core.Presentations.Views.Dialogs;
using System.ComponentModel;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class ColorSelector : UserControl
    {
        public ColorSelector()
        {
            InitializeComponent();
            DialogService = new DialogService();
        }

        private IDialogService DialogService { get; set; }

        public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(INotifyPropertyChanged), typeof(ColorSelector), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public INotifyPropertyChanged ViewModel
        {
            get { return (INotifyPropertyChanged)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(ColorSelector), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(ColorSelector), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSelector), new FrameworkPropertyMetadata(Color.FromArgb(255, 255, 255, 255), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogService.Show<ColorSelectorWindow>((INotifyPropertyChanged)this.DataContext, ViewModel);
        }
    }
}
