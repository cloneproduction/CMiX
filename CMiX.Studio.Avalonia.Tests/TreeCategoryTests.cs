using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using AvaloniaColor = Avalonia.Media.Color;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Image = CMiX.Core.Texturing.Sources.Image;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Every kind of row of the tree has a kind and a color.
    public class TreeCategoryTests
    {
        [Theory]
        [InlineData(typeof(Composition), TreeCategory.Composable)]
        [InlineData(typeof(Layer), TreeCategory.Composable)]
        [InlineData(typeof(Entity), TreeCategory.Entity)]
        [InlineData(typeof(FullscreenQuad), TreeCategory.Entity)]
        [InlineData(typeof(TextEntity), TreeCategory.Entity)]
        [InlineData(typeof(Camera), TreeCategory.Camera)]
        [InlineData(typeof(LightEntity), TreeCategory.Light)]
        [InlineData(typeof(Image), TreeCategory.Texture)]
        [InlineData(typeof(Blur), TreeCategory.Filter)]
        [InlineData(typeof(TranslateModifier), TreeCategory.Modifier)]
        [InlineData(typeof(LFOModulator), TreeCategory.Modulator)]
        public void Category_AnItem_IsTheKindOfItsType(System.Type type, TreeCategory expected)
        {
            var item = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

            Assert.Equal(expected, TreeCategories.Of(item));
        }

        [Fact]
        public void Category_AGroup_IsTheKindOfItsItems()
        {
            var provider = TestServiceProviderFactory.Create();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var entity = (Entity)provider.GetRequiredService<ControlFactory>().Create(typeof(Entity));

            Assert.Equal(TreeCategory.Entity, new EntitiesGroup(layer, layer.ModelEntityManager).Category);
            Assert.Equal(TreeCategory.Camera, new CamerasGroup(layer, layer.CameraManager).Category);
            Assert.Equal(TreeCategory.Light, new LightsGroup(layer, layer.LightManager).Category);
            Assert.Equal(TreeCategory.Texture, new TexturesGroup(entity, entity.Texture.DiffuseTexture.TextureManager).Category);
            Assert.Equal(TreeCategory.Filter, new FiltersGroup(layer, layer.TextureModifierManager).Category);
            Assert.Equal(TreeCategory.Modifier, new ModifiersGroup(entity, entity.ModifierManager).Category);
            Assert.Equal(TreeCategory.Modulator, new ModulatorsGroup(entity, entity.ModifierManager).Category);
        }

        [AvaloniaFact]
        public void ColorKey_EveryKindAndLevel_GivesAColorPerLevelForAComposable()
        {
            foreach (var category in System.Enum.GetValues<TreeCategory>())
                Assert.IsType<AvaloniaColor>(Application.Current!.FindResource("Category" + category));

            for (var level = 0; level < 6; level++)
                Assert.IsType<AvaloniaColor>(Application.Current!.FindResource("CategoryComposable" + level));
        }
    }
}
