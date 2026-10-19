using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers the FullscreenQuad texture-manager leak: FullscreenQuad owns a Texture with two
    // nested slot managers, the same shape Entity owns, and needed the same Dispose fix.
    public class FullscreenQuadTests
    {
        [Fact]
        public void DeleteThenDropTheUndoEntry_DisposesBothTextureSlotManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            var heldBeforeTheQuad = HeldCollectionManagers.Count(repository);

            layer.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            var quad = (FullscreenQuad)layer.ModelEntityManager.SelectedItem;
            quad.Texture.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            quad.Texture.MaskTexture.TextureManager.AddItem(typeof(CheckerBoard));
            var textures = quad.Texture.DiffuseTexture.TextureManager.ManagerData.Items
                .Concat(quad.Texture.MaskTexture.TextureManager.ManagerData.Items)
                .ToList();

            Assert.True(HeldCollectionManagers.Count(repository) > heldBeforeTheQuad);

            layer.ModelEntityManager.DeleteItem(quad);
            undoManager.Clear();

            Assert.Null(repository.GetControl(quad.ID));
            Assert.All(textures, texture => Assert.Null(repository.GetControl(texture.ID)));
            Assert.Equal(heldBeforeTheQuad, HeldCollectionManagers.Count(repository));
        }
    }
}
