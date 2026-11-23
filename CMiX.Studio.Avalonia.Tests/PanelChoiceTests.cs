// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class PanelChoiceTests
    {
        [Fact]
        public void All_TheListTree_ComesRightAfterTheTree()
        {
            var all = PanelChoice.All.ToList();

            Assert.IsType<ListTreePanelChoice>(PanelChoice.ListTree);
            Assert.Equal(all.IndexOf(PanelChoice.Tree) + 1, all.IndexOf(PanelChoice.ListTree));
            Assert.Equal("List tree", PanelChoice.ListTree.Name);
        }
    }
}
