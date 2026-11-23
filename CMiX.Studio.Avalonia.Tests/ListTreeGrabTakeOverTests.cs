// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeGrabTakeOverTests
    {
        [Theory]
        [InlineData(13.9, true)]
        [InlineData(14, false)]
        public void KeepsTheKeyMark_AMoveNearTheDistance_KeepsTheMarkOnlyUnderIt(double length, bool expected)
        {
            var keyMove = new ListTreePoint(10, 20);

            var keeps = ListTreeGrabTakeOver.KeepsTheKeyMark(keyMove, new ListTreePoint(10, 20 + length), 14);

            Assert.Equal(expected, keeps);
        }
    }
}
