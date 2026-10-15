using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Number;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // The menu of a value editor starts with the name of the control, then a line, then the rows.
    public class ResetMenuTitleTests
    {
        private static DragValue Show(DragValue box)
        {
            box.ResetCommand = new RelayCommand(() => { });
            new Window { Content = box, Width = 300, Height = 60 }.Show();
            Dispatcher.UIThread.RunJobs();
            return box;
        }

        private static MenuItem[] Rows(Control box) => box.ContextMenu!.Items.OfType<MenuItem>().ToArray();

        [AvaloniaFact]
        public void TheTitle_IsTheCaption_WithoutTheSpacesAtTheEnds()
        {
            var box = Show(new DragValue { Caption = " Position X" });

            var rows = Rows(box);
            Assert.Equal("Position X", rows[0].Header);
            Assert.False(rows[0].IsEnabled);
            Assert.Equal("-", rows[1].Header);
            Assert.Equal("Copy", rows[2].Header);
        }

        [AvaloniaFact]
        public void TheTitle_IsTheControlTitle_WhenTheCaptionIsOnlyAPartOfTheName()
        {
            var box = new DragValue { Caption = "Y" };
            ControlTitle.SetText(box, "Position Y");
            Show(box);

            Assert.Equal("Position Y", Rows(box)[0].Header);
        }

        [AvaloniaFact]
        public void WithoutAName_TheTitleAndTheLineAreHidden()
        {
            var box = Show(new DragValue());

            var rows = Rows(box);
            Assert.False(rows[0].IsVisible);
            Assert.False(rows[1].IsVisible);
            Assert.True(rows[2].IsVisible);
        }

        [AvaloniaFact]
        public void WithResetAll_TheMenuEndsWithResetAndResetAll()
        {
            var box = new DragValue { Caption = "Position X", ResetAllCommand = new RelayCommand(() => { }) };
            Show(box);

            Assert.Equal(new object?[] { "Position X", "-", "Copy", "Paste", "-", "Reset", "Reset All" }, Rows(box).Select(r => r.Header).ToArray());
        }
    }
}
