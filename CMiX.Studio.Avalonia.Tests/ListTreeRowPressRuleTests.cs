// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeRowPressRuleTests
    {
        [Fact]
        public void ActionOf_APlainPressOnARow_Selects()
        {
            Assert.Equal(ListTreeRowPressAction.Select, ListTreeRowPressRule.ActionOf(new(IsControlDown: false, IsShiftDown: false), ListTreeRowPressPart.Row));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionOf_APressOnTheIconOfACard_PassesOn(bool isShiftDown)
        {
            Assert.Equal(ListTreeRowPressAction.PassOn, ListTreeRowPressRule.ActionOf(new(IsControlDown: false, isShiftDown), ListTreeRowPressPart.CardIcon));
        }

        [Fact]
        public void ActionOf_ACtrlPressOnTheIconOfACard_Stops()
        {
            Assert.Equal(ListTreeRowPressAction.Stop, ListTreeRowPressRule.ActionOf(new(IsControlDown: true, IsShiftDown: false), ListTreeRowPressPart.CardIcon));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionOf_ACtrlPressOnARow_AddsOrRemoves(bool isShiftDown)
        {
            Assert.Equal(ListTreeRowPressAction.AddOrRemove, ListTreeRowPressRule.ActionOf(new(IsControlDown: true, isShiftDown), ListTreeRowPressPart.Row));
        }

        [Fact]
        public void ActionOf_AShiftPressOnARow_SelectsARange()
        {
            Assert.Equal(ListTreeRowPressAction.SelectRange, ListTreeRowPressRule.ActionOf(new(IsControlDown: false, IsShiftDown: true), ListTreeRowPressPart.Row));
        }
    }
}
