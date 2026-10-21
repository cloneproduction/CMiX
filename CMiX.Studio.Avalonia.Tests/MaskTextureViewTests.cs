using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Enum;
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

        private static ExpanderIndented ExpanderWithHeader(Control view, string header) =>
            view.GetVisualDescendants().OfType<ExpanderIndented>().Single(expander => Equals(expander.Header, header));

        private static (CoreMaskTexture Mask, Views.MaskTexture View) ShowWithATexture()
        {
            var mask = CreateMask();
            mask.TextureManager.AddItem(typeof(Gradient));
            var view = new Views.MaskTexture { DataContext = mask };
            new Window { Content = view, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (mask, view);
        }

        private static double TopOf(Control control, Control view) => control.TranslatePoint(new Point(0, 0), view)!.Value.Y;

        private static (ComboBoxCaptioned MaskChannel, ToggleButtonCaptioned Invert, PrefabSlotManager Slot) SlotAndMaskControls(Control view) =>
            (view.GetVisualDescendants().OfType<ComboBoxCaptioned>().Single(combo => combo.Caption == "MaskChannel"),
             view.GetVisualDescendants().OfType<ToggleButtonCaptioned>().Single(toggle => toggle.Caption == "Invert"),
             view.GetVisualDescendants().OfType<PrefabSlotManager>().Single());

        [AvaloniaFact]
        public void MaskChannelAndInvert_SitUnderTheSlot_AndAboveSettingsFilterAndTransform()
        {
            var (_, view) = ShowWithATexture();
            var (maskChannel, invert, slot) = SlotAndMaskControls(view);
            var below = new Control[] { ExpanderWithHeader(view, "Settings"), ExpanderWithHeader(view, "Filter"), ExpanderWithHeader(view, "Transform") };

            Assert.True(TopOf(slot, view) < TopOf(maskChannel, view));
            Assert.True(TopOf(maskChannel, view) < TopOf(invert, view));
            Assert.All(below, control => Assert.True(TopOf(invert, view) < TopOf(control, view)));
        }

        [AvaloniaFact]
        public void EmptySlot_StillShowsMaskChannelAndInvertUnderTheSlot()
        {
            var mask = CreateMask();
            var view = new Views.MaskTexture { DataContext = mask };
            new Window { Content = view, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
            var (maskChannel, invert, slot) = SlotAndMaskControls(view);

            Assert.True(maskChannel.IsEffectivelyVisible);
            Assert.True(invert.IsEffectivelyVisible);
            Assert.True(TopOf(slot, view) < TopOf(maskChannel, view));
        }

        [AvaloniaFact]
        public void SettingsFilterAndTransform_HaveTheSameIndentation()
        {
            var (_, view) = ShowWithATexture();

            var indentations = new[] { "Settings", "Filter", "Transform" }
                .Select(header => ExpanderWithHeader(view, header).Indentation)
                .Distinct()
                .ToList();

            Assert.Equal(new[] { 4 }, indentations);
        }

        [AvaloniaFact]
        public void Transform_HasItsOwnExpandedState_ApartFromTheMaskExpander()
        {
            var mask = CreateMask();
            mask.TextureManager.AddItem(typeof(Gradient));
            mask.IsExpanded = true;
            var view = new Views.MaskTexture { DataContext = mask };
            new Window { Content = view, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
            var transform = ExpanderWithHeader(view, "Transform");

            Assert.False(transform.IsExpanded);

            transform.IsExpanded = true;
            transform.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();

            Assert.True(mask.IsExpanded);
            Assert.False(mask.TransformIsExpanded);
        }

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
