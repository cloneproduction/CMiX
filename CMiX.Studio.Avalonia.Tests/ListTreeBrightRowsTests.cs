// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeScenes;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeBrightRowsTests
    {
        private readonly TreeDropService service = new();

        // Layer 1 of A has two entities with modifiers, a camera and a filter. Layer 2 of A has only a filter.
        // B has an empty layer and a layer with one entity. Every empty list is closed.
        private static Scene CreateScene() => Opened(TreeFixtures.CreateScene([2, 2], Fill));

        private static void Fill(Scene scene)
        {
            var layer1 = scene.LayerOfA(0);
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.CameraManager.AddItem(typeof(Camera));
            layer1.TextureModifierManager.AddItem(typeof(Blur));
            var first = (Entity)layer1.ModelEntityManager.ManagerData.Items[0];
            first.ModifierManager.AddItem(typeof(TranslateModifier));
            first.ModifierManager.AddItem(typeof(ScaleModifier));
            scene.LayerOfA(1).TextureModifierManager.AddItem(typeof(Blur));
            ((Layer)scene.B.LayerManager.ManagerData.Items[1]).ModelEntityManager.AddItem(typeof(Entity));
        }

        private static ListTreeRowKey Key(TreeNode node) => ListTreeKeys.Of(node);

        private ListTreeLight LightOf(Scene scene, TreeNode source)
        {
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var row = snapshot.VisibleRows().Single(visible => ReferenceEquals(visible.Node, source));
            var candidates = new ListTreeCandidates(new ListTreeSlots(service), service, ListTreeSizes.Default).Of(snapshot, row);
            return ListTreeBrightRows.Of(snapshot, row, candidates);
        }

        [Fact]
        public void Of_AnEntity_KeepsEveryEntitiesListAndEveryEntityBright()
        {
            var scene = CreateScene();

            var light = LightOf(scene, Entity(scene, 0));

            Assert.Contains(Key(scene.Group<EntitiesGroup>(0)), light.PossibleTargets);
            Assert.Contains(Key(Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(1))), light.PossibleTargets);
            Assert.True(light.IsBright(Key(Entity(scene, 1))));
            Assert.True(light.IsBright(Key(EntityOfB(scene))));
        }

        [Fact]
        public void Of_AnEntity_KeepsEveryOpenLayerBrightAndDimsItsOtherLists()
        {
            var scene = CreateScene();

            var light = LightOf(scene, Entity(scene, 0));

            Assert.True(light.IsBright(Key(scene.LayerNodeOfA(0))));
            Assert.True(light.IsBright(Key(scene.LayerNodeOfB(1))));
            Assert.DoesNotContain(Key(scene.LayerNodeOfA(0)), light.PossibleTargets);
            Assert.False(light.IsBright(Key(scene.Group<CamerasGroup>(0))));
            Assert.False(light.IsBright(Key(scene.Group<FiltersGroup>(0))));
        }

        [Fact]
        public void Of_AnEntity_DimsTheCompositionsAndTheModifiersOfAnEntity()
        {
            var scene = CreateScene();
            var modifiers = Scene.Group<ModifiersGroup>(Entity(scene, 0));

            var light = LightOf(scene, Entity(scene, 1));

            Assert.False(light.IsBright(Key(scene.NodeA)));
            Assert.False(light.IsBright(Key(scene.NodeB)));
            Assert.False(light.IsBright(Key(modifiers)));
            Assert.False(light.IsBright(Key(modifiers.Children[0])));
        }

        [Fact]
        public void Of_ALayerWithAHiddenEntitiesList_TakesTheLayerAsAPossibleTarget()
        {
            var scene = CreateScene();
            var layerWithAFilter = scene.LayerNodeOfA(1);

            var light = LightOf(scene, Entity(scene, 0));

            Assert.Contains(Key(scene.LayerNodeOfB(0)), light.PossibleTargets);
            Assert.Contains(Key(layerWithAFilter), light.PossibleTargets);
            Assert.True(light.IsBright(Key(layerWithAFilter)));
            Assert.False(light.IsBright(Key(Scene.Group<FiltersGroup>(layerWithAFilter))));
        }

        [Fact]
        public void Of_NoCandidate_KeepsTheSourceAndItsSiblingsBright()
        {
            var scene = CreateScene();
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var source = snapshot.VisibleRows().Single(row => ReferenceEquals(row.Node, Entity(scene, 1)));

            var light = ListTreeBrightRows.Of(snapshot, source, Array.Empty<ListTreeCandidate>());

            Assert.Equal(new[] { Key(scene.Group<EntitiesGroup>(0)) }, light.PossibleTargets);
            Assert.True(light.IsBright(source.Key));
            Assert.True(light.IsBright(Key(Entity(scene, 0))));
        }

        [Fact]
        public void Of_AComposition_KeepsTheCompositionsBrightAndDimsTheEntities()
        {
            var scene = CreateScene();

            var light = LightOf(scene, scene.NodeA);

            Assert.True(light.IsBright(Key(scene.NodeB)));
            Assert.False(light.IsBright(Key(Entity(scene, 0))));
            Assert.False(light.IsBright(Key(EntityOfB(scene))));
        }
    }
}
