using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class EntityMaterialCompositionIDTests
    {
        private static (Composition composition, Entity entity, ControlRepository repository) CreateEntity()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;

            return (composition, entity, provider.GetRequiredService<ControlRepository>());
        }

        [Fact]
        public void Entity_GivesItsCompositionIDToTheMaterialAndEverythingUnderIt()
        {
            var (composition, entity, _) = CreateEntity();

            entity.Material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            entity.Material.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            entity.Material.ModifierManager.AddItem(typeof(SelectTextureModifier));

            Assert.Equal(composition.ID, entity.Material.CompositionID);
            Assert.Equal(composition.ID, entity.Material.DiffuseTexture.CompositionID);
            Assert.Equal(composition.ID, entity.Material.MaskTexture.CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)entity.Material.DiffuseTexture.TextureManager.SelectedItem).CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)entity.Material.MaskTexture.TextureManager.SelectedItem).CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)entity.Material.ModifierManager.SelectedItem).CompositionID);
        }

        // Sees only the controls a manager registered, so the material subtree needs the test above.
        [Fact]
        public void EveryRegisteredControlThatHasACompositionIDReceivesIt()
        {
            var (composition, entity, repository) = CreateEntity();
            entity.Texture.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            entity.Texture.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            entity.ModifierManager.AddItem(typeof(SelectTextureModifier));

            var owned = repository.Controls.OfType<IHasCompositionID>().ToList();

            Assert.NotEmpty(owned);
            Assert.All(owned, control => Assert.Equal(composition.ID, control.CompositionID));
        }
    }
}
