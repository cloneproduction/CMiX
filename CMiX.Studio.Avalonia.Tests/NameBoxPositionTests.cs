using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // The text of a name is at the same place when it is shown and when it is edited. Every name of the app is a
    // TextBoxEditable: the rows of the tree and of the lists use the same control.
    public class NameBoxPositionTests
    {
        private sealed class Measure
        {
            public Rect DisplayBox { get; init; }
            public Point DisplayText { get; init; }
            public Rect InputBox { get; init; }
            public Point EditText { get; init; }
            public Vector Scroll { get; init; }
            public ScrollBarVisibility VerticalScroll { get; init; }
        }

        // The boxes and the origin of the text in the display mode and in the edit mode, in window coordinates.
        private static Measure Take(Window window, TextBoxEditable box)
        {
            var display = box.GetVisualDescendants().OfType<TextBlock>().First(block => block.Name == "textDisplay");
            var input = box.GetVisualDescendants().OfType<TextBox>().First();

            window.UpdateLayout();
            var displayBox = new Rect(display.TranslatePoint(new Point(0, 0), window)!.Value, display.Bounds.Size);
            var displayText = display.TranslatePoint(new Point(display.Padding.Left, display.Padding.Top), window)!.Value;

            box.IsEditing = true;
            TestServiceProviderFactory.Pump();
            window.UpdateLayout();
            var presenter = input.GetVisualDescendants().OfType<TextPresenter>().First(p => p.Name == "PART_TextPresenter");
            var viewer = input.GetVisualDescendants().OfType<ScrollViewer>().First();
            return new Measure
            {
                DisplayBox = displayBox,
                DisplayText = displayText,
                InputBox = new Rect(input.TranslatePoint(new Point(0, 0), window)!.Value, input.Bounds.Size),
                EditText = presenter.TranslatePoint(new Point(0, 0), window)!.Value,
                Scroll = viewer.Offset,
                VerticalScroll = ScrollViewer.GetVerticalScrollBarVisibility(input)
            };
        }

        private static void AssertSamePlace(Measure measure)
        {
            // The text is at the same place.
            Assert.Equal(measure.DisplayText, measure.EditText);

            // The two boxes have the same height and the same top, so any rounding is the same for both.
            Assert.Equal(measure.DisplayBox.Top, measure.InputBox.Top);
            Assert.Equal(measure.DisplayBox.Height, measure.InputBox.Height);

            // The edit mode does not scroll the text.
            Assert.Equal(0, measure.Scroll.Y);
            Assert.Equal(ScrollBarVisibility.Disabled, measure.VerticalScroll);
        }

        [AvaloniaFact]
        public void Show_ANameBox_PutsTheTextAtTheSamePlaceInBothModes()
        {
            var box = new TextBoxEditable { Text = "Layer.001" };
            var window = new Window { Content = box, Width = 300, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            AssertSamePlace(Take(window, box));
        }

        // A font size with a fractional line height is where a rounding of 1 pixel shows.
        [AvaloniaFact]
        public void Show_ANameBoxOfAnyHeightAndFontSize_PutsTheTextAtTheSamePlaceInBothModes()
        {
            foreach (var (height, fontSize) in new[] { (20.0, 12.0), (21.0, 12.5), (24.0, 12.5), (24.0, 13.3), (25.0, 11.7), (31.0, 12.5), (24.0, 10.0) })
            {
                var box = new TextBoxEditable { Text = "Layer.001", Height = height, FontSize = fontSize };
                var window = new Window { Content = box, Width = 300, Height = 100 };
                window.Show();
                Dispatcher.UIThread.RunJobs();

                AssertSamePlace(Take(window, box));
                window.Close();
            }
        }

        [AvaloniaFact]
        public void Show_ANameInTheTree_PutsTheTextAtTheSamePlaceInBothModes()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            var tree = window.GetVisualDescendants().OfType<TreeViewProject>().Single();
            var box = tree.GetVisualDescendants().OfType<TextBoxEditable>().First(name => name.IsEffectivelyVisible);

            AssertSamePlace(Take(window, box));
        }

        [AvaloniaFact]
        public void Show_ANameInTheList_PutsTheTextAtTheSamePlaceInBothModes()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.SelectMainTab(window, "Composition");
            TestServiceProviderFactory.Pump();
            var tree = window.GetVisualDescendants().OfType<TreeViewProject>().Single();
            var box = window.GetVisualDescendants().OfType<RowComposable>().First()
                .GetVisualDescendants().OfType<TextBoxEditable>().First();
            Assert.False(tree.IsVisualAncestorOf(box));

            AssertSamePlace(Take(window, box));
        }
    }
}
