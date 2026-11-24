// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The windows that one test opens. Dispose closes them, the last one first.
    internal sealed class TestWindows : IDisposable
    {
        private readonly List<Action> closes = new();

        public T Add<T>(T window) where T : Window
        {
            closes.Add(window.Close);
            return window;
        }

        public (IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel) ShowMainWindow() =>
            ShowMainWindow(_ => { });

        public (IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel) ShowMainWindow(Action<IServiceCollection> replace)
        {
            var shown = TestServiceProviderFactory.ShowMainWindow(replace);
            // The main window does not close while the project has unsaved changes.
            closes.Add(() =>
            {
                shown.Provider.GetRequiredService<UnsavedChanges>().MarkSaved();
                shown.Window.Close();
            });
            return shown;
        }

        public void Dispose()
        {
            for (var i = closes.Count - 1; i >= 0; i--)
                closes[i]();
            closes.Clear();
        }
    }
}
