// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Threading.Tasks;
using Avalonia;

namespace CMiX.Studio.Avalonia
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => App.HandleUnhandledException(e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                App.HandleUnhandledException(e.Exception);
                e.SetObserved();
            };

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
        }
    }
}
