// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ModalDialog : ObservableObject, IModalDialogViewModel
    {
        public ModalDialog()
        {
            SaveCommand = new RelayCommand<Window>(Save);
            DontSaveCommand = new RelayCommand<Window>(DontSave);
            CancelCommand = new RelayCommand<Window>(Cancel);
        }

        public bool? DialogResult { get; set; }

        public bool SaveProject { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand DontSaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public void Save(object obj)
        {
            if (obj is Window)
            {
                DialogResult = true;
                SaveProject = true;
                ((Window)obj).Close();
            }
        }

        public void DontSave(object obj)
        {
            if (obj is Window)
            {
                DialogResult = true;
                SaveProject = false;
                ((Window)obj).Close();
            }
        }

        public void Cancel(object obj)
        {
            DialogResult = false;
            ((Window)obj).Close();
        }
    }
}
