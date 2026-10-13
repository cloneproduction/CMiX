using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // A context menu with the default theme, such as the Reset menu of a value editor, closes when the pointer
    // goes far from it and from the control that owns it.
    public class ContextMenuPointerAwayTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 800, Height = 500 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static void OpenMenu(Control source)
        {
            source.RaiseEvent(new ContextRequestedEventArgs { Source = source });
            Dispatcher.UIThread.RunJobs();
        }

        private static void Move(Window window, Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        private static DragValue NewDragValue(double width) => new()
        {
            Width = width,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            ResetCommand = new RelayCommand(() => { }),
        };

        [AvaloniaFact]
        public void TheResetMenuOfADragValue_ClosesWhenThePointerGoesFarAway()
        {
            var drag = NewDragValue(200);
            var window = Host(drag);
            OpenMenu(drag);
            Assert.True(drag.ContextMenu!.IsOpen);

            Move(window, new Point(790, 490));

            Assert.False(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void TheMenuWrittenInXaml_ClosesWhenThePointerGoesFarAway()
        {
            var selector = new ColorSelector { DataContext = new StringValue(), Width = 200, Height = 40, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var window = Host(selector);
            OpenMenu(selector.FindControl<Control>("PopupToggle")!);
            Assert.True(selector.ContextMenu!.IsOpen);

            Move(window, new Point(790, 490));

            Assert.False(selector.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void APointerOnTheFarEndOfAWideOwner_KeepsTheMenuOpen()
        {
            var drag = NewDragValue(700);
            var window = Host(drag);
            OpenMenu(drag);
            Assert.True(drag.ContextMenu!.IsOpen);

            Move(window, new Point(690, 15));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void ABiggerDistance_KeepsTheMenuOpenFartherAway()
        {
            var drag = NewDragValue(200);
            var window = Host(drag);
            OpenMenu(drag);
            ContextMenuPointerAway.SetDistance(drag.ContextMenu!, 2000);

            Move(window, new Point(790, 490));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void AMenuWithTheRuleSwitchedOff_StaysOpen()
        {
            var drag = NewDragValue(200);
            var window = Host(drag);
            ContextMenuPointerAway.SetIsEnabled(drag.ContextMenu!, false);
            OpenMenu(drag);
            Assert.True(drag.ContextMenu.IsOpen);

            Move(window, new Point(790, 490));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void TheDistance_IsSixtyFourByDefault()
        {
            Assert.Equal(64, ContextMenuPointerAway.GetDistance(new ContextMenu()));
        }
    }
}
