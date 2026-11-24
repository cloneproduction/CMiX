// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class PanelChoiceTests
    {
        [Fact]
        public void All_TheChoices_StartWithTheTree()
        {
            var all = PanelChoice.All.ToList();

            Assert.IsType<TreePanelChoice>(PanelChoice.Tree);
            Assert.Equal(0, all.IndexOf(PanelChoice.Tree));
            Assert.Equal("Tree", PanelChoice.Tree.Name);
            Assert.Equal(new[] { PanelChoice.Tree, PanelChoice.Tabs, PanelChoice.Assets }, all);
        }
    }
}
