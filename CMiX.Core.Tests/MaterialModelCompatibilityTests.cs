using System.Text.Json;
using CMiX.Core.Materials;
using CMiX.Core.Persistence;
using Xunit;

namespace CMiX.Core.Tests
{
    // A material has no modifiers and no explode fields any more. A project file that was saved before still has the keys, and it must load.
    public class MaterialModelCompatibilityTests
    {
        [Fact]
        public void AProjectFileWithTheOldModifiersOfAMaterial_StillLoads()
        {
            var json = JsonSerializer.Serialize(new MaterialModel(), ProjectSerializer.Options);
            var withOldKey = json.Insert(json.LastIndexOf('}'),
                ",\"ModifierManager\":{\"Items\":[{\"$type\":\"CMiX.Core.Modulation.Modifiers.SelectTextureModifierModel\"}]}");

            var loaded = JsonSerializer.Deserialize<MaterialModel>(withOldKey, ProjectSerializer.Options);

            Assert.NotNull(loaded);
        }

        [Fact]
        public void AProjectFileWithTheOldExplodeFieldsOfAMaterial_StillLoads()
        {
            var json = JsonSerializer.Serialize(new MaterialModel(), ProjectSerializer.Options);
            var withOldKeys = json.Insert(json.LastIndexOf('}'),
                ",\"ExplodeTriangleTextureManager\":{\"Items\":[]},\"ExplodeStrength\":{\"Value\":0.6}");

            var loaded = JsonSerializer.Deserialize<MaterialModel>(withOldKeys, ProjectSerializer.Options);

            Assert.NotNull(loaded);
        }

        [Fact]
        public void ASavedMaterial_HasNoExplodeFields()
        {
            var json = JsonSerializer.Serialize(new MaterialModel(), ProjectSerializer.Options);

            Assert.DoesNotContain("Explode", json);
        }

        [Fact]
        public void ASavedMaterial_HasNoModifiers()
        {
            var json = JsonSerializer.Serialize(new MaterialModel(), ProjectSerializer.Options);

            Assert.DoesNotContain("ModifierManager", json);
        }
    }
}
