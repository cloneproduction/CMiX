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
            Assert.Equal(ListTreeRowPressRule.Action.Select, ListTreeRowPressRule.ActionOf(new(IsControlDown: false, IsShiftDown: false), ListTreeRowPressRule.Part.Row));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionOf_APressOnTheIconOfACard_PassesOn(bool isShiftDown)
        {
            Assert.Equal(ListTreeRowPressRule.Action.PassOn, ListTreeRowPressRule.ActionOf(new(IsControlDown: false, isShiftDown), ListTreeRowPressRule.Part.CardIcon));
        }

        [Fact]
        public void ActionOf_ACtrlPressOnTheIconOfACard_Stops()
        {
            Assert.Equal(ListTreeRowPressRule.Action.Stop, ListTreeRowPressRule.ActionOf(new(IsControlDown: true, IsShiftDown: false), ListTreeRowPressRule.Part.CardIcon));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionOf_ACtrlPressOnARow_AddsOrRemoves(bool isShiftDown)
        {
            Assert.Equal(ListTreeRowPressRule.Action.AddOrRemove, ListTreeRowPressRule.ActionOf(new(IsControlDown: true, isShiftDown), ListTreeRowPressRule.Part.Row));
        }

        [Fact]
        public void ActionOf_AShiftPressOnARow_SelectsARange()
        {
            Assert.Equal(ListTreeRowPressRule.Action.SelectRange, ListTreeRowPressRule.ActionOf(new(IsControlDown: false, IsShiftDown: true), ListTreeRowPressRule.Part.Row));
        }
    }
}
