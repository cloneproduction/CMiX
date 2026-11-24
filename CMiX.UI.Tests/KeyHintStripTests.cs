using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The hint strip: one quiet line of key caps and words, as high as HintStripHeight, with its own brushes. No click and no focus.
    public class KeyHintStripTests
    {
        private static (KeyHintStrip Strip, Border Behind, Window Window) Show(params KeyHint[] hints)
        {
            var strip = new KeyHintStrip { Hints = hints, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom };
            // A focusable area with a fill under the strip. A fill makes it hit by the pointer.
            var behind = new Border { Background = Brushes.Gray, Focusable = true };
            var area = new Grid();
            area.Children.Add(behind);
            area.Children.Add(strip);
            return (strip, behind, TestWindow.Show(area, 600, 200));
        }

        private static string?[] KeyTexts(Control strip) =>
            strip.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Name == "PART_KeyText").Select(text => text.Text).ToArray();

        private static string?[] Words(KeyHintStrip strip) =>
            strip.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Name != "PART_KeyText").Select(text => text.Text).ToArray();

        [AvaloniaFact]
        public void Hints_TwoHints_ShowTheKeyCapsAndTheWordsInOrder()
        {
            var (strip, _, _) = Show(new KeyHint("E", "open"), new KeyHint(new[] { "Click", "Enter" }, "place"));

            Assert.Equal(new[] { "E", "Click", "Enter" }, KeyTexts(strip));
            Assert.Equal(new[] { "open", KeyHintStrip.KeySeparator, "place" }, Words(strip));
            Assert.All(strip.GetVisualDescendants().OfType<ContentControl>(),
                cap => Assert.Same(Tokens.Get<object>("ContentControlKeyCap"), cap.Theme));
        }

        [AvaloniaFact]
        public void Hints_NewHints_ReplaceTheOldHints()
        {
            var (strip, _, _) = Show(new KeyHint("E", "open"));

            strip.Hints = new[] { new KeyHint("Esc", "close") };
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new[] { "Esc" }, KeyTexts(strip));
            Assert.Equal(new[] { "close" }, Words(strip));
        }

        [AvaloniaFact]
        public void Of_GesturesWithANull_GivesTheTextOfTheOtherGestures()
        {
            var hint = KeyHint.Of("undo", null, new KeyGesture(Key.Z, KeyModifiers.Control), new KeyGesture(Key.D1, KeyModifiers.Control));

            Assert.Equal(new[] { "Ctrl+Z", "Ctrl+1" }, hint!.Keys);
            Assert.Equal("undo", hint.Words);
            Assert.Null(KeyHint.Of("undo", null));
        }

        [AvaloniaFact]
        public void Show_AStrip_HasTheHeightTheSpacingAndTheBrushesOfItsTokens()
        {
            var (strip, _, _) = Show(new KeyHint("E", "open"), new KeyHint("W", "menu"));

            Assert.Equal(Tokens.Get<double>("HintStripHeight"), strip.Bounds.Height);
            Assert.Equal(Tokens.Get<Thickness>("HintStripPadding"), strip.Padding);
            Assert.Equal(Tokens.Get<double>("HintStripSpacing"), strip.Line.Spacing);
            Assert.Equal(Tokens.Get<Color>("Grey200"), ((ISolidColorBrush)strip.Background!).Color);
            Assert.All(Words(strip).Select(word => strip.GetVisualDescendants().OfType<TextBlock>().First(text => text.Text == word)),
                text => Assert.Equal(Tokens.Get<Color>("ShortcutText"), ((ISolidColorBrush)text.Foreground!).Color));
            Assert.All(strip.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Name == "PART_KeyText"),
                text => Assert.Equal(Tokens.Get<FontFamily>("FontFamilyMono"), text.FontFamily));
        }

        [AvaloniaFact]
        public void Show_AStrip_FitsInItsHeight()
        {
            var (strip, _, _) = Show(new KeyHint("Shift+A", "add"));

            var cap = strip.GetVisualDescendants().OfType<ContentControl>().Single();
            Assert.True(cap.Bounds.Height <= strip.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_WithTheFocusBehind_KeepsTheFocusAndLetsTheClickThrough()
        {
            var (strip, behind, window) = Show(new KeyHint("E", "open"));
            behind.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.False(strip.IsHitTestVisible);
            Assert.All(strip.GetSelfAndVisualDescendants().OfType<InputElement>(), element => Assert.False(element.Focusable));

            var pressed = false;
            behind.PointerPressed += (_, _) => pressed = true;
            TestWindow.Render();
            var center = strip.TranslatePoint(new Point(strip.Bounds.Width / 2, strip.Bounds.Height / 2), window)!.Value;
            window.MouseDown(center, MouseButton.Left);
            window.MouseUp(center, MouseButton.Left);

            Assert.True(pressed);
            Assert.Same(behind, window.FocusManager!.GetFocusedElement());
        }
    }
}
