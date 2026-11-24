// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Notices;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class MainWindowOwnerTests
    {
        [AvaloniaFact]
        public void Find_WithoutDesktopLifetime_ReturnsNull()
        {
            var owner = MainWindowOwner.Find();

            Assert.Null(owner);
        }
    }
}
