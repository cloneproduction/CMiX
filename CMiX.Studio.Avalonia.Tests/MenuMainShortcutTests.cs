using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row of the main menu shows the gesture of the shortcut map through InputGesture, with the shortcut look of every menu.
    public class MenuMainShortcutTests
    {
        private static (TextBlock Gesture, ViewModels.MainViewModel ViewModel) OpenAndFindGesture(string top, string row)
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            var rowItem = MenuMainRows.OpenRow(window, top, row);
            return (rowItem.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_InputGestureText"), viewModel);
        }

        [AvaloniaTheory]
        [InlineData("Edit", "Undo", "Edit.Undo")]
        [InlineData("Edit", "Redo", "Edit.Redo")]
        [InlineData("File", "Open", "File.Open")]
        public void Open_AMenuMainRow_ShowsTheGestureOfTheShortcutMap(string top, string row, string shortcut)
        {
            var (gesture, viewModel) = OpenAndFindGesture(top, row);

            var expected = viewModel.Shortcuts[shortcut];
            Assert.True(gesture.IsEffectivelyVisible);
            Assert.Equal(expected!.ToString(), gesture.Text);
        }

        [AvaloniaFact]
        public void Open_TheUndoRow_ShowsCtrlZInTheShortcutLook()
        {
            var (gesture, _) = OpenAndFindGesture("Edit", "Undo");

            var application = global::Avalonia.Application.Current!;
            Assert.Equal("Ctrl+Z", gesture.Text);
            Assert.Equal((FontFamily)application.FindResource("FontFamilyMono")!, gesture.FontFamily);
            Assert.Equal(((ISolidColorBrush)application.FindResource("ShortcutTextBrush")!).Color, ((ISolidColorBrush)gesture.Foreground!).Color);
        }
    }
}
