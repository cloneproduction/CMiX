using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.UI.Tests
{
    // A row of the main menu dropdown has the hover fill on hover and on the row of the arrow keys, as a context menu row.
    // On that fill the shortcut is lighter. An open top item keeps the accent fill.
    public class MainMenuRowLookTests
    {
        private static (Window Window, MenuItem File, MenuItem Open, MenuItem Save) OpenFile()
        {
            var open = new MenuItem { Header = "Open", InputGesture = new KeyGesture(Key.O, KeyModifiers.Control) };
            var save = new MenuItem { Header = "Save", InputGesture = new KeyGesture(Key.S, KeyModifiers.Control) };
            var file = new MenuItem
            {
                Header = "File",
                Theme = (ControlTheme)Application.Current!.FindResource("MenuItemTop")!,
                ItemsSource = new object[] { open, save },
            };
            var menu = new Menu { Theme = (ControlTheme)Application.Current!.FindResource("MenuDefault")!, ItemsSource = new object[] { file } };
            var window = new Window { Content = menu, Width = 500, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            file.Open();
            Dispatcher.UIThread.RunJobs();
            return (window, file, open, save);
        }

        private static ContextMenu OpenContextMenu(params MenuItem[] rows)
        {
            var menu = new ContextMenu
            {
                Theme = (ControlTheme)Application.Current!.FindResource("ContextMenuDefault")!,
                ItemsSource = rows,
            };
            var owner = new Border { Width = 200, Height = 30, ContextMenu = menu };
            new Window { Content = owner, Width = 500, Height = 400 }.Show();
            menu.Open(owner);
            Dispatcher.UIThread.RunJobs();
            return menu;
        }

        private static void Press(Control target, Key key)
        {
            var focused = TopLevel.GetTopLevel(target)?.FocusManager?.GetFocusedElement() as Interactive;
            (focused ?? target).RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });
            Dispatcher.UIThread.RunJobs();
        }

        // The menu selects a row when the pointer enters it.
        private static void Hover(MenuItem row) => RaisePointer(row, InputElement.PointerEnteredEvent);

        // The pointer goes off the row.
        private static void Leave(MenuItem row) => RaisePointer(row, InputElement.PointerExitedEvent);

        private static void RaisePointer(MenuItem row, RoutedEvent routedEvent)
        {
            row.RaiseEvent(new PointerEventArgs(routedEvent, row,
                new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true), TopLevel.GetTopLevel(row), default, 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None));
            Dispatcher.UIThread.RunJobs();
        }

        private static Color? FillOf(MenuItem row) =>
            (row.GetVisualDescendants().OfType<Border>().First(border => border.Name == "root").Background as ISolidColorBrush)?.Color;

        private static Color ShortcutOf(MenuItem row) =>
            ((ISolidColorBrush)row.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_InputGestureText").Foreground!).Color;

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

        private static Color HoverFill => Tokens.Get<Color>("Grey500");

        [AvaloniaFact]
        public void Hover_ARowOfTheDropdown_GivesItTheHoverFill()
        {
            var (_, _, open, save) = OpenFile();

            Hover(open);

            Assert.True(open.IsSelected);
            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("RowHoverBrush")!).Color, FillOf(open));
            Assert.NotEqual(HoverFill, FillOf(save));
        }

        [AvaloniaFact]
        public void Down_InTheDropdown_MovesTheHoverFillToTheNextRow()
        {
            var (_, _, open, save) = OpenFile();
            Hover(open);
            Leave(open);

            Press(open, Key.Down);

            Assert.True(save.IsSelected);
            Assert.False(open.IsSelected);
            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("RowHoverBrush")!).Color, FillOf(save));
            Assert.NotEqual(HoverFill, FillOf(open));
        }

        [AvaloniaFact]
        public void Hover_ARowOfTheDropdown_MakesTheShortcutLighterAndReadable()
        {
            var (_, _, open, save) = OpenFile();

            Hover(open);

            Assert.Equal(Tokens.Get<Color>("Grey900"), ShortcutOf(open));
            Assert.True(ContrastRatio(ShortcutOf(open), FillOf(open)!.Value) >= 4.5);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ShortcutOf(save));
            Assert.True(ContrastRatio(ShortcutOf(save), Tokens.Get<Color>("PopupCardBackground")) >= 4.5);
        }

        [AvaloniaFact]
        public void Down_InAContextMenu_MakesTheShortcutLighterAndReadable()
        {
            var first = new MenuItem { Header = "Duplicate", InputGesture = new KeyGesture(Key.D, KeyModifiers.Control) };
            var second = new MenuItem { Header = "Rename", InputGesture = new KeyGesture(Key.R, KeyModifiers.Control) };
            var menu = OpenContextMenu(first, second);

            Press(menu, Key.Down);

            Assert.True(first.IsSelected);
            Assert.Equal(Tokens.Get<Color>("Grey900"), ShortcutOf(first));
            Assert.True(ContrastRatio(ShortcutOf(first), FillOf(first)!.Value) >= 4.5);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ShortcutOf(second));
            Assert.True(ContrastRatio(ShortcutOf(second), Tokens.Get<Color>("PopupCardBackground")) >= 4.5);
        }

        [AvaloniaFact]
        public void Open_TheTopItem_KeepsTheAccentFill()
        {
            var (_, file, open, _) = OpenFile();
            Hover(open);

            Assert.True(file.IsSubMenuOpen);
            Assert.Equal(Tokens.Get<Color>("AccentPrimary"), FillOf(file));
        }
    }
}
