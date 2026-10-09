using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Logging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The settings panels bind a Layer and a Composition by the same property names. A wrong name fails only at run time.
    public class ComposableSettingsPanelTests
    {
        [AvaloniaTheory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void SettingsPanels_BindWithoutErrors_AndShowTheModelValues(Type type)
        {
            var originalSink = Logger.Sink;
            var sink = new RecordingLogSink(originalSink);
            Logger.Sink = sink;

            try
            {
                var item = (IComposable)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);
                item.Compositing.Opacity.Value = 0.25f;
                item.Mask.Invert.Value = true;

                var panels = new StackPanel
                {
                    Children =
                    {
                        new Views.LayerManagerSettings { DataContext = item },
                        new Views.LayerManagerMaskSettings { DataContext = item }
                    }
                };
                new Window { Content = panels, Width = 400, Height = 600 }.Show();
                for (var i = 0; i < 5; i++)
                    Dispatcher.UIThread.RunJobs();

                var errors = sink.Entries
                    .Where(e => e.Area == LogArea.Binding && e.FlattenedText.Contains("Could not find a matching property accessor"))
                    .Select(e => e.FlattenedText)
                    .Distinct()
                    .ToList();
                Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));

                var alpha = panels.GetVisualDescendants().OfType<CMiXSlider>().Single(s => s.Caption == "Alpha");
                Assert.Equal(0.25, alpha.Value, 3);
                var invert = panels.GetVisualDescendants().OfType<CaptionedToggleButton>().Single(t => t.Caption == "Invert");
                Assert.True(invert.IsChecked);
            }
            finally
            {
                Logger.Sink = originalSink;
            }
        }
    }
}
