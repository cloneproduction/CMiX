using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // A node of the tree knows its kind, and how many composables are around it.
    public class ProjectTreeCategoryTests
    {
        private static ProjectTree CreateTree()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.ManagerData.Items[0];
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.TextureModifierManager.AddItem(typeof(Blur));
            var entity = (Entity)layer.ModelEntityManager.ManagerData.Items[0];
            entity.ModifierManager.AddItem(typeof(TranslateModifier));
            ((CMiX.Core.Modulation.IModulatable)entity.ModifierManager.ManagerData.Items[0]).ModulatorManager.AddItem(typeof(LFOModulator));
            return TreeFixtures.CreateTree(provider);
        }

        private static TreeNode Child<T>(TreeNode node) where T : TreeGroup => node.Children.Single(child => child.Group is T);

        [Fact]
        public void Build_EveryRow_HasTheKindOfItsItems()
        {
            var composition = CreateTree().CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entities = Child<EntitiesGroup>(layer);
            var entity = entities.Children[0];
            var modifier = Child<ModifiersGroup>(entity).Children[0];

            Assert.Equal(TreeCategory.Composable, composition.Category);
            Assert.Equal(TreeCategory.Composable, layer.Category);
            Assert.Equal(TreeCategory.Entity, entities.Category);
            Assert.Equal(TreeCategory.Entity, entity.Category);
            Assert.Equal(TreeCategory.Camera, Child<CamerasGroup>(layer).Category);
            Assert.Equal(TreeCategory.Light, Child<LightsGroup>(layer).Category);
            Assert.Equal(TreeCategory.Filter, Child<FiltersGroup>(layer).Category);
            Assert.Equal(TreeCategory.Filter, Child<FiltersGroup>(layer).Children[0].Category);
            Assert.Equal(TreeCategory.Texture, Child<TexturesGroup>(entity).Category);
            Assert.Equal(TreeCategory.Modifier, Child<ModifiersGroup>(entity).Category);
            Assert.Equal(TreeCategory.Modifier, modifier.Category);
            Assert.Equal(TreeCategory.Modulator, Child<ModulatorsGroup>(modifier).Category);
            Assert.Equal(TreeCategory.Modulator, Child<ModulatorsGroup>(modifier).Children[0].Category);
        }

        [Fact]
        public void ComposableDepth_ARow_CountsTheComposablesAroundIt()
        {
            var composition = CreateTree().CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entity = Child<EntitiesGroup>(layer).Children[0];

            Assert.Equal(0, composition.ComposableDepth);
            Assert.Equal(1, layer.ComposableDepth);
            Assert.Equal(2, Child<EntitiesGroup>(layer).ComposableDepth);
            Assert.Equal(2, entity.ComposableDepth);
        }

        [Fact]
        public void KindDepth_ARow_CountsTheChangesOfKindDownToIt()
        {
            var composition = CreateTree().CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entities = Child<EntitiesGroup>(layer);
            var entity = entities.Children[0];
            var modifiers = Child<ModifiersGroup>(entity);
            var modifier = modifiers.Children[0];
            var modulators = Child<ModulatorsGroup>(modifier);

            Assert.Equal(0, composition.KindDepth);
            Assert.Equal(0, layer.KindDepth);
            Assert.Equal(1, entities.KindDepth);
            Assert.Equal(1, entity.KindDepth);
            Assert.Equal(2, modifiers.KindDepth);
            Assert.Equal(2, modifier.KindDepth);
            Assert.Equal(3, modulators.KindDepth);
            Assert.Equal(3, modulators.Children[0].KindDepth);
            Assert.Equal(1, Child<FiltersGroup>(layer).KindDepth);
            Assert.Equal(1, Child<FiltersGroup>(layer).Children[0].KindDepth);
        }

        [Fact]
        public void Add_ALayerLater_GivesItTheKindAndTheDepthOfALayer()
        {
            var tree = CreateTree();
            var composition = tree.CompositionNodes()[0];
            var source = (Composition)composition.Item!;

            source.LayerManager.AddItem(typeof(Layer));

            var added = composition.Children[0].Children.Last();
            Assert.Equal(TreeCategory.Composable, added.Category);
            Assert.Equal(1, added.ComposableDepth);
        }
    }
}
