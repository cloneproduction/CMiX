using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The right-click menu of a filter value must reset it to its default.
    public class FilterValueResetTests
    {
        [AvaloniaFact]
        public void DragValueResetMenu_RestoresTheDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var hscb = (HSCB)provider.GetRequiredService<ControlFactory>().Create(typeof(HSCB));
            var view = new Views.HSCB { DataContext = hscb };
            var window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            hscb.Saturation.Value = 0.3f;
            var value = view.GetVisualDescendants().OfType<DragValue>().Single(c => c.DataContext == hscb.Saturation);
            var reset = value.ContextMenu!.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "Reset"));

            Assert.NotNull(reset.Command);
            reset.Command.Execute(null);

            Assert.Equal(1.0f, hscb.Saturation.Value);
        }
    }
}
