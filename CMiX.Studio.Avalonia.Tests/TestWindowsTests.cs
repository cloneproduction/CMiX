// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Tests.Support;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class TestWindowsTests
    {
        [AvaloniaFact]
        public void Dispose_AWindowAndAMainWindowWithChanges_ClosesBoth()
        {
            var windows = new TestWindows();
            var window = windows.Add(new Window { Width = 200, Height = 100 });
            window.Show();
            var (_, mainWindow, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var closed = 0;
            window.Closed += (_, _) => closed++;
            mainWindow.Closed += (_, _) => closed++;

            windows.Dispose();

            Assert.Equal(2, closed);
        }
    }
}
