using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CharWriterModifierTests
    {
        [Fact]
        public void CharWriterModifier_HasNoChannels()
        {
            var provider = TestServiceProviderFactory.Create();
            var charWriter = provider.GetRequiredService<CharWriterModifier>();

            Assert.Empty(charWriter.Channels);
        }

        [Fact]
        public void CharWriterModifier_IsDiscoverableOnTextEntity()
        {
            var attributes = typeof(CharWriterModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(TextEntity), owners);
        }

        [Fact]
        public void CharWriterModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var charWriter = provider.GetRequiredService<CharWriterModifier>();

            Assert.IsAssignableFrom<IModifier>(charWriter);
        }

        [Fact]
        public void CharWriterModifier_ToModel_FromModel_RoundTrips()
        {
            var provider = TestServiceProviderFactory.Create();
            var charWriter = provider.GetRequiredService<CharWriterModifier>();
            charWriter.ModulatorManager.AddItem(typeof(CMiX.Core.Animations.BeatModifier));

            var model = charWriter.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CharWriterModifier>();
            reloaded.FromModel(model);

            Assert.Single(reloaded.ModulatorManager.ManagerData.Items);
        }
    }
}
