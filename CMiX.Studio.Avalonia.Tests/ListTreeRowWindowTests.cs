// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeRowWindowTests
    {
        [Fact]
        public void Around_AView_AddsHalfAViewAboveAndBelow()
        {
            var window = ListTreeRowWindow.Around(1000, 600);

            Assert.Equal(700, window.Top);
            Assert.Equal(1900, window.Bottom);
        }

        [Theory]
        [InlineData(670, 32, true)]
        [InlineData(1890, 32, true)]
        [InlineData(668, 32, false)]
        [InlineData(1900, 32, false)]
        public void Overlaps_ARow_IsTrueWhenAPartOfTheRowIsInTheWindow(double top, double height, bool expected)
        {
            var window = new ListTreeRowWindow(700, 1900);

            Assert.Equal(expected, window.Overlaps(top, height));
        }

        [Theory]
        [InlineData(1000, true)]
        [InlineData(1100, true)]
        [InlineData(1200, false)]
        [InlineData(800, false)]
        public void Holds_AScrolledView_IsTrueUntilAQuarterOfAViewIsLeft(double viewTop, bool expected)
        {
            var window = ListTreeRowWindow.Around(1000, 600);

            Assert.Equal(expected, window.Holds(viewTop, 600));
        }

        [Fact]
        public void All_AnyRow_OverlapsAndHolds()
        {
            Assert.True(ListTreeRowWindow.All.Overlaps(1e9, 32));
            Assert.True(ListTreeRowWindow.All.Holds(1e9, 600));
        }
    }
}
