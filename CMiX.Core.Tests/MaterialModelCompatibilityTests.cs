using System.Text.Json;
using CMiX.Core.Materials;
using CMiX.Core.Persistence;
using Xunit;

namespace CMiX.Core.Tests
{
    // A material has no modifiers and no explode fields any more.
    public class MaterialModelCompatibilityTests
    {
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
