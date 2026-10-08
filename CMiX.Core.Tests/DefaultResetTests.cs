using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // ControlFactory.Create gives each value the default of its model, so Reset restores that default.
    public class DefaultResetTests
    {
        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        private static void AssertResetRestores(GenericValue<float> value, float expected)
        {
            Assert.Equal(expected, value.Value);
            value.Value = 2f;
            Assert.Equal(2f, value.Value);
            value.Reset();
            Assert.Equal(expected, value.Value);
        }

        [Fact]
        public void CreateFromType_ResetRestoresTheModelDefault()
        {
            var text = (TextEntity)CreateFactory().Create(typeof(TextEntity));

            AssertResetRestores(text.Size, 0.8f);
            AssertResetRestores(text.LineHeight, 1.5f);
            AssertResetRestores(text.Width, 9.0f);
        }

        [Fact]
        public void LoadedModel_ResetRestoresTheDefault_NotTheSavedValue()
        {
            var source = (TextEntity)CreateFactory().Create(typeof(TextEntity));
            var model = (TextEntityModel)source.ToModel();
            model.Size = model.Size with { Value = 3f };

            var loaded = (TextEntity)CreateFactory().Create(model);

            Assert.Equal(3f, loaded.Size.Value);
            loaded.Size.Reset();
            Assert.Equal(0.8f, loaded.Size.Value);
        }

        [Fact]
        public void ConstructorDefault_StillWorks()
        {
            var blur = (Blur)CreateFactory().Create(typeof(Blur));

            Assert.Equal(0.5f, blur.Strength.Value);
            blur.Strength.Value = 0.9f;
            Assert.Equal(0.9f, blur.Strength.Value);
            blur.Strength.ResetCommand.Execute(null);
            Assert.Equal(0.5f, blur.Strength.Value);
        }

        // The child is built by a nested Create while the layer loads. It must not take the loaded layer values as defaults.
        [Fact]
        public void NestedCreate_DoesNotOverwriteTheParentDefaults()
        {
            var source = (Layer)CreateFactory().Create(typeof(Layer));
            source.LayerSettings.Opacity.Value = 0.5f;
            source.ModelEntityManager.AddItem(typeof(Entity));
            var model = (LayerModel)source.ToModel();
            Assert.Single(model.ModelEntityManager.ManagerData.Items);

            var loaded = (Layer)CreateFactory().Create(model);

            Assert.Single(loaded.ModelEntityManager.ManagerData.Items);
            Assert.Equal(0.5f, loaded.LayerSettings.Opacity.Value);
            loaded.LayerSettings.Opacity.Reset();
            Assert.Equal(1.0f, loaded.LayerSettings.Opacity.Value);
        }
    }
}
