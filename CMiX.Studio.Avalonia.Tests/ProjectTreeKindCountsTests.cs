using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row knows what it holds, by kind, so a collapsed row can tell the user.
    public class ProjectTreeKindCountsTests
    {
        // A composition with two layers. The first layer has two entities, one filter and three modifiers.
        private static Scene CreateScene() => TreeFixtures.CreateScene([2], scene => AddKinds(scene.Layer));

        [Fact]
        public void KindCounts_ALayer_CountsItsItemsByKindInTheOrderOfItsRows()
        {
            var scene = CreateScene();

            Assert.Equal(
                new[] { new KindCount(TreeCategory.Entity, 2), new KindCount(TreeCategory.Filter, 1), new KindCount(TreeCategory.Modifier, 3) },
                scene.LayerNode.KindCounts);
            Assert.True(scene.LayerNode.HasKindCounts);
        }

        [Fact]
        public void KindCounts_AComposition_CountsItsLayersAsComposables()
        {
            var scene = CreateScene();

            Assert.Equal(new[] { new KindCount(TreeCategory.Composable, 2) }, scene.CompositionNode.KindCounts);
        }

        [Fact]
        public void KindCounts_AGroup_CountsTheItemsOfItsKind()
        {
            var scene = CreateScene();

            Assert.Equal(new[] { new KindCount(TreeCategory.Entity, 2) }, Scene.Group<EntitiesGroup>(scene.LayerNode).KindCounts);
            Assert.Empty(Scene.Group<CamerasGroup>(scene.LayerNode).KindCounts);
            Assert.False(Scene.Group<CamerasGroup>(scene.LayerNode).HasKindCounts);
        }

        [Fact]
        public void KindCounts_AnEmptyRow_IsEmpty()
        {
            var scene = CreateScene();
            var secondLayer = scene.CompositionNode.Children[0].Children[1];

            Assert.Empty(secondLayer.KindCounts);
            Assert.False(secondLayer.HasKindCounts);
        }

        [Fact]
        public void Change_TheLists_UpdatesTheCountsOfTheParents()
        {
            var scene = CreateScene();
            var changes = new List<string?>();
            scene.LayerNode.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            scene.Layer.CameraManager.AddItem(typeof(CMiX.Core.Rendering.Cameras.Camera));

            Assert.Contains(nameof(TreeNode.KindCounts), changes);
            Assert.Contains(new KindCount(TreeCategory.Camera, 1), scene.LayerNode.KindCounts);

            scene.Layer.ModelEntityManager.RemoveItem(scene.Layer.ModelEntityManager.ManagerData.Items[0]);
            Assert.Contains(new KindCount(TreeCategory.Entity, 1), scene.LayerNode.KindCounts);

            scene.Layer.ModelEntityManager.ClearAll();
            Assert.DoesNotContain(scene.LayerNode.KindCounts, count => count.Category == TreeCategory.Entity);
        }

        [Fact]
        public void ShowCounts_ARow_IsTrueOnlyWhenCollapsedAndNotEmpty()
        {
            var scene = CreateScene();
            var changes = new List<string?>();
            scene.LayerNode.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            Assert.False(scene.LayerNode.ShowsKindCounts);

            scene.LayerNode.IsExpanded = false;
            Assert.True(scene.LayerNode.ShowsKindCounts);
            Assert.Contains(nameof(TreeNode.ShowsKindCounts), changes);

            var empty = scene.CompositionNode.Children[0].Children[1];
            empty.IsExpanded = false;
            Assert.False(empty.ShowsKindCounts);
        }

        [Fact]
        public void Add_ALayerToTheComposition_ChangesItsCount()
        {
            var scene = CreateScene();
            var changes = new List<string?>();
            scene.CompositionNode.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            scene.Composition.LayerManager.AddItem(typeof(Layer));

            Assert.Contains(nameof(TreeNode.HasKindCounts), changes);
            Assert.Equal(new KindCount(TreeCategory.Composable, 3), scene.CompositionNode.KindCounts.Single());
        }

        [Fact]
        public void KindCounts_AfterAChildIsAdded_RecomputesOnce()
        {
            var scene = CreateScene();
            var group = Scene.Group<EntitiesGroup>(scene.LayerNode);
            var groupBefore = group.KindCountsComputations;
            var layerBefore = scene.LayerNode.KindCountsComputations;
            var compositionBefore = scene.CompositionNode.KindCountsComputations;

            scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            _ = scene.LayerNode.KindCounts;
            _ = scene.LayerNode.HasKindCounts;
            _ = scene.LayerNode.ShowsKindCounts;

            Assert.Equal(groupBefore + 1, group.KindCountsComputations);
            Assert.Equal(layerBefore + 1, scene.LayerNode.KindCountsComputations);
            Assert.Equal(compositionBefore, scene.CompositionNode.KindCountsComputations);
            Assert.Contains(new KindCount(TreeCategory.Entity, 3), scene.LayerNode.KindCounts);
        }

        [Theory]
        [InlineData(TreeCategory.Composable, 1, "1 Composable")]
        [InlineData(TreeCategory.Composable, 4, "4 Composables")]
        [InlineData(TreeCategory.Entity, 1, "1 Entity")]
        [InlineData(TreeCategory.Entity, 2, "2 Entities")]
        [InlineData(TreeCategory.Camera, 1, "1 Camera")]
        [InlineData(TreeCategory.Camera, 3, "3 Cameras")]
        [InlineData(TreeCategory.Light, 1, "1 Light")]
        [InlineData(TreeCategory.Light, 2, "2 Lights")]
        [InlineData(TreeCategory.Texture, 1, "1 Texture")]
        [InlineData(TreeCategory.Texture, 2, "2 Textures")]
        [InlineData(TreeCategory.Filter, 1, "1 Filter")]
        [InlineData(TreeCategory.Filter, 5, "5 Filters")]
        [InlineData(TreeCategory.Modifier, 1, "1 Modifier")]
        [InlineData(TreeCategory.Modifier, 3, "3 Modifiers")]
        [InlineData(TreeCategory.Modulator, 1, "1 Modulator")]
        [InlineData(TreeCategory.Modulator, 2, "2 Modulators")]
        public void Describe_ACount_GivesTheNumberAndTheNameOfTheKind(TreeCategory category, int count, string expected)
        {
            Assert.Equal(expected, new KindCount(category, count).Description);
        }
    }
}
