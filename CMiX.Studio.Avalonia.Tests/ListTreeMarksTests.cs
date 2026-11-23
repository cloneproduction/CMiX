// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeMarksTests
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly ListTreeMarks marks = new(null, null, null);

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entity => scene.Group<EntitiesGroup>(0).Children[0];

        [Fact]
        public void IsHidden_ARowAtRest_IsFalse()
        {
            Assert.False(marks.IsHidden(Layer));
            Assert.False(marks.IsHidden(Entity));
            Assert.False(marks.IsHidden(null));
        }

        [Fact]
        public void IsHidden_AHiddenLayer_IsTrueForTheLayerAndEveryRowUnderIt()
        {
            scene.Layer.PrefabService.Visibility.Value = true;

            Assert.True(marks.IsHidden(Layer));
            Assert.True(marks.IsHidden(Entity));
            Assert.False(marks.IsHidden(scene.LayerNodeOfA(1)));
        }
    }
}
