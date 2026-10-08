using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The right-click menu of a filter slider must reset the value to its default.
    public class FilterSliderResetTests
    {
        [AvaloniaFact]
        public void SliderResetMenu_RestoresTheDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var hscb = (HSCB)provider.GetRequiredService<ControlFactory>().Create(typeof(HSCB));
            var view = new Views.HSCB { DataContext = hscb };
            var window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            hscb.Saturation.Value = 0.3f;
            var slider = view.GetVisualDescendants().OfType<CMiXSlider>().Single(c => c.DataContext == hscb.Saturation);
            var reset = slider.ContextMenu!.Items.OfType<MenuItem>().Single();

            Assert.NotNull(reset.Command);
            reset.Command.Execute(null);

            Assert.Equal(1.0f, hscb.Saturation.Value);
        }
    }
}
