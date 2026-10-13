using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // A long name stays on one line, ends with "..." and shows its full text on hover while it is cut.
    public class TextBlockTrimmedTests
    {
        private const string LongName = "a_very_long_name_for_a_video_file_that_can_never_fit_in_a_small_box";

        private static TextBlockTrimmed Show(string text, double width, string? toolTipText = null)
        {
            var block = new TextBlockTrimmed { Text = text, ToolTipText = toolTipText };
            var host = new Border { Width = width, Child = block };
            var window = new Window { Content = new StackPanel { Children = { host } }, Width = 600, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return block;
        }

        [AvaloniaFact]
        public void ItStaysOnOneLine_AndEndsWithEllipsis()
        {
            var block = new TextBlockTrimmed();

            Assert.Equal(TextTrimming.CharacterEllipsis, block.TextTrimming);
            Assert.Equal(TextWrapping.NoWrap, block.TextWrapping);
        }

        [AvaloniaFact]
        public void ANameThatDoesNotFit_ShowsItsFullTextOnHover()
        {
            var block = Show(LongName, 60);

            Assert.Equal(LongName, ToolTip.GetTip(block));
        }

        [AvaloniaFact]
        public void ANameThatFits_HasNoToolTip()
        {
            var block = Show("short", 400);

            Assert.Null(ToolTip.GetTip(block));
        }

        [AvaloniaFact]
        public void ToolTipText_ReplacesTheHoverText_EvenWhenTheNameFits()
        {
            var block = Show("short", 400, @"C:\videos\short.mp4");

            Assert.Equal(@"C:\videos\short.mp4", ToolTip.GetTip(block));
        }

        [AvaloniaFact]
        public void TheStylesOfTheTextBlock_ApplyToIt()
        {
            var block = Show("short", 400);

            // The library style gives every TextBlock the vertical alignment Center.
            Assert.Equal(Avalonia.Layout.VerticalAlignment.Center, block.VerticalAlignment);
        }
    }
}
