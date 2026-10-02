using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers the CompositionID propagation gap on Entity.Texture and FullscreenQuad.Texture:
    // neither used to cascade into their own Texture field, so a texture source placed there
    // silently stayed at Guid.Empty forever.
    public class CompositionIDTests
    {
        [Fact]
        public void Entity_PropagatesCompositionIDIntoATextureSourceUnderItsOwnTextureField()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;

            entity.Texture.DiffuseTexture.TextureManager.AddItem(typeof(BubbleNoise));
            var bubbleNoise = (BubbleNoise)entity.Texture.DiffuseTexture.TextureManager.SelectedItem;

            Assert.Equal(composition.ID, bubbleNoise.CompositionID);
        }

        [Fact]
        public void FullscreenQuad_PropagatesCompositionIDIntoATextureSourceUnderItsOwnTextureField()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            layer.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            var fullscreenQuad = (FullscreenQuad)layer.ModelEntityManager.SelectedItem;

            fullscreenQuad.Texture.DiffuseTexture.TextureManager.AddItem(typeof(BubbleNoise));
            var bubbleNoise = (BubbleNoise)fullscreenQuad.Texture.DiffuseTexture.TextureManager.SelectedItem;

            Assert.Equal(composition.ID, bubbleNoise.CompositionID);
        }

        [Fact]
        public void PrefabManager_GivesOnlyTheLastCompositionIDToAnItemAddedLater()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();

            manager.CompositionID = first;
            manager.CompositionID = second;
            var item = new RecordingCompositionIDControl();
            manager.Collection.AddItem(item);

            Assert.Equal(new[] { second }, item.ReceivedIDs);
        }

        [Fact]
        public void PrefabManager_GivesTheCompositionIDToAnItemAlreadyInTheManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(Layer));
            var layer = (Layer)manager.SelectedItem;
            var id = Guid.NewGuid();

            manager.CompositionID = id;

            Assert.Equal(id, layer.CompositionID);
            Assert.Equal(id, layer.ModifierManager.CompositionID);
        }

        // Records every composition id that it receives.
        private class RecordingCompositionIDControl : IControl, IHasCompositionID
        {
            private Guid _compositionID;

            public List<Guid> ReceivedIDs { get; } = new();
            public Guid ID { get; set; } = Guid.NewGuid();

            public Guid CompositionID
            {
                get => _compositionID;
                set
                {
                    _compositionID = value;
                    ReceivedIDs.Add(value);
                }
            }

            public IControlModel ToModel() => null;
            public void FromModel(IControlModel model) { }
        }
    }
}
