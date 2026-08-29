using System.Text.Json;
using CMiX.Core.Persistence;
using CMiX.Core.Transformation.Modifiers;
using Xunit;

namespace CMiX.Core.Tests
{
    // GridModel/CircularSpreadModel/LinearXYZModel were renamed to *LegacyModel when Grid/
    // CircularSpread/LinearXYZ were renamed to *Legacy. $type is written as the concrete type's
    // FullName, so a project saved before that rename has "$type":"...GridModel" (etc) in its
    // JSON - without a legacy-name alias, deserializing it throws "Unknown type" and the whole
    // project fails to load.
    public class IControlModelJsonConverterLegacyTypeNameTests
    {
        [Fact]
        public void Deserialize_OldGridModelTypeName_ResolvesToGridLegacyModel()
        {
            var json = """
                {
                    "$type": "CMiX.Core.Transformation.Modifiers.GridModel",
                    "ID": "11111111-1111-1111-1111-111111111111"
                }
                """;

            var model = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            Assert.IsType<GridLegacyModel>(model);
            Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), model!.ID);
        }

        [Fact]
        public void Deserialize_OldCircularSpreadModelTypeName_ResolvesToCircularSpreadLegacyModel()
        {
            var json = """
                {
                    "$type": "CMiX.Core.Transformation.Modifiers.CircularSpreadModel",
                    "ID": "22222222-2222-2222-2222-222222222222"
                }
                """;

            var model = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            Assert.IsType<CircularSpreadLegacyModel>(model);
            Assert.Equal(Guid.Parse("22222222-2222-2222-2222-222222222222"), model!.ID);
        }

        [Fact]
        public void Deserialize_OldLinearXYZModelTypeName_ResolvesToLinearXYZLegacyModel()
        {
            var json = """
                {
                    "$type": "CMiX.Core.Transformation.Modifiers.LinearXYZModel",
                    "ID": "33333333-3333-3333-3333-333333333333"
                }
                """;

            var model = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options);

            Assert.IsType<LinearXYZLegacyModel>(model);
            Assert.Equal(Guid.Parse("33333333-3333-3333-3333-333333333333"), model!.ID);
        }
    }
}
