// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;

namespace CMiX.Studio.Views.Menus
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
            //DialogService = new DialogService();
        }

        //private IDialogService DialogService { get; set; }


        //public static readonly DependencyProperty PathToOpenProperty =
        //DependencyProperty.Register("PathToOpen", typeof(string), typeof(MainMenu), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //public string PathToOpen
        //{
        //    get { return (string)GetValue(PathToOpenProperty); }
        //    set { SetValue(PathToOpenProperty, value); }
        //}


        //private void Open_File(object sender, RoutedEventArgs e)
        //{
        //    var settings = new OpenFileDialogSettings
        //    {
        //        Title = "This Is The Title",
        //        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        //        Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*"
        //    };

        //    bool? success = DialogService.ShowOpenFileDialog((INotifyPropertyChanged)this.DataContext, settings);
        //    if (success == true)
        //    {
        //        Console.WriteLine();
        //        PathToOpen = settings.FileName;
        //    }
        //}
    }
}
