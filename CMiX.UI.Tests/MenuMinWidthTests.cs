using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // Every menu has the same least width. The frame adds a margin of 8 on each side for the shadow.
    public class MenuMinWidthTests
    {
        private const double ShadowMargin = 16;

        private static double OpenFrameWidth(string caption)
        {
            var box = new DragValue { Caption = caption, ResetCommand = new RelayCommand(() => { }) };
            new Window { Content = box, Width = 800, Height = 300, FontSize = 12.5 }.Show();
            Dispatcher.UIThread.RunJobs();

            box.ContextMenu!.Open(box);
            Dispatcher.UIThread.RunJobs();

            return box.ContextMenu.GetVisualDescendants().OfType<PopupFrame>().Single().Bounds.Width;
        }

        [AvaloniaFact]
        public void TheToken_IsTwoHundred()
        {
            Assert.Equal(200.0, (double)Application.Current!.FindResource("PopupMinWidth")!);
        }

        [AvaloniaFact]
        public void AShortMenu_HasTheLeastWidth()
        {
            Assert.Equal(200 + ShadowMargin, OpenFrameWidth("X"));
        }

        [AvaloniaFact]
        public void ALongRow_MakesTheMenuWider()
        {
            Assert.True(OpenFrameWidth(new string('W', 60)) > 200 + ShadowMargin);
        }
    }
}
