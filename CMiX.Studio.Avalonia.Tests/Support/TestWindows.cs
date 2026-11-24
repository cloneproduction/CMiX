// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Session;
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
            closes.Add(() => Close(window));
            return window;
        }

        // The headless platform gives each window its own keyboard device. Its input method manager hangs on a static
        // property subscription and keeps the focused element and its window alive. A cleared focus lets the window go.
        public static void Close(Window window)
        {
            CloseMenus(window);
            window.FocusManager?.ClearFocus();
            window.Close();
        }

        // A context menu, a flyout or a popup that is open when its window closes keeps its subscription to the raw input of
        // the process, and with it the window. A close of the window does not close them.
        private static void CloseMenus(Window window)
        {
            foreach (var control in window.GetVisualDescendants().OfType<Control>().ToArray())
            {
                if (control.ContextMenu is { IsOpen: true } menu)
                    menu.Close();
                if (FlyoutBase.GetAttachedFlyout(control) is { IsOpen: true } flyout)
                    flyout.Hide();
                if (control is Button { Flyout: { IsOpen: true } buttonFlyout })
                    buttonFlyout.Hide();
                if (control is Popup { IsOpen: true } popup)
                    popup.IsOpen = false;
            }
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
                Close(shown.Window);
            });
            return shown;
        }

        public void Dispose()
        {
            for (var i = closes.Count - 1; i >= 0; i--)
                closes[i]();
            closes.Clear();
            StopPendingTimers();
        }

        // Avalonia starts a one shot hold timer on every press. The timer holds the press event and its window until it
        // fires, and the headless dispatcher fires it only when it is due at a Settle. The timers of a closed window are
        // of no use, so Dispose stops every pending timer of the dispatcher. WindowLeakTests fails when this stops working.
        private static readonly FieldInfo TimersField =
            typeof(Dispatcher).GetField("_timers", BindingFlags.Instance | BindingFlags.NonPublic)!;

        public static void StopPendingTimers()
        {
            var timers = (List<DispatcherTimer>)TimersField.GetValue(Dispatcher.UIThread)!;
            foreach (var timer in timers.ToArray())
                timer.Stop();
        }
    }
}
