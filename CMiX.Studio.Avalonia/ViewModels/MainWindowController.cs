// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public class MainWindowController : ObservableObject, IControl
    {
        public MainWindowController()
        {
            CloseWindowCommand = new RelayCommand(CloseMainWindow);
            MinimizeWindowCommand = new RelayCommand<Window>(MinimizeWindow);
            MaximizeWindowCommand = new RelayCommand<Window>(MaximizeWindow);
        }

        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }
        public Guid ID { get; set; }

        private static void CloseMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow?.Close();
        }

        public void MaximizeWindow(Window? window)
        {
            if (window == null)
                return;

            window.WindowState = window.WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        public void MinimizeWindow(Window? window)
        {
            if (window != null)
                window.WindowState = WindowState.Minimized;
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
