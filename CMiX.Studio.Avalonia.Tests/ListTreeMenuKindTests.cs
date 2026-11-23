// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views.Managers;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeMenuKindTests
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));

        [Fact]
        public void Of_AGroupRow_IsGroup()
        {
            Assert.Equal(RowMenuKind.Group, ListTreeMenuKind.Of(scene.Group<EntitiesGroup>(0)));
        }

        [Fact]
        public void Of_ALayerAndAComposition_IsComposable()
        {
            Assert.Equal(RowMenuKind.Composable, ListTreeMenuKind.Of(scene.LayerNodeOfA(0)));
            Assert.Equal(RowMenuKind.Composable, ListTreeMenuKind.Of(scene.NodeA));
        }

        [Fact]
        public void Of_AnItemOfALayer_IsSlot()
        {
            Assert.Equal(RowMenuKind.Slot, ListTreeMenuKind.Of(scene.Group<EntitiesGroup>(0).Children[0]));
        }
    }
}
