using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CoreMaskTexture = CMiX.Core.Texturing.MaskTexture;

namespace CMiX.Studio.Avalonia.Tests
{
    // The mask panel. Settings, Filter and Transform only make sense once the slot holds a texture.
    public class MaskTextureViewTests
    {
        private static List<object?> VisibleHeaders(CoreMaskTexture mask)
        {
            var view = new Views.MaskTexture { DataContext = mask };
            new Window { Content = view, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();

            return view.GetVisualDescendants().OfType<ExpanderIndented>()
                .Where(expander => expander.IsEffectivelyVisible)
                .Select(expander => expander.Header)
                .ToList();
        }

        private static CoreMaskTexture CreateMask() =>
            (CoreMaskTexture)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(CoreMaskTexture));

        [AvaloniaFact]
        public void EmptySlot_HidesSettingsFilterAndTransform()
        {
            var headers = VisibleHeaders(CreateMask());

            Assert.DoesNotContain("Settings", headers);
            Assert.DoesNotContain("Filter", headers);
            Assert.DoesNotContain("Transform", headers);
        }

        [AvaloniaFact]
        public void SlotWithATexture_ShowsSettingsFilterAndTransform()
        {
            var mask = CreateMask();
            mask.TextureManager.AddItem(typeof(Gradient));

            var headers = VisibleHeaders(mask);

            Assert.Contains("Settings", headers);
            Assert.Contains("Filter", headers);
            Assert.Contains("Transform", headers);
        }
    }
}
