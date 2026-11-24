using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class DisplaceTests
    {
        private static (ITextureSource Texture, Displace Filter) TextureWithDisplace(ControlFactory factory)
        {
            var texture = (ITextureSource)factory.Create(typeof(Gradient));
            texture.TextureModifierManager.AddItem(typeof(Displace));
            return (texture, (Displace)texture.TextureModifierManager.ManagerData.Items[0]);
        }

        [Fact]
        public void AddExistingItem_TheOwnTexture_IsRefused()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var (texture, filter) = TextureWithDisplace(factory);

            filter.TextureSelector.AddExistingItem(texture);

            Assert.Empty(filter.TextureSelector.ManagerData.Items);
        }

        [Fact]
        public void SelectedItem_TheOwnTexture_IsRefused()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var (texture, filter) = TextureWithDisplace(factory);

            filter.TextureSelector.SelectedItem = texture;

            Assert.Empty(filter.TextureSelector.ManagerData.Items);
            Assert.Null(filter.TextureSelector.SelectedItem);
        }

        [Fact]
        public void AddExistingItem_ATextureThatDisplacesWithTheOwner_IsRefused()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var (a, filterA) = TextureWithDisplace(factory);
            var (b, filterB) = TextureWithDisplace(factory);

            filterA.TextureSelector.AddExistingItem(b);
            filterB.TextureSelector.AddExistingItem(a);

            Assert.Same(b, Assert.Single(filterA.TextureSelector.ManagerData.Items));
            Assert.Empty(filterB.TextureSelector.ManagerData.Items);
        }

        [Fact]
        public void AddExistingItem_ATextureThatLoopsThroughAThirdOne_IsRefused()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var (a, filterA) = TextureWithDisplace(factory);
            var (b, filterB) = TextureWithDisplace(factory);
            var (c, filterC) = TextureWithDisplace(factory);

            filterA.TextureSelector.AddExistingItem(b);
            filterB.TextureSelector.AddExistingItem(c);
            filterC.TextureSelector.AddExistingItem(a);

            Assert.Empty(filterC.TextureSelector.ManagerData.Items);
        }

        [Fact]
        public void AddExistingItem_AnUnrelatedTexture_IsAccepted()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var (_, filter) = TextureWithDisplace(factory);
            var other = (ITextureSource)factory.Create(typeof(Gradient));

            filter.TextureSelector.AddExistingItem(other);

            Assert.Same(other, filter.TextureSelector.SelectedItem);
        }
    }
}
