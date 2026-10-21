using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The add menus do not offer the types that do not work yet. A project can still hold them.
    public class HiddenTypesTests
    {
        private static List<object?> Offered(AutoSelectionPanel panel) =>
            ((Grid)panel.Content!).Children
                .OfType<StackPanel>()
                .SelectMany(stack => stack.Children.OfType<Button>())
                .Select(button => button.CommandParameter)
                .ToList();

        [AvaloniaFact]
        public void TheLayerMenu_DoesNotOfferTheRenderModifiers()
        {
            var offered = Offered(new AutoSelectionPanel { PanelOwner = typeof(Layer) });

            Assert.DoesNotContain(typeof(RenderRandomEntityModifier), offered);
            Assert.DoesNotContain(typeof(RenderSequenceEntityModifier), offered);
            Assert.Contains(typeof(AlphaModifier), offered);
        }

        [AvaloniaFact]
        public void TheEntityMenu_DoesNotOfferBillboard()
        {
            var offered = Offered(new AutoSelectionPanel { PanelOwner = typeof(Entity) });

            Assert.DoesNotContain(typeof(BillboardModifier), offered);
            Assert.Contains(typeof(TranslateModifier), offered);
        }

        [AvaloniaFact]
        public void TheModulatorMenu_DoesNotOfferTheRedisAddressModulator()
        {
            var offered = Offered(new AutoSelectionPanel { Interface = typeof(IModulator) });

            Assert.DoesNotContain(typeof(RedisAddressModulator), offered);
            Assert.Contains(typeof(RandomModulator), offered);
        }

        [Theory]
        [InlineData(typeof(RenderRandomEntityModifier))]
        [InlineData(typeof(RenderSequenceEntityModifier))]
        [InlineData(typeof(BillboardModifier))]
        [InlineData(typeof(RedisAddressModulator))]
        public void AHiddenType_CanStillBeCreated(Type type)
        {
            var provider = TestServiceProviderFactory.Create();

            var control = provider.GetRequiredService<ControlFactory>().Create(type);

            Assert.IsType(type, control);
        }
    }
}
