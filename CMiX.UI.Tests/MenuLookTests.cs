// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Mathematics;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The main menu with MenuDefault and MenuItemTop: the dropdown is a PopupFrame with MenuItemContext rows on the card,
    // hover and the arrow keys give a row the hover fill, the shortcut is readable on the fill, the open top item keeps the accent.
    public class MenuLookTests
    {
        // Shows a main menu with one top item and opens it.
        private static MenuItem OpenTop(string header, params object[] rows)
        {
            var top = new MenuItem
            {
                Header = header,
                Theme = Tokens.Theme("MenuItemTop"),
                ItemsSource = rows,
            };
            var menu = new Menu { Theme = Tokens.Theme("MenuDefault"), ItemsSource = new object[] { top } };
            TestWindow.Show(menu, 500, 400);
            top.Open();
            Dispatcher.UIThread.RunJobs();
            return top;
        }

        private static (MenuItem File, MenuItem Open, MenuItem Save) OpenFile()
        {
            var open = new MenuItem { Header = "Open", InputGesture = new KeyGesture(Key.O, KeyModifiers.Control) };
            var save = new MenuItem { Header = "Save", InputGesture = new KeyGesture(Key.S, KeyModifiers.Control) };
            return (OpenTop("File", open, save), open, save);
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
            (Parts.Find<Border>(row, "root").Background as ISolidColorBrush)?.Color;

        private static TextBlock GestureOf(MenuItem row) => Parts.Find<TextBlock>(row, "PART_InputGestureText");

        private static Color ShortcutOf(MenuItem row) => ((ISolidColorBrush)GestureOf(row).Foreground!).Color;

        // The fill of the panel of the dropdown of an item: the one Border of the frame that has an opaque fill.
        private static Color BackgroundOf(MenuItem item)
        {
            var popup = Parts.Find<Popup>(item, "PART_Popup");
            var panel = popup.Child!.GetVisualDescendants().OfType<Border>().First(part => part.Background is ISolidColorBrush fill && fill.Color.A == 255);
            return ((ISolidColorBrush)panel.Background!).Color;
        }

        private static Color HoverFill => Tokens.Get<Color>("Grey500");

        [AvaloniaFact]
        public void Open_ARowOfTheMenuBar_ShowsContextMenuRowsInAPopupFrame()
        {
            var open = new MenuItem { Header = "Open" };

            var file = OpenTop("File", open);

            Assert.IsType<PopupFrame>(Parts.Find<Popup>(file, "PART_Popup").Child);
            Assert.Same(Tokens.Theme("MenuItemContext"), open.Theme);
            Assert.Equal(Tokens.Get<double>("ControlHeightMD"), open.Bounds.Height);
        }

        [AvaloniaFact]
        public void Open_TheMainMenuDropdownAndItsSubmenu_HaveThePopupCardBackground()
        {
            var nested = new MenuItem { Header = "Recent", ItemsSource = new object[] { new MenuItem { Header = "One" } } };
            var file = OpenTop("File", nested);
            nested.Open();
            Dispatcher.UIThread.RunJobs();

            Assert.IsType<PopupFrame>(Parts.Find<Popup>(file, "PART_Popup").Child);
            Assert.IsType<PopupFrame>(Parts.Find<Popup>(nested, "PART_Popup").Child);
            var card = Tokens.Get<Color>("PopupCardBackground");
            Assert.Equal(card, BackgroundOf(file));
            Assert.Equal(card, BackgroundOf(nested));
        }

        [AvaloniaFact]
        public void Hover_ARowOfTheDropdown_GivesItTheHoverFill()
        {
            var (_, open, save) = OpenFile();

            Hover(open);

            Assert.True(open.IsSelected);
            Assert.Equal(Tokens.Color("RowHoverBrush"), FillOf(open));
            Assert.NotEqual(HoverFill, FillOf(save));
        }

        [AvaloniaFact]
        public void Down_InTheDropdown_MovesTheHoverFillToTheNextRow()
        {
            var (_, open, save) = OpenFile();
            Hover(open);
            Leave(open);

            KeyInput.PressOnFocus(open, Key.Down);

            Assert.True(save.IsSelected);
            Assert.False(open.IsSelected);
            Assert.Equal(Tokens.Color("RowHoverBrush"), FillOf(save));
            Assert.NotEqual(HoverFill, FillOf(open));
        }

        [AvaloniaFact]
        public void Hover_ARowOfTheDropdown_MakesTheShortcutLighterAndReadable()
        {
            var (_, open, save) = OpenFile();

            Hover(open);

            Assert.Equal(Tokens.Get<Color>("Grey900"), ShortcutOf(open));
            Assert.True(ColorMix.Contrast(ShortcutOf(open), FillOf(open)!.Value) >= 4.5);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ShortcutOf(save));
            Assert.True(ColorMix.Contrast(ShortcutOf(save), Tokens.Get<Color>("PopupCardBackground")) >= 4.5);
        }

        [AvaloniaFact]
        public void Open_TheShortcutOfAMainMenuRow_HasTheShortcutLook()
        {
            var undo = new MenuItem { Header = "Undo", InputGesture = new KeyGesture(Key.Z, KeyModifiers.Control) };
            var plain = new MenuItem { Header = "Save" };

            OpenTop("Edit", undo, plain);

            Assert.True(GestureOf(undo).IsEffectivelyVisible);
            Assert.Equal("Ctrl+Z", GestureOf(undo).Text);
            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), GestureOf(undo).FontFamily);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ShortcutOf(undo));
            Assert.False(GestureOf(plain).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_TheShortcutTextOnTheDropdown_ReachesWcagAA()
        {
            var file = OpenTop("File", new MenuItem { Header = "Open" });

            Assert.True(ColorMix.Contrast(Tokens.Get<Color>("ShortcutText"), BackgroundOf(file)) >= 4.5);
        }

        [AvaloniaFact]
        public void Open_TheTopItem_KeepsTheAccentFill()
        {
            var (file, open, _) = OpenFile();
            Hover(open);

            Assert.True(file.IsSubMenuOpen);
            Assert.Equal(Tokens.Get<Color>("AccentPrimary"), FillOf(file));
        }
    }
}
