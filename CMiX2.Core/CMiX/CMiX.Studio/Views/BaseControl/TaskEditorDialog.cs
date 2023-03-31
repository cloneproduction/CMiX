// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using CMiX.Core.Presentations.Views.Scheduling;
using MvvmDialogs;

namespace CMiX.Studio.Views.Dialogs
{
    public class TaskEditorDialog : IWindow
    {
        private readonly TaskEditor dialog;

        public TaskEditorDialog()
        {
            dialog = new TaskEditor();
        }

        object IWindow.DataContext
        {
            get => dialog.DataContext;
            set => dialog.DataContext = value;
        }

        bool? IWindow.DialogResult
        {
            get => dialog.DialogResult;
            set => dialog.DialogResult = value;
        }

        ContentControl IWindow.Owner
        {
            get => dialog.Owner;
            set => dialog.Owner = (Window)value;
        }

        bool? IWindow.ShowDialog()
        {
            return dialog.ShowDialog();
        }

        void IWindow.Show()
        {
            dialog.Show();
        }
    }
}
