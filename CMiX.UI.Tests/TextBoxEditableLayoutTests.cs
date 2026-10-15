using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // The name in a row must not move when the user starts to edit it.
    public class TextBoxEditableLayoutTests
    {
        private static (Rect shown, Rect edited, double height) Measure(TextBoxEditable box)
        {
            var window = new Window { Content = box, Width = 300, Height = 60 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var grid = (Grid)box.Content!;
            var block = box.GetVisualDescendants().OfType<TextBlock>().First(t => t.Name == "TextDisplay");
            var shownOrigin = block.TranslatePoint(new Point(block.Padding.Left, 0), grid)!.Value;
            var shown = new Rect(shownOrigin, new Size(block.Bounds.Width, block.Bounds.Height));

            box.IsEditing = true;
            box.GetVisualDescendants().OfType<TextBox>().First().IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            var presenter = box.GetVisualDescendants().OfType<TextPresenter>().First();
            var editedOrigin = presenter.TranslatePoint(new Point(0, 0), grid)!.Value;
            var edited = new Rect(editedOrigin, presenter.Bounds.Size);

            return (shown, edited, grid.Bounds.Height);
        }

        [AvaloniaFact]
        public void TheText_StaysInTheSamePlace_WhenTheUserStartsToEdit()
        {
            var (shown, edited, _) = Measure(new TextBoxEditable { Text = "Layer 1", FontSize = 12.5 });

            Assert.Equal(shown.X, edited.X);
            Assert.Equal(shown.Y, edited.Y);
        }
    }
}
