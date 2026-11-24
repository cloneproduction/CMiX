// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.Notices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public class MainWindowController : ObservableObject
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

        private static void CloseMainWindow() => MainWindowOwner.Find()?.Close();

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
    }
}
