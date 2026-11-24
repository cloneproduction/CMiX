// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A closed test window must be garbage. A window that stays alive keeps its whole project graph alive.
    public class WindowLeakTests
    {
        [AvaloniaFact]
        public void Dispose_OfTheTestWindows_LetsTheMainWindowGo()
        {
            var window = ShowAndClose(openMenu: false);

            AssertCollected(window);
        }

        [AvaloniaFact]
        public void Dispose_OfTheTestWindowsWithAnOpenMenu_LetsTheMainWindowGo()
        {
            var window = ShowAndClose(openMenu: true);

            AssertCollected(window);
        }

        private static void AssertCollected(WeakReference window)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.False(window.IsAlive, "The main window is still alive after its close.");
        }

        // The window lives only in this frame, so nothing on the test stack keeps it.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference ShowAndClose(bool openMenu)
        {
            var windows = new TestWindows();
            var screen = ListTreeMainScreen.Show(windows);
            screen.Window.Settle();
            screen.Window.Click(screen.Window.CenterOf(screen.Window));
            if (openMenu)
            {
                var row = screen.View.GetVisualDescendants().OfType<RowListTree>().First();
                screen.Window.RightClick(screen.Window.CenterOf(row));
                Assert.True(screen.View.Menu?.IsOpen);
            }
            windows.Dispose();
            screen.Window.Settle();
            Dispatcher.UIThread.RunJobs();
            return new WeakReference(screen.Window);
        }
    }
}
