// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class TreeNodeTests
    {
        [Fact]
        public void ToggleExpand_ACardTwice_ClosesAndOpensIt()
        {
            var node = CreateScene().NodeA;
            var before = node.IsExpanded;

            node.ToggleExpand();
            var toggled = node.IsExpanded;
            node.ToggleExpand();

            Assert.Equal(!before, toggled);
            Assert.Equal(before, node.IsExpanded);
        }
    }
}
