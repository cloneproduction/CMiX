using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using Xunit;

namespace CMiX.UI.Tests
{
    // The list of groups with a search field: the state of the groups, the search, the keys and the clicks.
    public class ListGroupedSearchTests
    {
        private static readonly string[] Content = { "Entity", "Fullscreen quad", "Text", "Camera", "Light" };

        private static readonly string[] Filters =
            { "Alpha", "Ascii", "Blend", "Blur", "Displace", "Dither", "Echo", "Edge", "Feedback", "HSCB", "Halftone", "Kaleidoscope" };

        private static readonly string[] Modifiers =
            { "Orbit", "Target", "Zoom", "Alpha", "Color", "Visibility", "Flip", "Grid", "Rotation", "Scale" };

        private static PickerGroup Group(string header, string[] names, string kind) =>
            new(header, names.Select(name => new PickerItem(name, name, kind)).ToList(), kind);

        private static (Window Window, ListGroupedSearch List, List<PickerPickedEventArgs> Picks) Show()
        {
            var picks = new List<PickerPickedEventArgs>();
            var list = new ListGroupedSearch
            {
                Groups = new[] { Group("Content", Content, "content"), Group("Filter", Filters, "filter"), Group("Modifier", Modifiers, "modifier") },
                KindBrush = kind => kind switch
                {
                    "content" => Brushes.Red,
                    "filter" => Brushes.Green,
                    _ => null,
                },
                Width = 240,
            };
            list.Picked += (_, e) => picks.Add(e);
            var window = new Window { Content = list, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, list, picks);
        }

        private static TextBox Search(ListGroupedSearch list) =>
            list.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");

        private static void Type(ListGroupedSearch list, string text)
        {
            Search(list).Text = text;
            Dispatcher.UIThread.RunJobs();
        }

        private static void Press(ListGroupedSearch list, Key key, int times = 1)
        {
            for (var i = 0; i < times; i++)
                Search(list).RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });

