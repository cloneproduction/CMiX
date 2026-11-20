using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Popups;
using Xunit;

namespace CMiX.UI.Tests
{
    // A shortcut has one look in a tooltip, in a context menu, in the main menu and in a key cap: the mono font and ShortcutTextBrush.
    public class ShortcutTextLookTests
    {
        // The WCAG 2 contrast ratio of two opaque colors.
        private static double ContrastRatio(Color text, Color background)
        {
            static double Channel(byte value)
            {
                var c = value / 255.0;
                return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
            }

            static double Luminance(Color color) =>
                0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);

            var lighter = Math.Max(Luminance(text), Luminance(background));
            var darker = Math.Min(Luminance(text), Luminance(background));
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static void AssertShortcutLook(TextBlock text)
        {
            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), text.FontFamily);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ((ISolidColorBrush)text.Foreground!).Color);
        }

        [AvaloniaTheory]
        [InlineData("ShortcutText", 4.835)]
        [InlineData("Grey800", 4.499)]
        [InlineData("Grey700", 2.827)]
        public void ContrastRatio_TextOnThePopupCard_HasItsValue(string key, double expected)
        {
            var ratio = ContrastRatio(Tokens.Get<Color>(key), Tokens.Get<Color>("PopupCardBackground"));

            Assert.Equal(expected, ratio, 3);
        }

        [AvaloniaFact]
        public void ContrastRatio_ShortcutTextOnThePopupCard_ReachesWcagAA()
        {
            Assert.True(ContrastRatio(Tokens.Get<Color>("ShortcutText"), Tokens.Get<Color>("PopupCardBackground")) >= 4.5);
        }

        // The dropdown of the main menu and its submenus are on the popup card, as the context menu.
        [AvaloniaFact]
        public void Open_TheMainMenuDropdownAndItsSubmenu_HaveThePopupCardBackground()
        {
            var nested = new MenuItem { Header = "Recent", ItemsSource = new object[] { new MenuItem { Header = "One" } } };
            var file = new MenuItem
            {
                Header = "File",
                Theme = (ControlTheme)Application.Current!.FindResource("MenuItemTop")!,
                ItemsSource = new object[] { nested },
            };
            var menu = new Menu { Theme = (ControlTheme)Application.Current!.FindResource("MenuDefault")!, ItemsSource = new object[] { file } };
            new Window { Content = menu, Width = 500, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            file.Open();
            Dispatcher.UIThread.RunJobs();
            nested.Open();
            Dispatcher.UIThread.RunJobs();

            Color BackgroundOf(MenuItem item)
            {
                var popup = item.GetVisualDescendants().OfType<Popup>().Single(part => part.Name == "PART_Popup");
                // The panel is the one Border of the frame that has a fill.
                var frame = Assert.IsType<CMiX.UI.Controls.Parts.PopupFrame>(popup.Child);
                var panel = frame.GetVisualDescendants().OfType<Border>().First(part => part.Background is ISolidColorBrush fill && fill.Color.A == 255);
                return ((ISolidColorBrush)panel.Background!).Color;
            }

            var card = Tokens.Get<Color>("PopupCardBackground");
            Assert.Equal(card, BackgroundOf(file));
            Assert.Equal(card, BackgroundOf(nested));
        }

        [AvaloniaFact]
        public void ContrastRatio_ShortcutTextOnTheMainMenuDropdown_ReachesWcagAA()
        {
            var file = new MenuItem
            {
                Header = "File",
                Theme = (ControlTheme)Application.Current!.FindResource("MenuItemTop")!,
                ItemsSource = new object[] { new MenuItem { Header = "Open" } },
            };
            var menu = new Menu { Theme = (ControlTheme)Application.Current!.FindResource("MenuDefault")!, ItemsSource = new object[] { file } };
            new Window { Content = menu, Width = 500, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            file.Open();
            Dispatcher.UIThread.RunJobs();

            var popup = file.GetVisualDescendants().OfType<Popup>().Single(part => part.Name == "PART_Popup");
            var panel = popup.Child!.GetVisualDescendants().OfType<Border>().First(part => part.Background is ISolidColorBrush fill && fill.Color.A == 255);
            var background = ((ISolidColorBrush)panel.Background!).Color;

            Assert.True(ContrastRatio(Tokens.Get<Color>("ShortcutText"), background) >= 4.5);
        }

        [AvaloniaFact]
        public void Show_TheShortcutOfATooltip_HasTheShortcutLook()
        {
            var tip = new ToolTip { Content = new TipContent("Add", "Shift+A") };
            new Window { Content = tip, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            AssertShortcutLook(tip.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Text == "Shift+A"));
        }

        [AvaloniaFact]
        public void Open_TheShortcutOfAContextMenuRow_HasTheShortcutLook()
        {
            var menu = new ContextMenu
            {
                Theme = (ControlTheme)Application.Current!.FindResource("ContextMenuDefault")!,
                ItemsSource = new object[] { new MenuItem { Header = "Duplicate", InputGesture = new KeyGesture(Key.D, KeyModifiers.Control) } },
            };
            var owner = new Border { Width = 200, Height = 30, ContextMenu = menu };
            new Window { Content = owner, Width = 500, Height = 400 }.Show();
            menu.Open(owner);
            Dispatcher.UIThread.RunJobs();

            var gesture = menu.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_InputGestureText");

            Assert.Equal("Ctrl+D", gesture.Text);
            AssertShortcutLook(gesture);
        }

        // A row of the main menu has the default MenuItem theme. Its shortcut comes from InputGesture, as in a context menu.
        [AvaloniaFact]
        public void Open_TheShortcutOfAMainMenuRow_HasTheShortcutLook()
        {
            var undo = new MenuItem { Header = "Undo", InputGesture = new KeyGesture(Key.Z, KeyModifiers.Control) };
            var plain = new MenuItem { Header = "Save" };
            var edit = new MenuItem
            {
                Header = "Edit",
                Theme = (ControlTheme)Application.Current!.FindResource("MenuItemTop")!,
                ItemsSource = new object[] { undo, plain },
            };
            var menu = new Menu { Theme = (ControlTheme)Application.Current!.FindResource("MenuDefault")!, ItemsSource = new object[] { edit } };
            new Window { Content = menu, Width = 500, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            edit.Open();
            Dispatcher.UIThread.RunJobs();

            TextBlock GestureOf(MenuItem row) => row.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_InputGestureText");

            Assert.True(GestureOf(undo).IsEffectivelyVisible);
            Assert.Equal("Ctrl+Z", GestureOf(undo).Text);
            AssertShortcutLook(GestureOf(undo));
            Assert.False(GestureOf(plain).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_AKeyCap_HasTheShortcutLookInARoundedFrame()
        {
            var cap = new ContentControl { Theme = Tokens.Get<ControlTheme>("ContentControlKeyCap"), Content = "G" };
            new Window { Content = cap, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

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
