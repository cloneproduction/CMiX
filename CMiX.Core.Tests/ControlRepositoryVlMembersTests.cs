using System.Collections.ObjectModel;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The VL patches use these members and these interfaces.
    public class ControlRepositoryVlMembersTests
    {
        private static (ControlFactory Factory, ControlRepository Repository, CollectionManager CollectionManager, IServiceProvider Provider) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider.GetRequiredService<ControlFactory>(), provider.GetRequiredService<ControlRepository>(),
                    provider.GetRequiredService<CollectionManager>(), provider);
        }

        [Fact]
        public void Controls_OfTheRepository_IsAnObservableCollectionOfIControl()
        {
            var (_, repository, _, _) = Create();

            Assert.IsType<ObservableCollection<IControl>>(repository.Controls);
        }

        [Theory]
        [InlineData(typeof(ScaleModifier))]
        [InlineData(typeof(TranslateModifier))]
        [InlineData(typeof(RotationModifier))]
        [InlineData(typeof(ColorModifier))]
        [InlineData(typeof(TexCoordModifier))]
        [InlineData(typeof(TransformSRTModifier))]
        [InlineData(typeof(CircularSpreadModifier))]
        [InlineData(typeof(LinearModifier))]
        public void Type_OfASpreadableModifier_ImplementsISpreadableModifier(Type modifierType)
        {
            Assert.True(typeof(ISpreadableModifier).IsAssignableFrom(modifierType),
                $"{modifierType.Name} must implement ISpreadableModifier to stay eligible for VL-side Spread grouping, matching its old counterpart.");
        }

        [Theory]
        [InlineData(typeof(GridModifier))]
        public void Type_OfAGridModifier_ImplementsISpreadableModifier3(Type modifierType)
        {
            Assert.True(typeof(ISpreadableModifier3).IsAssignableFrom(modifierType),
                $"{modifierType.Name} must implement ISpreadableModifier3 to stay eligible for VL-side Spread grouping, matching its old counterpart.");
        }

        [Theory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void Type_OfALayerOrComposition_ImplementsIComposable(Type type)
        {
            Assert.True(typeof(IComposable).IsAssignableFrom(type),
                $"{type.Name} must implement IComposable so the VL side can use one set of nodes.");
        }

        [Theory]
        [InlineData(typeof(CompositingSettings))]
        [InlineData(typeof(Blend))]
        public void Type_OfABlendSetting_ImplementsIBlendable(Type type)
        {
            Assert.True(typeof(IBlendable).IsAssignableFrom(type),
                $"{type.Name} must implement IBlendable so the VL side can use one blend node.");
        }

        [Theory]
        [InlineData(typeof(MaskSettings))]
        [InlineData(typeof(MaskTexture))]
        public void Type_OfAMaskSetting_ImplementsIMaskable(Type type)
        {
            Assert.True(typeof(IMaskable).IsAssignableFrom(type),
                $"{type.Name} must implement IMaskable so the VL side can use one mask node.");
        }
    }
}