            Dispatcher.UIThread.RunJobs();
        }

        private static void Click(Window window, Control control)
        {
            window.UpdateLayout();
            var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        private static string[] Opening { get; } = { "Content", "Entity", "Fullscreen quad", "Text", "Camera", "Light", "Filter", "Modifier" };

        [AvaloniaFact]
        public void Show_TheGroups_OpensTheSmallGroupAndClosesTheLargeOnes()
        {
            var (_, list, _) = Show();

            Assert.Equal(Opening, list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Show_NotCollapsible_OpensEveryGroupAndTheHeadersAreOnlyNames()
        {
            var (window, list, picks) = Show();
            list.Collapsible = false;
            Dispatcher.UIThread.RunJobs();

            Assert.All(list.Groups!, group => Assert.True(list.IsOpen(group)));
            Assert.Empty(list.GetVisualDescendants().OfType<AppIcon>().Where(icon => icon.IconKey is "ChevronDown" or "ChevronRight"));

            // A click on a header changes nothing and picks nothing, and the highlight skips the headers.
            Click(window, list.RowVisuals.First(row => row.Classes.Contains("header")));
            Assert.Empty(picks);
            Assert.All(list.Groups!, group => Assert.True(list.IsOpen(group)));
            Assert.DoesNotContain("header", list.RowVisuals[list.HighlightedIndex].Classes);
        }

        [AvaloniaFact]
        public void Show_TheGroups_HighlightsTheFirstRowNotTheFirstHeader()
        {
            var (_, list, _) = Show();

            Assert.Equal(1, list.HighlightedIndex);
            Assert.True(list.RowVisuals[1].Classes.Contains("highlighted"));
            Assert.False(list.RowVisuals[0].Classes.Contains("highlighted"));
        }

        [AvaloniaFact]
        public void Show_TheRows_AreOneControlHeightHigh()
        {
            var (_, list, _) = Show();

            var height = (double)Application.Current!.FindResource("ControlHeightMD")!;
            Assert.All(list.RowVisuals, row => Assert.Equal(height, row.Bounds.Height));
        }

        [AvaloniaFact]
        public void Show_ARowWithAColoredKind_ShowsASquareInThatColorAndNoIcon()
        {
            var (_, list, _) = Show();

            var entity = list.RowVisuals[1];
            var children = ((StackPanel)entity.Child!).Children;
            var square = children.OfType<Border>().Single();

            Assert.Empty(children.OfType<AppIcon>());
            Assert.Equal(Colors.Red, ((ISolidColorBrush)square.Background!).Color);
        }

        [AvaloniaFact]
        public void Show_ARowWithoutAColor_ShowsOnlyItsText()
        {
            var list = new ListGroupedSearch
            {
                Groups = new[] { new PickerGroup("Group", new[] { new PickerItem("No color") }) },
                Width = 240,
            };
            new Window { Content = list, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.All(list.RowVisuals.Skip(1), row => Assert.Single(((StackPanel)row.Child!).Children));
            Assert.Equal(new[] { "Group", "No color" }, list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Show_AGroupHeader_HasNoCount()
        {
            var (_, list, _) = Show();

            Assert.Equal(new[] { "Content", "Filter", "Modifier" }, list.VisibleTexts.Where((_, index) => list.RowVisuals[index].Classes.Contains("header")));
        }


        private static ListGroupedSearch ShowWithDisabledRows(out List<PickerPickedEventArgs> picks)
        {
            var picked = new List<PickerPickedEventArgs>();
            var list = new ListGroupedSearch
            {
                Groups = new[]
                {
                    new PickerGroup("Open", new[] { new PickerItem("First"), new PickerItem("Off", IsEnabled: false), new PickerItem("Last") }),
                    new PickerGroup("Nothing", new[] { new PickerItem("Never", IsEnabled: false) }),
                },
                Width = 240,
            };
            list.Picked += (_, e) => picked.Add(e);
            new Window { Content = list, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();
            picks = picked;
            return list;
        }

        [AvaloniaFact]
        public void Show_ARowThatIsNotEnabled_IsGreyed()
        {
            var list = ShowWithDisabledRows(out _);

            var off = list.RowVisuals[2];

            Assert.Contains("disabled", off.Classes);
            Assert.Equal(0.4, off.Opacity);
            Assert.DoesNotContain("disabled", list.RowVisuals[1].Classes);
        }

        [AvaloniaFact]
        public void Show_AGroupWithNoEnabledRow_StartsClosedWithAGreyedHeader()
        {
            var list = ShowWithDisabledRows(out _);

            Assert.Equal(new[] { "Open", "First", "Off", "Last", "Nothing" }, list.VisibleTexts);
            Assert.Contains("dimmed", list.RowVisuals[4].Classes);
            Assert.Equal(0.5, list.RowVisuals[4].Opacity);
        }

        [AvaloniaFact]
        public void Press_DownAndUp_SkipARowThatIsNotEnabled()
        {
            var list = ShowWithDisabledRows(out _);
            Assert.Equal(1, list.HighlightedIndex);

            Press(list, Key.Down);
            Assert.Equal(3, list.HighlightedIndex);

            Press(list, Key.Up);
            Assert.Equal(1, list.HighlightedIndex);
        }

        [AvaloniaFact]
        public void Click_ARowThatIsNotEnabled_PicksNothingAndMovesNoHighlight()
        {
            var list = ShowWithDisabledRows(out var picks);
            var window = (Window)list.GetVisualRoot()!;

            Click(window, list.RowVisuals[2]);

            Assert.Empty(picks);
            Assert.Equal(1, list.HighlightedIndex);
        }

        [AvaloniaFact]
        public void Type_TextOfARowThatIsNotEnabled_ShowsItGreyedAndPicksNothing()
        {
            var list = ShowWithDisabledRows(out var picks);

            Type(list, "off");
            Press(list, Key.Enter);

            Assert.Equal(new[] { "Open", "Off" }, list.VisibleTexts);
            Assert.Contains("disabled", list.RowVisuals[1].Classes);
            Assert.Empty(picks);
        }

        [AvaloniaFact]
        public void Show_TheRows_HaveAnOverlayScrollBar()
        {
            var (_, list, _) = Show();

            var scroll = list.GetVisualDescendants().OfType<ScrollViewer>().Single(viewer => viewer.Name == "PART_Scroll");

            Assert.Equal(ScrollBarVisibility.Auto, scroll.VerticalScrollBarVisibility);
            Assert.Same(Application.Current!.FindResource("ScrollViewerOverlay"), scroll.Theme);
        }

        [AvaloniaFact]
        public void Click_AClosedHeader_OpensItsGroup()
        {
            var (window, list, _) = Show();

            Click(window, list.RowVisuals[6]);

            Assert.Equal(Opening.Take(7).Concat(Filters).Append("Modifier"), list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Click_AnOpenHeader_ClosesItsGroup()
        {
            var (window, list, _) = Show();

            Click(window, list.RowVisuals[0]);

            Assert.Equal(new[] { "Content", "Filter", "Modifier" }, list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Click_ARow_RaisesPickedWithTheRowAndItsGroup()
        {
            var (window, list, picks) = Show();

            Click(window, list.RowVisuals[3]);

            var pick = Assert.Single(picks);
            Assert.Equal("Text", pick.Item.Text);
            Assert.Equal("Content", pick.Group.Header);
        }

        [AvaloniaFact]
        public void Type_Text_ShowsTheMatchingRowsOfEveryGroupAndOpensThem()
        {
            var (_, list, _) = Show();

            Type(list, "bl");

            Assert.Equal(new[] { "Filter", "Blend", "Blur" }, list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Type_Text_MatchesInAnyCaseAndInEveryGroup()
        {
            var (_, list, _) = Show();

            Type(list, "AL");

            Assert.Equal(new[] { "Filter", "Alpha", "Halftone", "Kaleidoscope", "Modifier", "Alpha", "Scale" }, list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Type_ThenClear_GoesBackToTheFirstState()
        {
            var (_, list, _) = Show();

            Type(list, "bl");
            Type(list, "");

            Assert.Equal(Opening, list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Type_TextWithNoMatch_ShowsTheNoMatchLine()
        {
            var (_, list, _) = Show();

            Type(list, "zzz");

            var line = list.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_NoMatch");
            Assert.Empty(list.VisibleTexts);
            Assert.True(line.IsVisible);
        }

        [AvaloniaFact]
        public void Press_DownAndUp_MoveTheHighlightOverHeadersAndRows()
        {
            var (_, list, _) = Show();

            Press(list, Key.Down);
            Assert.Equal(2, list.HighlightedIndex);

            Press(list, Key.Up, 2);
            Assert.Equal(0, list.HighlightedIndex);

            Press(list, Key.Up);
            Assert.Equal(0, list.HighlightedIndex);

            Press(list, Key.Down, 50);
            Assert.Equal(list.VisibleTexts.Count - 1, list.HighlightedIndex);
        }

        [AvaloniaFact]
        public void Press_EnterOnARow_RaisesPicked()
        {
            var (_, list, picks) = Show();

            Press(list, Key.Down);
            Press(list, Key.Enter);

            Assert.Equal("Fullscreen quad", Assert.Single(picks).Item.Text);
        }

        [AvaloniaFact]
        public void Press_EnterOnAHeader_OpensTheGroupAndKeepsTheHighlightOnIt()
        {
            var (_, list, picks) = Show();

            Press(list, Key.Down, 5);
            Assert.Equal("Filter", list.VisibleTexts[list.HighlightedIndex]);
            Press(list, Key.Enter);

            Assert.Empty(picks);
            Assert.Contains("Alpha", list.VisibleTexts);
            Assert.Equal("Filter", list.VisibleTexts[list.HighlightedIndex]);
        }

        [AvaloniaFact]
        public void Press_RightAndLeftOnAHeader_OpenAndCloseTheGroup()
        {
            var (_, list, _) = Show();

            Press(list, Key.Down, 5);
            Press(list, Key.Right);
            Assert.Contains("Alpha", list.VisibleTexts);

            Press(list, Key.Left);
            Assert.DoesNotContain("Alpha", list.VisibleTexts);
        }

        [AvaloniaFact]
        public void Press_EnterWhileSearching_PicksTheFirstMatch()
        {
            var (_, list, picks) = Show();

            Type(list, "bl");
            Press(list, Key.Enter);

            Assert.Equal("Blend", Assert.Single(picks).Item.Text);
        }

        [AvaloniaFact]
        public void Press_Escape_ClearsTheTextThenRaisesCancelled()
        {
            var (_, list, _) = Show();
            var cancelled = 0;
            list.Cancelled += (_, _) => cancelled++;

            Type(list, "bl");
            Press(list, Key.Escape);
            Assert.Equal(string.Empty, Search(list).Text);
            Assert.Equal(0, cancelled);

            Press(list, Key.Escape);
            Assert.Equal(1, cancelled);
        }

        [AvaloniaFact]
        public void Move_ThePointerOverARow_MovesTheHighlightToIt()
        {
            var (window, list, _) = Show();
            window.UpdateLayout();

            var row = list.RowVisuals[4];
            window.MouseMove(row.TranslatePoint(new Point(10, 10), window)!.Value);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(4, list.HighlightedIndex);
        }

        [AvaloniaFact]
        public void CollapseFrom_Changed_ChangesWhichGroupsStartClosed()
        {
            var (_, list, _) = Show();

            list.CollapseFrom = 4;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new[] { "Content", "Filter", "Modifier" }, list.VisibleTexts);
        }
    }
}
