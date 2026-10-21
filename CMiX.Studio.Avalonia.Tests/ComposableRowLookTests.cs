using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row of the layer and composition list has the same colors as every other row of a list.
    public class ComposableRowLookTests
    {
        private static Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        private static ListBoxItem FirstRow()
        {
            var list = new ListBox
            {
                Theme = (ControlTheme)Application.Current!.FindResource("ListBoxComposable")!,
                ItemsSource = new[] { "One", "Two" },
                ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text })
            };
            new Window { Content = list, Width = 300, Height = 300 }.Show();
            Dispatcher.UIThread.RunJobs();
            return list.GetVisualDescendants().OfType<ListBoxItem>().First();
        }

        private static Color? Background(ListBoxItem row) =>
            (row.GetVisualDescendants().OfType<Border>().First(border => border.Name == "Bd").Background as ISolidColorBrush)?.Color;

        private static void Pointer(ListBoxItem row, bool over)
        {
            ((IPseudoClasses)row.Classes).Set(":pointerover", over);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void ARowWithThePointerOver_IsGrey()
        {
            var row = FirstRow();

            Pointer(row, true);

            Assert.Equal(Brush("Grey400Brush"), Background(row));
        }

        [AvaloniaFact]
        public void ASelectedRow_IsInactiveBlue()
        {
            var row = FirstRow();

            row.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Brush("AccentInactiveBrush"), Background(row));
        }

        [AvaloniaFact]
        public void ASelectedRowWithThePointerOver_IsBlue()
        {
            var row = FirstRow();

            row.IsSelected = true;
            Pointer(row, true);

            Assert.Equal(Brush("AccentBrush"), Background(row));
        }
    }
}
