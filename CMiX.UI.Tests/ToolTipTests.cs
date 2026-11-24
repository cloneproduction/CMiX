using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The tooltip: the placement (custom by default, kept when set, follows the pointer, flips, the side),
    // the look of the card, the lines of a TipContent and the record itself.
    public class ToolTipTests
    {
        // A tooltip opens on one side of its control, 8 pixels away, and follows the pointer along the control. Its tip points at the pointer.

        private static (Window Window, Button Button) Show(double left = 40, double top = 100, double width = 80)
        {
            var button = new Button
            {
                Content = "Hover", Width = width, Height = 24,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(left, top, 0, 0),
            };
            ToolTip.SetTip(button, "A tip");
            var window = TestWindow.Show(button, 400, 300);
            ToolTipDefaults.AnchoringOf(button)!.Bounds = new Rect(0, 0, 400, 300);
            return (window, button);
        }

        private static (ToolTipCard Card, Rect Box) Open(Window window, Button button)
        {
            ToolTip.SetIsOpen(button, true);
            Dispatcher.UIThread.RunJobs();
            return Read(window);
        }

        private static (ToolTipCard Card, Rect Box) Read(Window window)
        {
            var card = window.GetVisualDescendants().OfType<ToolTipCard>().First();
            var box = card.GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_Surface");
            return (card, new Rect(box.TranslatePoint(default, window)!.Value, box.Bounds.Size));
        }

        [AvaloniaFact]
        public void Placement_ByDefault_IsCustomForEveryControl()
        {
            foreach (var control in new Control[] { new Button(), new TextBlock(), new Border(), new ToggleButton(), new CMiX.UI.Controls.AppIcon() })
            {
                ToolTip.SetTip(control, "A tip");
                Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(control));
                Assert.NotNull(ToolTip.GetCustomPopupPlacementCallback(control));
            }
        }

        [AvaloniaFact]
        public void Placement_SetForOneControl_StaysAsSet()
        {
            var button = new Button();
            ToolTip.SetPlacement(button, PlacementMode.Right);
            ToolTip.SetVerticalOffset(button, 3);

            ToolTip.SetTip(button, "A tip");

            Assert.Equal(PlacementMode.Right, ToolTip.GetPlacement(button));
            Assert.Equal(3, ToolTip.GetVerticalOffset(button));
            Assert.Null(ToolTip.GetCustomPopupPlacementCallback(button));
        }

        [AvaloniaFact]
        public void Placement_ATipFromABinding_IsSetToo()
        {
            var button = new Button { DataContext = new { Text = "From a binding" } };
            button.Bind(ToolTip.TipProperty, new Avalonia.Data.Binding("Text"));

            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(button));
        }

        [AvaloniaFact]
        public void Show_TheTooltipTheme_HasTheShadowRoomOfTheOtherPopups()
        {
            var (window, button) = Show();

            var (card, box) = Open(window, button);

            var origin = card.TranslatePoint(default, window)!.Value;
            Assert.Equal(PopupFrame.ShadowMargin.Left, box.X - origin.X, 0.51);
            Assert.Equal(PopupFrame.ShadowMargin.Top, box.Y - origin.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_ATooltip_IsEightPixelsBelowTheControl()
        {
            var (window, button) = Show();

            var (_, box) = Open(window, button);

            Assert.Equal(100 + 24 + ToolTipDefaults.Gap, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_ATooltip_ShowsTheTipOnTheTopEdge()
        {
            var (window, button) = Show();

            var (card, _) = Open(window, button);

            Assert.True(card.ShowsTip);
            Assert.Equal(PopupSide.Top, card.TipEdge);
        }

        [AvaloniaFact]
        public void Open_WithThePointerOnTheControl_PutsTheCardAndTheTipOnThePointer()
        {
            var (window, button) = Show(width: 300);
            PointerInput.Move(window, new Point(150, 110));

            var (card, box) = Open(window, button);

            Assert.Equal(150 - ToolTipDefaults.PointerAlign, box.X, 0.51);
            Assert.Equal(ToolTipDefaults.PointerAlign, card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Move_ThePointerWhileTheTooltipIsOpen_KeepsTheCardWhereItOpened()
        {
            var (window, button) = Show(width: 300);
            PointerInput.Move(window, new Point(150, 110));
            Open(window, button);

            PointerInput.Move(window, new Point(220, 110));

            var (card, box) = Read(window);
            Assert.Equal(150 - ToolTipDefaults.PointerAlign, box.X, 0.51);
            Assert.Equal(ToolTipDefaults.PointerAlign, card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Open_WithThePointerNearTheRightEdge_StopsAtTheMarginAndKeepsTheTipOnThePointer()
        {
            var (window, button) = Show(left: 20, width: 360);
            PointerInput.Move(window, new Point(370, 110));

            var (card, box) = Open(window, button);

            Assert.Equal(400 - 8, box.Right, 0.51);
            Assert.Equal(370 - box.X, card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Open_NoRoomBelow_OpensAboveWithTheTipOnTheBottomEdge()
        {
            var (window, button) = Show(top: 270);

            var (card, box) = Open(window, button);

            Assert.Equal(270 - ToolTipDefaults.Gap, box.Bottom, 0.51);
            Assert.Equal(PopupSide.Bottom, card.TipEdge);
        }

        [AvaloniaTheory]
        [InlineData(PopupSide.Right)]
        [InlineData(PopupSide.Left)]
        public void Open_ASideTooltip_OpensBesideTheControlAndFollowsThePointerInY(PopupSide side)
        {
            var (window, button) = Show(left: 150, top: 100, width: 80);
            ToolTipDefaults.SetSide(button, side);
            PointerInput.Move(window, new Point(190, 110));

            var (card, box) = Open(window, button);

            if (side == PopupSide.Right)
                Assert.Equal(150 + 80 + ToolTipDefaults.Gap, box.X, 0.51);
            else
                Assert.Equal(150 - ToolTipDefaults.Gap, box.Right, 0.51);

            Assert.Equal(110 - ToolTipDefaults.PointerAlign, box.Y, 0.51);
            Assert.Equal(side == PopupSide.Right ? PopupSide.Left : PopupSide.Right, card.TipEdge);
        }

        // A tooltip is a card: the color of the token PopupBackgroundBrush, with no outline. A string shows as a title, and a
        // TipContent shows its lines.

        private static ToolTip ShowTip(object content)
        {
            var tip = new ToolTip { Content = content };
            TestWindow.Show(tip, 300, 200);
            return tip;
        }

        private static string[] Texts(ToolTip tip) =>
            tip.GetVisualDescendants().OfType<TextBlock>().Where(text => text.IsEffectivelyVisible).Select(text => text.Text ?? string.Empty).ToArray();

        [AvaloniaFact]
        public void Show_ATooltip_HasTheLookOfTheCard()
        {
            var tip = ShowTip("Tip");

            var card = tip.GetVisualDescendants().OfType<Border>().First(part => part.Name == "PART_Surface");

            var fill = Tokens.Color("PopupBackgroundBrush");
            Assert.Equal(fill, ((ISolidColorBrush)card.Background!).Color);
            Assert.Equal(new Thickness(0), card.BorderThickness);
        }

        [AvaloniaFact]
        public void Show_AString_ShowsAsAMediumTitle()
        {
            var tip = ShowTip("Add");

            var title = tip.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Text == "Add");
            Assert.Equal(FontWeight.Medium, title.FontWeight);
            Assert.Equal(new[] { "Add" }, Texts(tip));
        }

        [AvaloniaFact]
        public void Show_ATipContent_ShowsOnlyTheLinesThatItHas()
        {
            var tip = ShowTip(new TipContent("Add", "Shift+A", "Adds under the row."));

            Assert.Equal(new[] { "Add", "Shift+A", "Adds under the row." }, Texts(tip));
        }

        [AvaloniaFact]
        public void Show_ATipContentWithAllTheLines_ShowsThemInOrder()
        {
            var tip = ShowTip(new TipContent("Remove", "Del", "Takes the row out.", "Select a row first.", "Tree.Remove"));

            Assert.Equal(new[] { "Remove", "Del", "Takes the row out.", "Select a row first.", "Tree.Remove" }, Texts(tip));
        }

        [AvaloniaFact]
        public void Show_TheReasonLine_HasTheWarningColor()
        {
            var tip = ShowTip(new TipContent("Remove", Reason: "Select a row first."));

            var reason = tip.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Text == "Select a row first.");
            var warning = Tokens.Color("PopupCardWarningBrush");
            Assert.Equal(warning, ((ISolidColorBrush)reason.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Show_TheShortcutAndTheFooter_UseTheMonoFont()
        {
            var tip = ShowTip(new TipContent("Add", "Shift+A", Footer: "Tree.Add"));

            var mono = Tokens.Get<FontFamily>("FontFamilyMono");
            Assert.All(new[] { "Shift+A", "Tree.Add" }, text =>
                Assert.Equal(mono, tip.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Text == text).FontFamily));
        }

        // The last line of a tooltip is quiet, and it can still be read: TextMutedBrush has a contrast of 4.5:1 or more on the card.
        [AvaloniaFact]
        public void Show_TheShortcutOfATooltip_HasTheShortcutLook()
        {
            var tip = new ToolTip { Content = new TipContent("Add", "Shift+A") };
            TestWindow.Show(tip, 300, 200);

            var text = tip.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Text == "Shift+A");

            Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), text.FontFamily);
            Assert.Equal(Tokens.Get<Color>("ShortcutText"), ((ISolidColorBrush)text.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Show_TheFooter_HasTheMutedTextColorThatCanBeRead()
        {
            var tip = ShowTip(new TipContent("Add", "Shift+A", Footer: "Tree.Add"));

            var footer = tip.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Text == "Tree.Add");
            var muted = Tokens.Color("TextMutedBrush");
            var card = Tokens.Color("PopupBackgroundBrush");

            Assert.Equal(muted, ((ISolidColorBrush)footer.Foreground!).Color);
            Assert.True(CMiX.UI.Mathematics.ColorMix.Contrast(muted, card) >= 4.5);
        }

        // The lines of a tooltip. Only the title is needed.

        [Fact]
        public void Create_WithATitleOnly_HasNoOtherLine()
        {
            var tip = new TipContent("Add");

            Assert.Equal("Add", tip.Title);
            Assert.Null(tip.Shortcut);
            Assert.Null(tip.Description);
            Assert.Null(tip.Reason);
            Assert.Null(tip.Footer);
        }

        [Fact]
        public void ToString_OfATip_IsItsTitle()
        {
            Assert.Equal("Remove", new TipContent("Remove", "Del", "Takes the row out.").ToString());
        }

        [Fact]
        public void Equals_TwoTipsWithTheSameLines_AreEqual()
        {
            Assert.Equal(new TipContent("Add", "Shift+A"), new TipContent("Add", "Shift+A"));
            Assert.NotEqual(new TipContent("Add", "Shift+A"), new TipContent("Add"));
        }
    }
}
