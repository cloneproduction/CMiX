using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The ColorSelector has the same menu as the other value editors.
    public class ColorSelectorMenuTests
    {
        private static (ColorSelector Selector, StringValue Model) Show(string value = "#FF336699")
        {
            var model = new StringValue { Value = value };
            var selector = new ColorSelector { Caption = "Color", DataContext = model, Width = 200, Height = 40 };
            new Window { Content = selector, Width = 300, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (selector, model);
        }

        private static IValueText Text(ColorSelector selector) => selector;

        [AvaloniaFact]
        public void TheMenu_HasTheTitleCopyPasteAndReset()
        {
            var (selector, _) = Show();

            Assert.Equal(new object?[] { "Color", "-", "Copy", "Paste", "-", "Reset" },
                selector.ContextMenu!.Items.OfType<MenuItem>().Select(item => item.Header).ToArray());
        }

        [AvaloniaFact]
        public void TheSelectedColor_FollowsTheModel_BothWays()
        {
            var (selector, model) = Show();

            Assert.Equal("#FF336699", Text(selector).CopyValueText());

            model.Value = "#80FF0000";
            Assert.Equal("#80FF0000", Text(selector).CopyValueText());
        }

        [AvaloniaTheory]
        [InlineData("#FF00FF00", "#FF00FF00")]
        [InlineData("#00FF00", "#FF00FF00")]
        [InlineData("  red ", "#FFFF0000")]
        public void APaste_SetsTheModel(string text, string expected)
        {
            var (selector, model) = Show();

            Assert.True(Text(selector).CanPasteValueText(text));
            Text(selector).PasteValueText(text);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(expected, model.Value);
        }

        [AvaloniaTheory]
        [InlineData("")]
        [InlineData("not a color")]
        [InlineData("12.5")]
        public void ATextThatIsNotAColor_CannotBePasted(string text)
        {
            var (selector, model) = Show();

            Assert.False(Text(selector).CanPasteValueText(text));
            Text(selector).PasteValueText(text);

            Assert.Equal("#FF336699", model.Value);
        }

        [AvaloniaFact]
        public void TheResetRow_RestoresTheModel()
        {
            var (selector, model) = Show();

            selector.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Reset"))
                .Command!.Execute(null);

            Assert.Equal("#FF000000", model.Value);
        }
    }
}
