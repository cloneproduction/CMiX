// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeNodeWatchTests
    {
        private readonly Scene scene = CreateScene();
        private readonly ListTreeNodeWatch watch = new();
        private int changes;

        public ListTreeNodeWatchTests()
        {
            watch.Changed += (_, _) => changes++;
            watch.Watch(scene.Tree.Roots);
        }

        [Fact]
        public void Watch_ADeepCardCloses_RaisesChanged()
        {
            scene.LayerNodeOfA(0).IsExpanded = !scene.LayerNodeOfA(0).IsExpanded;

            Assert.Equal(1, changes);
        }

        [Fact]
        public void Watch_ARowComes_RaisesChanged()
        {
            scene.A.LayerManager.AddItem(typeof(Layer));

            Assert.True(changes > 0);
        }

        [Fact]
        public void Watch_ALayerIsHidden_RaisesHiddenChangedOnceAndNotChanged()
        {
            var hiddenChanges = 0;
            watch.HiddenChanged += (_, _) => hiddenChanges++;

            scene.Layer.PrefabService.Visibility.Value = true;

            Assert.Equal(1, hiddenChanges);
            Assert.Equal(0, changes);
        }

        [Fact]
        public void Unwatch_ACardCloses_RaisesNothing()
        {
            watch.Unwatch();

            scene.NodeA.IsExpanded = false;
            scene.A.LayerManager.AddItem(typeof(Layer));

            Assert.Equal(0, changes);
        }
    }
}
