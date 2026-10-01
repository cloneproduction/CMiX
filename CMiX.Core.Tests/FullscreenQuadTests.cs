using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
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

            layer.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            var quad = (FullscreenQuad)layer.ModelEntityManager.SelectedItem;

            var deleterCountBeforeDelete = repository.DeleterCount;

            layer.ModelEntityManager.DeleteItem(quad);
            undoManager.Clear();

            Assert.Null(repository.GetControl(quad.ID));
            // The quad's two texture slot managers, diffuse and mask, are disposed along with it,
            // each unregistering its own deleter.
            Assert.Equal(deleterCountBeforeDelete - 2, repository.DeleterCount);
        }
    }
}
