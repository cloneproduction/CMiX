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
            Assert.Equal(ListTreeRowPressAction.Select, ListTreeRowPressRule.ActionOf(isControlDown: false, isOnCardIcon: false));
        }

        [Fact]
        public void ActionOf_APressOnTheIconOfACard_PassesOn()
        {
            Assert.Equal(ListTreeRowPressAction.PassOn, ListTreeRowPressRule.ActionOf(isControlDown: false, isOnCardIcon: true));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionOf_ACtrlPress_Stops(bool isOnCardIcon)
        {
            Assert.Equal(ListTreeRowPressAction.Stop, ListTreeRowPressRule.ActionOf(isControlDown: true, isOnCardIcon));
        }
    }
}
