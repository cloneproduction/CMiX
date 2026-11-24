using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CMiX.UI.Mathematics;
using CMiX.UI.Popups;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A shortcut has one look in a tooltip, in a context menu, in the main menu and in a key cap: the mono font and ShortcutTextBrush.
    // The menus test their shortcut in ContextMenuLookTests and MenuLookTests.
    public class ShortcutTextLookTests
    {
        private static void AssertShortcutLook(TextBlock text)
        {
            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), text.FontFamily);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ((ISolidColorBrush)text.Foreground!).Color);
        }

        [AvaloniaFact]
        public void ContrastRatio_ShortcutTextOnThePopupCard_ReachesWcagAA()
        {
            Assert.True(ColorMix.Contrast(Tokens.Get<Color>("ShortcutText"), Tokens.Get<Color>("PopupCardBackground")) >= 4.5);
        }

        [AvaloniaFact]
        public void Show_TheShortcutOfATooltip_HasTheShortcutLook()
        {
            var tip = new ToolTip { Content = new TipContent("Add", "Shift+A") };
            TestWindow.Show(tip, 300, 200);

            AssertShortcutLook(tip.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Text == "Shift+A"));
        }

        [AvaloniaFact]
        public void Show_AKeyCap_HasTheShortcutLookInARoundedFrame()
        {
            var cap = new ContentControl { Theme = Tokens.Get<ControlTheme>("ContentControlKeyCap"), Content = "G" };
            TestWindow.Show(cap, 300, 200);

            var frame = cap.GetVisualDescendants().OfType<Border>().Single(part => part.Name == "PART_Cap");
            var text = cap.GetVisualDescendants().OfType<TextBlock>().Single(part => part.Name == "PART_KeyText");

            Assert.Equal("G", text.Text);
            AssertShortcutLook(text);
            Assert.Equal(Tokens.Get<Color>("Grey600"), ((ISolidColorBrush)frame.BorderBrush!).Color);
            Assert.Equal(Tokens.Get<Thickness>("KeyCapBorderThickness"), frame.BorderThickness);
            Assert.Equal(Tokens.Get<Thickness>("KeyCapPadding"), frame.Padding);
            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), frame.CornerRadius);
            Assert.True(frame.Bounds.Width > text.Bounds.Width);
        }
    }
}
