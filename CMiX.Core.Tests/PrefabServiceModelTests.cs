using System.Text.Json;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Xunit;

namespace CMiX.Core.Tests
{
    // A prefab keeps a name, a visible flag and an update flag. The old selected flag is gone.
    public class PrefabServiceModelTests
    {
        [Fact]
        public void ASavedPrefabService_HasNoSelectedFlag()
        {
            var json = JsonSerializer.Serialize(new PrefabServiceModel(), ProjectSerializer.Options);

            Assert.DoesNotContain("IsSelected", json);
            Assert.Contains("Visibility", json);
            Assert.Contains("IsUpdating", json);
        }

        [Fact]
        public void AProjectFileWithTheOldSelectedFlag_StillLoads()
        {
            var json = JsonSerializer.Serialize(new PrefabServiceModel(), ProjectSerializer.Options);
            var withOldKey = json.Insert(json.LastIndexOf('}'), ",\"IsSelected\":{\"ID\":\"11223344-5566-7788-99aa-bbccddeeff5d\",\"Value\":false}");

            var loaded = JsonSerializer.Deserialize<PrefabServiceModel>(withOldKey, ProjectSerializer.Options);

            Assert.NotNull(loaded);
        }
    }
}
