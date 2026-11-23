// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreePressTests
    {
        private static readonly ListTreePlace Place = new(new ListTreeRowKey(new object(), null), 70, 32, 32, 2, 32, false);

        [Fact]
        public void At_APointerOnARow_KeepsThePointerAndItsOffsetInTheRow()
        {
            var pointer = new ListTreePoint(120, 80);

            var press = ListTreePress.At(pointer, Place);

            Assert.Equal(new ListTreePress(pointer, 10, 2), press);
        }

        [Fact]
        public void AtMiddleOf_ARow_PressesAtTheMiddleOfTheRow()
        {
            var press = ListTreePress.AtMiddleOf(Place, 120);

            Assert.Equal(new ListTreePress(new ListTreePoint(120, 86), 16, 2), press);
        }
    }
}
