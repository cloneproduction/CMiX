using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.String;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The suffix is a control that follows the text of the box, such as the counts of a row of a tree.
    public class TextBoxEditableSuffixTests
    {
        private static (TextBoxEditable Box, Border Suffix, Window Window) Show(string text = "Layer 1")
        {
            var suffix = new Border { Width = 20, Height = 10 };
            var box = new TextBoxEditable { Text = text, Suffix = suffix };
            return (box, suffix, TestWindow.Show(box, 300, 60));
        }

        [AvaloniaFact]
        public void Show_AnyText_KeepsTheSuffixAtAFixedDistanceFromItsEnd()
        {
            var (box, suffix, window) = Show();
            var text = box.GetVisualDescendants().OfType<TextBlock>().First(block => block.Name == "textDisplay");

            var textRight = text.TranslatePoint(new Point(text.Bounds.Width, 0), window)!.Value.X;
            var suffixLeft = suffix.TranslatePoint(new Point(0, 0), window)!.Value.X;

            Assert.Equal(4, suffixLeft - textRight, 1);
        }

        [AvaloniaFact]
        public void Show_ALongerText_MovesTheSuffix()
        {
            var (_, shortSuffix, window) = Show("A");
            var shortLeft = shortSuffix.TranslatePoint(new Point(0, 0), window)!.Value.X;

            var (_, longSuffix, longWindow) = Show("A much longer name");
            var longLeft = longSuffix.TranslatePoint(new Point(0, 0), longWindow)!.Value.X;

            Assert.True(longLeft > shortLeft);
        }

        [AvaloniaFact]
        public void Show_TheSuffix_IsCenteredInTheHeightOfTheBox()
        {
            var (box, suffix, window) = Show();

            var middle = suffix.TranslatePoint(new Point(0, suffix.Bounds.Height / 2), box)!.Value.Y;

            Assert.Equal(box.Bounds.Height / 2, middle, 1);
        }

        [AvaloniaFact]
        public void Edit_TheText_HidesTheSuffix()
        {
            var (box, suffix, _) = Show();
            Assert.True(suffix.IsEffectivelyVisible);

            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            Assert.False(suffix.IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_ABoxWithoutASuffix_KeepsTheTextPlace()
        {
            var (_, _, window) = Show();
            var withSuffix = window.GetVisualDescendants().OfType<TextBlock>().First(block => block.Name == "textDisplay").Bounds;

            var plain = new TextBoxEditable { Text = "Layer 1" };
            TestWindow.Show(plain, 300, 60);
            var withoutSuffix = plain.GetVisualDescendants().OfType<TextBlock>().First(block => block.Name == "textDisplay").Bounds;

            Assert.Equal(withoutSuffix, withSuffix);
        }
    }
}
