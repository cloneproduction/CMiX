using CMiX.Core.Materials;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class MaterialSettingsTests
    {
        [Fact]
        public void MaterialSettings_KeepsMetalnessAndRoughnessApartThroughSaveAndLoad()
        {
            var provider = TestServiceProviderFactory.Create();
            var source = provider.GetRequiredService<MaterialSettings>();
            var loaded = provider.GetRequiredService<MaterialSettings>();

            source.Metalness.Value = 0.2f;
            source.Roughness.Value = 0.8f;
            loaded.FromModel(source.ToModel());

            Assert.Equal(0.2f, loaded.Metalness.Value);
            Assert.Equal(0.8f, loaded.Roughness.Value);
            Assert.Equal(source.Roughness.ID, loaded.Roughness.ID);
            Assert.NotEqual(loaded.Metalness.ID, loaded.Roughness.ID);
        }
    }
}
