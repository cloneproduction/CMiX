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
    // The engine does not use the update switch yet, so the row of a layer or a composition does not show it.
    public class ComposableItemUpdateToggleTests
    {
        [AvaloniaFact]
        public void TheRowOfALayer_ShowsTheVisibilityAndTheMaskToggles_ButNotTheUpdateToggle()
        {
            var layer = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var row = new Views.ComposableManagerItem { DataContext = layer };
            new Window { Content = row, Width = 400, Height = 60 }.Show();
            Dispatcher.UIThread.RunJobs();

            var visible = row.GetVisualDescendants().OfType<ToggleButton>().Where(toggle => toggle.IsEffectivelyVisible).ToList();

            Assert.Equal(2, visible.Count);
        }
    }
}
