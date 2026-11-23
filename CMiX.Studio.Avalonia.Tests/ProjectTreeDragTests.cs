// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ProjectTreeDragTests
    {
        [Fact]
        public void DropService_OfATree_TakesTheDropsThatTheTreeTakes()
        {
            var scene = CreateScene();
            var source = scene.LayerNodeOfA(0);
            var target = scene.LayerNodeOfA(1);

            var canDrop = scene.Tree.DropService.CanDrop(source, target, DropPosition.After);

            Assert.True(canDrop);
            Assert.Equal(scene.Tree.CanDrop(source, target, DropPosition.After), canDrop);
            Assert.Same(scene.Tree.DropService, scene.Tree.DropService);
        }
    }
}
