// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Input;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.ViewModels
{
    public class MainWindowController : ObservableObject, IControl
    {
        public MainWindowController()
        {
            CloseWindowCommand = new RelayCommand(() => Application.Current.MainWindow.Close());
            MinimizeWindowCommand = new RelayCommand<Window>(MinimizeWindow);
            MaximizeWindowCommand = new RelayCommand<Window>(MaximizeWindow);
        }

        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }
        public Guid ID { get; set ; }

        public void MaximizeWindow(object obj)
        {
            if (obj is Window)
            {
                var window = obj as Window;
                if (window.WindowState == WindowState.Normal)
                    window.WindowState = WindowState.Maximized;
                else
                    window.WindowState = WindowState.Normal;
            }
        }

        public void MinimizeWindow(object obj)
        {
            if (obj is Window)
                ((Window)obj).WindowState = WindowState.Minimized;
        }

        public void CloseWindow(object obj)
        {
            //if (obj is Window)
            //{
            //    var window = obj as Window;

            //    var modalDialog = new ModalDialog();
            //    bool? success = DialogService.ShowDialog(this, modalDialog);
            //    if (success == true)
            //    {
            //        if (modalDialog.SaveProject)
            //        {
            //            return;
            //            //var projectSaved = SaveAsProject();
            //            //if (projectSaved)
            //            //    window.Close();
            //        }
            //        window.Close();
            //    }
            //}
        }

        private void Quit(object p)
        {
            var window = p as Window;
            window.Close();
        }

        public IControlModel ToModel()
        {
            throw new NotImplementedException();
        }

        public void FromModel(IControlModel model)
        {
            throw new NotImplementedException();
        }
    }
}
