// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.Services;

namespace CMiX.Studio.Avalonia.Views.Dialogs
{
    // The question when the window closes with unsaved changes. The window closes with the choice as its result.
    public partial class UnsavedChangesDialog : Window
    {
        public UnsavedChangesDialog()
        {
            InitializeComponent();
        }

        private void OnSave(object? sender, RoutedEventArgs e) => Close(UnsavedChangesChoice.Save);

        private void OnDontSave(object? sender, RoutedEventArgs e) => Close(UnsavedChangesChoice.DontSave);

        private void OnCancel(object? sender, RoutedEventArgs e) => Close(UnsavedChangesChoice.Cancel);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key != Key.Escape)
                return;

            Close(UnsavedChangesChoice.Cancel);
            e.Handled = true;
        }
    }
}
