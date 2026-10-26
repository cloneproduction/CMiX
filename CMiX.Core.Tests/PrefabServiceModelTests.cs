using System.Text.Json;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using Xunit;

namespace CMiX.Core.Tests
{
    // A prefab keeps a name and a visible flag. The old selected flag and the old update flag are gone.
    public class PrefabServiceModelTests
    {
        [Fact]
        public void ASavedPrefabService_HasNoSelectedFlagAndNoUpdateFlag()
        {
            var json = JsonSerializer.Serialize(new PrefabServiceModel(), ProjectSerializer.Options);

            Assert.DoesNotContain("IsSelected", json);
            Assert.DoesNotContain("IsUpdating", json);
            Assert.Contains("Visibility", json);
        }

        [Fact]
        public void AProjectFileWithTheOldFlags_StillLoads()
        {
            var json = JsonSerializer.Serialize(new PrefabServiceModel(), ProjectSerializer.Options);
            var withOldKey = json.Insert(json.LastIndexOf('}'), ",\"IsSelected\":{\"ID\":\"11223344-5566-7788-99aa-bbccddeeff5d\",\"Value\":false},\"IsUpdating\":{\"ID\":\"11223344-5566-7788-99aa-bbccddeeff60\",\"Value\":true}");

            var loaded = JsonSerializer.Deserialize<PrefabServiceModel>(withOldKey, ProjectSerializer.Options);

            Assert.NotNull(loaded);
        }
    }
}
