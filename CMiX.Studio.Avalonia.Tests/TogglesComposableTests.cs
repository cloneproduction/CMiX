using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The visibility and mask toggles of an item, on their squares.
    public class TogglesComposableTests
    {
        private static object Create(Type type) =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

        private static TogglesComposable Show(object? item, string? size = null, Action<Window>? setup = null)
        {
            var toggles = new TogglesComposable { DataContext = item };
            if (size != null)
                toggles.Classes.Add(size);
            var window = new Window { Width = 300, Height = 100 };
            setup?.Invoke(window);
            window.Content = toggles;
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return toggles;
        }

        private static List<Border> Slots(TogglesComposable toggles) =>
            toggles.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot") && border.IsEffectivelyVisible).ToList();

        private static ToggleButton? Visible(Border slot) =>
            slot.GetVisualDescendants().OfType<ToggleButton>().SingleOrDefault(button => button.IsEffectivelyVisible);

        private static ToggleButton ToggleWithTheme(TogglesComposable toggles, string themeKey) =>
            toggles.GetVisualDescendants().OfType<ToggleButton>()
                .Single(button => button.IsEffectivelyVisible && ReferenceEquals(button.Theme, Application.Current!.FindResource(themeKey)));

        private static Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        [AvaloniaFact]
        public void Show_ALayerRow_HasAVisibilityAndAMaskToggleWithNoFill()
        {
            var slots = Slots(Show(Create(typeof(Layer)), "row", window => window.Resources["TreeRowHeight"] = 24.0));

            Assert.Equal(2, slots.Count);
            Assert.Same(Application.Current!.FindResource("ToggleButtonVisibility"), Visible(slots[0])!.Theme);
            Assert.Same(Application.Current!.FindResource("ToggleButtonMask"), Visible(slots[1])!.Theme);
            Assert.All(slots, slot => Assert.Equal(global::Avalonia.Media.Colors.Transparent, ((ISolidColorBrush)slot.Background!).Color));
        }

        [AvaloniaFact]
        public void Show_AnEntity_HasAVisibilityToggleAndAnEmptyMaskSlot()
        {
            var slots = Slots(Show(Create(typeof(Entity))));

            Assert.Equal(2, slots.Count);
            Assert.NotNull(Visible(slots[0]));
            Assert.Null(Visible(slots[1]));
            Assert.Equal(Brush("PanelBackgroundLevel2Brush"), ((ISolidColorBrush)slots[0].Background!).Color);
            Assert.Equal(Brush("Grey100Brush"), ((ISolidColorBrush)slots[1].Background!).Color);
        }

        [AvaloniaFact]
        public void Show_AGroup_HasTwoEmptySlots()
        {
            var slots = Slots(Show(null));

            Assert.Equal(2, slots.Count);
            Assert.All(slots, slot =>
            {
                Assert.Null(Visible(slot));
                Assert.Equal(Brush("Grey100Brush"), ((ISolidColorBrush)slot.Background!).Color);
            });
        }

        [AvaloniaFact]
        public void VisibilityToggle_Toggles_TheVisibilityValue()
        {
            var layer = (Layer)Create(typeof(Layer));
            var toggle = ToggleWithTheme(Show(layer), "ToggleButtonVisibility");
            var before = layer.PrefabService.Visibility.Value;

            toggle.IsChecked = !before;

            Assert.Equal(!before, layer.PrefabService.Visibility.Value);
        }

        [AvaloniaFact]
        public void MaskToggle_Toggles_TheMaskValue()
        {
            var layer = (Layer)Create(typeof(Layer));
            var toggle = ToggleWithTheme(Show(layer), "ToggleButtonMask");
            var before = layer.Mask.IsMask.Value;

            toggle.IsChecked = !before;

            Assert.Equal(!before, layer.Mask.IsMask.Value);
        }

        [AvaloniaFact]
        public void Slots_OfTheRowSize_MeasureTreeRowHeight()
        {
            // A value that no other token has, so the test shows that the row size reads TreeRowHeight.
            var slots = Slots(Show(Create(typeof(Layer)), "row", window => window.Resources["TreeRowHeight"] = 31.0));

            Assert.All(slots, slot =>
            {
                Assert.Equal(31.0, slot.Bounds.Width, 3);
                Assert.Equal(31.0, slot.Bounds.Height, 3);
            });
        }

        [AvaloniaFact]
        public void Slots_OfTheSmallSize_MeasureTheListSize()
        {
            var toggles = Show(Create(typeof(Layer)), "small");
            var slots = Slots(toggles);
            var size = Convert.ToDouble(Application.Current!.FindResource("ControlHeightXS"));

            Assert.Equal(2, slots.Count);
            Assert.All(slots, slot =>
            {
                Assert.Equal(size, slot.Bounds.Width, 3);
                Assert.Equal(size, slot.Bounds.Height, 3);
                Assert.Equal(Colors.Transparent, ((ISolidColorBrush)slot.Background!).Color);
            });
            var mask = ToggleWithTheme(toggles, "ToggleButtonMask");
            var visibility = ToggleWithTheme(toggles, "ToggleButtonVisibility");
            Assert.True(mask.TranslatePoint(new Point(mask.Bounds.Width, 0), toggles)!.Value.X <= visibility.TranslatePoint(new Point(0, 0), toggles)!.Value.X);
        }
    }
}
