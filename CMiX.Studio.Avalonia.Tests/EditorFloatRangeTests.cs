using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Min and Max editor of a FloatRange, with and without a caption.
    public class EditorFloatRangeTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private LFOModulator NewModulator()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(LFOModulator));
            return (LFOModulator)blur.ModulatorManager.SelectedItem;
        }

        private static EditorFloatRange Show(LFOModulator modulator, string caption)
        {
            var editor = new EditorFloatRange { DataContext = modulator.Range, Caption = caption };
            new Window { Content = editor, Width = 400, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();
            return editor;
        }

        private static RangeValue Range(Control editor) =>
            editor.GetVisualDescendants().OfType<RangeValue>().Single();

        [AvaloniaFact]
        public void LeftValue_Set_ChangesTheMinimum()
        {
            var modulator = NewModulator();
            var editor = Show(modulator, "Bass");

            Range(editor).LeftValue = -2.0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(-2f, modulator.Range.Minimum.Value);
        }

        [AvaloniaFact]
        public void RightValue_Set_ChangesTheMaximum()
        {
            var modulator = NewModulator();
            var editor = Show(modulator, "Bass");

            Range(editor).RightValue = 3.0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(3f, modulator.Range.Maximum.Value);
        }

        [AvaloniaFact]
        public void Caption_WhenSet_ShowsTheCaption()
        {
            var editor = Show(NewModulator(), "Bass");

            var range = Range(editor);
            var left = range.TranslatePoint(new Point(0, 0), editor)!.Value.X;
            Assert.True(left > 0);
            Assert.Contains(editor.GetVisualDescendants().OfType<TextBlock>(), t => t.Text == "Bass");
        }

        [AvaloniaFact]
        public void Caption_WhenEmpty_ShowsNoCaptionColumn()
        {
            var editor = Show(NewModulator(), "");

            var left = Range(editor).TranslatePoint(new Point(0, 0), editor)!.Value.X;
            Assert.Equal(0.0, left);
        }
    }
}
