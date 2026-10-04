using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Runs over every concrete texture source, so a new source cannot skip UseCompositionResolution.
    public class UseCompositionResolutionTests
    {
        public static IEnumerable<object[]> SourceTypeNames() =>
            typeof(TextureSourceBase).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(TextureSourceBase).IsAssignableFrom(t))
                .Select(t => new object[] { t.Name });

        private static Type SourceType(string name) =>
            typeof(TextureSourceBase).Assembly.GetTypes().Single(t => t.Name == name && typeof(TextureSourceBase).IsAssignableFrom(t));

        [Theory]
        [MemberData(nameof(SourceTypeNames))]
        public void Source_ExposesTheParameterThroughTheTextureSourceInterface(string name)
        {
            var type = SourceType(name);

            Assert.True(typeof(ITextureSource).IsAssignableFrom(type));
            Assert.Equal(typeof(TextureSourceBase), type.GetProperty(nameof(ITextureSource.UseCompositionResolution)).DeclaringType);
        }

        [Theory]
        [MemberData(nameof(SourceTypeNames))]
        public void Source_KeepsTheParameterThroughSaveAndLoad(string name)
        {
            var type = SourceType(name);
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

            var source = (TextureSourceBase)factory.Create(type);
            Assert.False(source.UseCompositionResolution.Value);
            source.UseCompositionResolution.Value = true;
            var model = source.ToModel();
            Assert.True(((ITextureSourceModel)model).UseCompositionResolution.Value);

            var loaded = (TextureSourceBase)factory.Create(type);
            loaded.FromModel(model);

            Assert.True(loaded.UseCompositionResolution.Value);
        }
    }
}
