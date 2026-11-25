// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeSizesTests
    {
        [AvaloniaFact]
        public void Default_RowHeight_IsTheTokenListTreeRowHeight()
        {
            Assert.Equal((double)Application.Current!.FindResource("ListTreeRowHeight")!, ListTreeSizes.Default.RowHeight);
        }
    }
}
