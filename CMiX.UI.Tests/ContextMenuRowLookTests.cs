using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Styling;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The look of a menu: the card around it, and the rows with a shortcut, an arrow, a title or a warning.
    public class ContextMenuRowLookTests
    {
        private static (ContextMenu Menu, Dictionary<string, MenuItem> Rows) Open()
        {
            var add = new MenuItem { Header = "Add", InputGesture = new KeyGesture(Key.A, KeyModifiers.Shift) };
            add.Classes.Add("opens");
            var title = new MenuItem { Header = "Layer 02", IsEnabled = false };
            title.Classes.Add("title");
            var delete = new MenuItem { Header = "Delete", InputGesture = new KeyGesture(Key.Delete) };
            delete.Classes.Add("danger");
            var submenu = new MenuItem { Header = "More", Items = { new MenuItem { Header = "One" } } };
            var menu = new ContextMenu
            {
                Theme = Tokens.Theme("ContextMenuDefault"),
                ItemsSource = new object[]
                {
                    title,
                    new MenuItem { Header = "-" },
                    add,
                    new MenuItem { Header = "Duplicate", InputGesture = new KeyGesture(Key.D, KeyModifiers.Control) },
                    new MenuItem { Header = "Rename" },
                    submenu,
                    delete,
                },
            };
            var owner = new Border { Width = 200, Height = 30, ContextMenu = menu };
            TestWindow.Show(owner, 500, 400);
            menu.Open(owner);
            Dispatcher.UIThread.RunJobs();

            var rows = menu.Items.OfType<MenuItem>().Where(item => item.Header is string header && header != "-")
                .ToDictionary(item => (string)item.Header!);
            return (menu, rows);
        }

        [AvaloniaFact]
        public void Open_AMenu_ShowsItsRowsOnTheCard()
        {
            var (menu, _) = Open();

            var frame = menu.GetVisualDescendants().OfType<PopupFrame>().First();
            var card = frame.GetVisualDescendants().OfType<Border>().First(border => border.CornerRadius != default && border.Background is ISolidColorBrush);

            var fill = Tokens.Color("PopupBackgroundBrush");
            Assert.Equal(fill, ((ISolidColorBrush)card.Background!).Color);
            Assert.Equal(new Thickness(0), card.BorderThickness);
            Assert.Equal(1, card.Opacity);
        }

        [AvaloniaFact]
        public void Open_ARowWithAShortcut_ShowsItInTheMonoFont()
        {
            var (_, rows) = Open();

            var gesture = Parts.Find<TextBlock>(rows["Duplicate"], "PART_InputGestureText");

            Assert.True(gesture.IsEffectivelyVisible);
            Assert.Equal("Ctrl+D", gesture.Text);
            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), gesture.FontFamily);
        }

        [AvaloniaFact]
        public void Open_ARowWithoutAShortcut_ShowsNone()
        {
            var (_, rows) = Open();

            Assert.False(Parts.Find<TextBlock>(rows["Rename"], "PART_InputGestureText").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_TheShortcuts_LineUpAcrossTheRows()
        {
            var (menu, rows) = Open();

            double LeftOf(string header) => Parts.Find<TextBlock>(rows[header], "PART_InputGestureText").TranslatePoint(default, menu)!.Value.X;

            Assert.Equal(LeftOf("Add"), LeftOf("Duplicate"), 0.51);
            Assert.Equal(LeftOf("Add"), LeftOf("Delete"), 0.51);
        }

        [AvaloniaFact]
        public void Open_ARowWithASubmenu_ShowsTheArrow()
        {
            var (_, rows) = Open();

            Assert.True(Parts.Find<AppIcon>(rows["More"], "PART_Arrow").IsEffectivelyVisible);
            Assert.False(Parts.Find<AppIcon>(rows["Rename"], "PART_Arrow").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_ARowOfTheClassOpens_ShowsTheArrowWithoutASubmenu()
        {
            var (_, rows) = Open();

            Assert.True(Parts.Find<AppIcon>(rows["Add"], "PART_Arrow").IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Open_TheTitleRow_IsQuietAndNotDimmed()
        {
            var (_, rows) = Open();

            var root = Parts.Find<Border>(rows["Layer 02"], "root");
            var header = Parts.Find<ContentPresenter>(rows["Layer 02"], "PART_HeaderPresenter");
            var quiet = Tokens.Color("TextMutedBrush");
            Assert.Equal(1, root.Opacity);
            Assert.Equal(quiet, ((ISolidColorBrush)header.Foreground!).Color);
            Assert.Equal(FontWeight.Medium, header.FontWeight);
        }

        [AvaloniaFact]
        public void Open_ARowOfTheClassDanger_HasTheWarningColor()
        {
            var (_, rows) = Open();

            var header = Parts.Find<ContentPresenter>(rows["Delete"], "PART_HeaderPresenter");

            var danger = Tokens.Color("PopupCardDangerBrush");
            Assert.Equal(danger, ((ISolidColorBrush)header.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Open_ADisabledRowThatIsNotATitle_IsDimmed()
        {
            var menu = new ContextMenu
            {
                Theme = Tokens.Theme("ContextMenuDefault"),
                ItemsSource = new object[] { new MenuItem { Header = "Off", IsEnabled = false } },
            };
            var owner = new Border { Width = 200, Height = 30, ContextMenu = menu };
            TestWindow.Show(owner, 500, 400);
            menu.Open(owner);
            Dispatcher.UIThread.RunJobs();

            var row = menu.Items.OfType<MenuItem>().Single();

            Assert.Equal(Tokens.Get<double>("OpacityDisabled"), Parts.Find<Border>(row, "root").Opacity);
        }
    }
}
