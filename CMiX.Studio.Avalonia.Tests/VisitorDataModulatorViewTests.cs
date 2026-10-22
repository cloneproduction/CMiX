using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewVisitorDataModulator = CMiX.Studio.Avalonia.Views.VisitorDataModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The ranges of a VisitorDataModulator in the window.
    public class VisitorDataModulatorViewTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private VisitorDataModulator NewModulator()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(VisitorDataModulator));
            return (VisitorDataModulator)blur.ModulatorManager.SelectedItem;
        }

        private static ViewVisitorDataModulator Show(VisitorDataModulator modulator)
        {
            var view = new ViewVisitorDataModulator { DataContext = modulator };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static List<RangeValue> RangeValues(Control view) =>
            view.GetVisualDescendants().OfType<RangeValue>().ToList();

        [AvaloniaFact]
        public void Show_AModulator_HasOneRangeValuePerFloatValue()
        {
            var view = Show(NewModulator());

            Assert.Equal(6, RangeValues(view).Count);
            Assert.Equal(7, view.GetVisualDescendants().OfType<ContentCaptioned>().Count());
        }

        [AvaloniaFact]
        public void Show_AFreshModulator_ShowsTheDefaults()
        {
            var view = Show(NewModulator());

            var ranges = RangeValues(view);
            Assert.Equal(0.0, ranges.First().LeftValue);
            Assert.Equal(1.0, ranges.First().RightValue);
            Assert.Equal(1.0, ranges.Last().LeftValue);
            Assert.Equal(-1.0, ranges.Last().RightValue);
        }

        [AvaloniaFact]
        public void SetValue_OnARangeValue_ChangesTheRange()
        {
            var modulator = NewModulator();
            var view = Show(modulator);

            var heartRate = RangeValues(view).First();
            heartRate.LeftValue = 40.0;
            heartRate.RightValue = 200.0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(40f, modulator.HeartRateRange.Minimum.Value);
            Assert.Equal(200f, modulator.HeartRateRange.Maximum.Value);
        }
    }
}
