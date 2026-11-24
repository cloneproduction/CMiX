// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Mathematics;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // The look of a menu with ContextMenuDefault: the card, the separator row, the middle on the click,
    // the rows (shortcut, arrow, title, danger, disabled), the row of the arrow keys, the submenu and the inset.
    public class ContextMenuLookTests
    {
        private static ContextMenu NewMenu(params object[] items) => new()
        {
            Theme = Tokens.Theme("ContextMenuDefault"),
            ItemsSource = items,
        };

        // Opens a menu with the rows on a small border.
        private static ContextMenu Open(params object[] items)
        {
            var menu = NewMenu(items);
            var owner = new Border { Width = 200, Height = 30, ContextMenu = menu };
            TestWindow.Show(owner, 500, 400);
            menu.Open(owner);
            Dispatcher.UIThread.RunJobs();
            return menu;
        }

        // A menu with a title, a separator, rows with and without a shortcut, a submenu and a danger row.
        private static (ContextMenu Menu, Dictionary<string, MenuItem> Rows) OpenFull()
        {
            var add = new MenuItem { Header = "Add", InputGesture = new KeyGesture(Key.A, KeyModifiers.Shift) };
            add.Classes.Add("opens");
            var title = new MenuItem { Header = "Layer 02", IsEnabled = false };
            title.Classes.Add("title");
            var danger = new MenuItem { Header = "Delete", InputGesture = new KeyGesture(Key.Delete) };
            danger.Classes.Add("danger");
            var submenu = new MenuItem { Header = "More", Items = { new MenuItem { Header = "One" } } };
            var menu = Open(
                title,
                new MenuItem { Header = "-" },
                add,
                new MenuItem { Header = "Duplicate", InputGesture = new KeyGesture(Key.D, KeyModifiers.Control) },
                new MenuItem { Header = "Rename" },
                submenu,
                danger);

            var rows = menu.Items.OfType<MenuItem>().Where(item => item.Header is string header && header != "-")
                .ToDictionary(item => (string)item.Header!);
            return (menu, rows);
        }

        // A menu on a big owner, for the tests of the position.
        private static (Window Window, Border Owner) ShowOwner(ContextMenu menu)
        {
            var owner = new Border { Width = 600, Height = 400, Background = Brushes.Gray, ContextMenu = menu };
            return (TestWindow.Show(owner, 800, 600), owner);
        }

        // The middle of the open menu, in window coordinates.
        private static Point MiddleOf(Window window, ContextMenu menu)
        {
            var frame = menu.GetVisualDescendants().OfType<PopupFrame>().First();
            return frame.TranslatePoint(new Point(frame.Bounds.Width / 2, frame.Bounds.Height / 2), window)!.Value;
        }

        private static void AssertMiddleOn(Point expected, Point actual)
        {
            Assert.InRange(actual.X, expected.X - 1, expected.X + 1);
            Assert.InRange(actual.Y, expected.Y - 1, expected.Y + 1);
        }

        private static IBrush? FillOf(MenuItem row) => Parts.Find<Border>(row, "root").Background;

        private static bool HasHoverFill(MenuItem row) =>
            FillOf(row) is ISolidColorBrush brush && brush.Color == Tokens.Color("RowHoverBrush");

        private static TextBlock GestureOf(MenuItem row) => Parts.Find<TextBlock>(row, "PART_InputGestureText");

        private static Color ShortcutOf(MenuItem row) => ((ISolidColorBrush)GestureOf(row).Foreground!).Color;

        // The card, the separator and the position.

        [AvaloniaFact]
        public void Open_TheResetMenuOfASlider_ShowsItsRowInAPopupFrame()
        {
            var slider = new SliderCMiX { Width = 200, Height = 30, ResetCommand = new RelayCommand(() => { }) };
            TestWindow.Show(slider, 400, 300);

            slider.ContextMenu!.Open(slider);
            Dispatcher.UIThread.RunJobs();

            var reset = slider.ContextMenu.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "Reset"));
            Assert.NotNull(reset.FindAncestorOfType<PopupFrame>());
        }

        [AvaloniaFact]
        public void Open_AMenu_ShowsItsRowsOnTheCard()
        {
            var (menu, _) = OpenFull();

            var frame = menu.GetVisualDescendants().OfType<PopupFrame>().First();
            var card = frame.GetVisualDescendants().OfType<Border>().First(border => border.CornerRadius != default && border.Background is ISolidColorBrush);

            Assert.Equal(Tokens.Color("PopupBackgroundBrush"), ((ISolidColorBrush)card.Background!).Color);
            Assert.Equal(new Thickness(0), card.BorderThickness);
            Assert.Equal(1, card.Opacity);
        }

        [AvaloniaFact]
        public void Open_ADashRow_ShowsAThinSeparator()
        {
            var menu = Open(new MenuItem { Header = "A" }, new MenuItem { Header = "-" }, new MenuItem { Header = "B" });

            var row = menu.GetVisualDescendants().OfType<MenuItem>().Single(i => Equals(i.Header, "-"));
            var line = row.GetVisualDescendants().OfType<Separator>().Single();
            Assert.Equal(0.5, line.Height);
            Assert.True(row.Bounds.Height < 12, $"The separator row is {row.Bounds.Height} px high.");
        }

        [AvaloniaTheory]
        [InlineData("Reset")]
        [InlineData("New Folder", "Rename this very long item", "Delete")]
        public void RightClick_AnyRows_PutsTheMiddleOfTheMenuOnTheClick(params string[] headers)
        {
            var menu = NewMenu(headers.Select(h => new MenuItem { Header = h }).ToArray());
            var (window, owner) = ShowOwner(menu);
            var click = new Point(300, 100);

            PointerInput.Click(window, owner, MouseButton.Right, click);

            Assert.True(menu.IsOpen);
            AssertMiddleOn(click, MiddleOf(window, menu));
        }

        [AvaloniaFact]
        public void RightClick_ASecondTime_CentersTheMenuAgain()
        {
            var menu = NewMenu(new MenuItem { Header = "Reset" });
            var (window, owner) = ShowOwner(menu);

            PointerInput.Click(window, owner, MouseButton.Right, new Point(300, 100));
            menu.Close();
            Dispatcher.UIThread.RunJobs();
            PointerInput.Click(window, owner, MouseButton.Right, new Point(200, 150));

            Assert.True(menu.IsOpen);
            AssertMiddleOn(new Point(200, 150), MiddleOf(window, menu));
        }

        // The rows.

        [AvaloniaFact]
        public void Open_ARowWithAShortcut_ShowsItInTheMonoFont()
        {
            var (_, rows) = OpenFull();

            var gesture = GestureOf(rows["Duplicate"]);

            Assert.True(gesture.IsEffectivelyVisible);
            Assert.Equal("Ctrl+D", gesture.Text);
            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), gesture.FontFamily);
        }

        [AvaloniaFact]
        public void Open_TheShortcutOfAContextMenuRow_HasTheShortcutLook()
        {
            var menu = Open(new MenuItem { Header = "Duplicate", InputGesture = new KeyGesture(Key.D, KeyModifiers.Control) });

            var gesture = menu.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_InputGestureText");

            Assert.Equal("Ctrl+D", gesture.Text);
            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), gesture.FontFamily);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ((ISolidColorBrush)gesture.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Open_ARowWithoutAShortcut_ShowsNone()
        {
            var (_, rows) = OpenFull();

            Assert.False(GestureOf(rows["Rename"]).IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_TheShortcuts_LineUpAcrossTheRows()
        {
            var (menu, rows) = OpenFull();

            double LeftOf(string header) => GestureOf(rows[header]).TranslatePoint(default, menu)!.Value.X;

            Assert.Equal(LeftOf("Add"), LeftOf("Duplicate"), 0.51);
            Assert.Equal(LeftOf("Add"), LeftOf("Delete"), 0.51);
        }

        [AvaloniaFact]
        public void Open_ARowWithASubmenu_ShowsTheArrow()
        {
            var (_, rows) = OpenFull();

            Assert.True(Parts.Find<AppIcon>(rows["More"], "PART_Arrow").IsEffectivelyVisible);
            Assert.False(Parts.Find<AppIcon>(rows["Rename"], "PART_Arrow").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_ARowOfTheClassOpens_ShowsTheArrowWithoutASubmenu()
        {
            var (_, rows) = OpenFull();

            Assert.True(Parts.Find<AppIcon>(rows["Add"], "PART_Arrow").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_TheTitleRow_IsQuietAndNotDimmed()
        {
            var (_, rows) = OpenFull();

            var root = Parts.Find<Border>(rows["Layer 02"], "root");
            var header = Parts.Find<ContentPresenter>(rows["Layer 02"], "PART_HeaderPresenter");
            Assert.Equal(1, root.Opacity);
            Assert.Equal(Tokens.Color("TextMutedBrush"), ((ISolidColorBrush)header.Foreground!).Color);
            Assert.Equal(FontWeight.Medium, header.FontWeight);
        }

        [AvaloniaFact]
        public void Open_ARowOfTheClassDanger_HasTheWarningColor()
        {
            var (_, rows) = OpenFull();

            var header = Parts.Find<ContentPresenter>(rows["Delete"], "PART_HeaderPresenter");

            Assert.Equal(Tokens.Color("PopupCardDangerBrush"), ((ISolidColorBrush)header.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Open_ADisabledRowThatIsNotATitle_IsDimmed()
        {
            var menu = Open(new MenuItem { Header = "Off", IsEnabled = false });

            var row = menu.Items.OfType<MenuItem>().Single();

            Assert.Equal(Tokens.Get<double>("OpacityDisabled"), Parts.Find<Border>(row, "root").Opacity);
        }

        [AvaloniaFact]
        public void Show_ARowOfAMenu_StartsItsContentAtEight()
        {
            var row = new MenuItem { Header = "Duplicate", Theme = Tokens.Theme("MenuItemContext"), Width = 140 };
            TestWindow.Show(row, 300, 200);

            var root = Parts.Find<Border>(row, "root");

            Assert.Equal(8, root.Padding.Left);
            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), root.CornerRadius);
        }

        // The row of the arrow keys has the hover fill, and its shortcut is lighter and readable on it.

        [AvaloniaFact]
        public void Down_InAnOpenMenu_GivesTheRowTheHoverFill()
        {
            var first = new MenuItem { Header = "One" };
            var second = new MenuItem { Header = "Two" };
            var menu = Open(first, second);

            KeyInput.PressOnFocus(menu, Key.Down);

            Assert.True(first.IsSelected);
            Assert.True(HasHoverFill(first));
            Assert.False(HasHoverFill(second));
        }

        [AvaloniaFact]
        public void Down_PastADisabledRow_SkipsItAndKeepsItsDimLook()
        {
            var first = new MenuItem { Header = "One" };
            var off = new MenuItem { Header = "Off", IsEnabled = false };
            var last = new MenuItem { Header = "Two" };
            var menu = Open(first, off, last);

            KeyInput.PressOnFocus(menu, Key.Down, 2);

            Assert.True(last.IsSelected);
            Assert.False(off.IsSelected);
            Assert.False(HasHoverFill(off));
            Assert.Equal(Tokens.Get<double>("OpacityDisabled"), Parts.Find<Border>(off, "root").Opacity);
        }

        [AvaloniaFact]
        public void Down_InAnOpenSubmenu_GivesTheRowTheHoverFill()
        {
            var inner = new MenuItem { Header = "Inner" };
            var more = new MenuItem { Header = "More", Items = { inner, new MenuItem { Header = "Other" } } };
            var menu = Open(more);

            KeyInput.PressOnFocus(menu, Key.Down);
            KeyInput.PressOnFocus(menu, Key.Right);

            Assert.True(more.IsSubMenuOpen);
            Assert.True(inner.IsSelected);
            Assert.True(HasHoverFill(inner));
        }

        [AvaloniaFact]
        public void Down_InAContextMenu_MakesTheShortcutLighterAndReadable()
        {
            var first = new MenuItem { Header = "Duplicate", InputGesture = new KeyGesture(Key.D, KeyModifiers.Control) };
            var second = new MenuItem { Header = "Rename", InputGesture = new KeyGesture(Key.R, KeyModifiers.Control) };
            var menu = Open(first, second);

            KeyInput.PressOnFocus(menu, Key.Down);

            Assert.True(first.IsSelected);
            Assert.Equal(Tokens.Get<Color>("Grey900"), ShortcutOf(first));
            Assert.True(ColorMix.Contrast(ShortcutOf(first), ((ISolidColorBrush)FillOf(first)!).Color) >= 4.5);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ShortcutOf(second));
            Assert.True(ColorMix.Contrast(ShortcutOf(second), Tokens.Get<Color>("PopupCardBackground")) >= 4.5);
        }

        // The submenu: its panel is a PopupFrame and its rows have the theme of the rows of the menu.

        [AvaloniaFact]
        public void Open_TheSubmenuPanel_IsAPopupFrame()
        {
            var submenu = OpenSubmenu(new MenuItem { Header = "One" });

            var popup = submenu.GetVisualDescendants().OfType<Popup>().First(p => p.Name == "PART_Popup");

            Assert.IsType<PopupFrame>(popup.Child);
        }

        [AvaloniaFact]
        public void Open_TheRowsOfTheSubmenu_HaveTheThemeOfTheMenuRows()
        {
            var row = new MenuItem { Header = "One" };

            var submenu = OpenSubmenu(row);

            Assert.Same(Tokens.Theme("MenuItemContext"), submenu.Theme);
            Assert.Same(Tokens.Theme("MenuItemContext"), row.Theme);
        }

        private static MenuItem OpenSubmenu(MenuItem row)
        {
            var submenu = new MenuItem { Header = "Add" };
            submenu.Items.Add(row);
            Open(submenu);
            submenu.IsSubMenuOpen = true;
            Dispatcher.UIThread.RunJobs();
            return submenu;
        }
    }
}
