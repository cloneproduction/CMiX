using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The row of a layer or a composition has two toggles: the visibility and the mask.
    public class ComposableItemTogglesTests
    {
        [AvaloniaFact]
        public void TheRowOfALayer_ShowsTheVisibilityAndTheMaskToggles()
        {
            var layer = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var row = new Views.RowComposable { DataContext = layer };
            new Window { Content = row, Width = 400, Height = 60 }.Show();
            Dispatcher.UIThread.RunJobs();

            var visible = row.GetVisualDescendants().OfType<ToggleButton>().Where(toggle => toggle.IsEffectivelyVisible).ToList();

            Assert.Equal(2, visible.Count);
        }

        [AvaloniaFact]
        public void TheVisibilityToggle_IsOnTheFarRight_AndTheMaskToggleIsLeftOfIt()
        {
            var layer = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var row = new Views.RowComposable { DataContext = layer };
            var window = new Window { Content = row, Width = 400, Height = 60 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var toggles = row.GetVisualDescendants().OfType<ToggleButton>().Where(toggle => toggle.IsEffectivelyVisible).ToList();
            var visibility = toggles.Single(toggle => ReferenceEquals(toggle.Theme, Application.Current!.FindResource("ToggleButtonVisibility")));
            var mask = toggles.Single(toggle => ReferenceEquals(toggle.Theme, Application.Current!.FindResource("ToggleButtonMask")));

            var visibilityLeft = visibility.TranslatePoint(new Point(0, 0), window)!.Value.X;
            var maskLeft = mask.TranslatePoint(new Point(0, 0), window)!.Value.X;
            Assert.True(maskLeft + mask.Bounds.Width <= visibilityLeft, "The mask toggle is not left of the visibility toggle.");
        }
    }
}
