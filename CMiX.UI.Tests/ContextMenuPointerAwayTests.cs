using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
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
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuInput.Request(drag);
            Assert.True(drag.ContextMenu!.IsOpen);

            PointerInput.Move(window,new Point(790, 490));

            Assert.False(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void TheMenuWrittenInXaml_ClosesWhenThePointerGoesFarAway()
        {
            var selector = new SelectorColor { DataContext = new StringValue(), Width = 200, Height = 40, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var window = TestWindow.Show(selector, 800, 500);
            ContextMenuInput.Request(selector.FindControl<Control>("popupToggle")!);
            Assert.True(selector.ContextMenu!.IsOpen);

            PointerInput.Move(window,new Point(790, 490));

            Assert.False(selector.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void APointerOnTheFarEndOfAWideOwner_KeepsTheMenuOpen()
        {
            var drag = NewDragValue(700);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuInput.Request(drag);
            Assert.True(drag.ContextMenu!.IsOpen);

            PointerInput.Move(window,new Point(690, 15));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void ABiggerDistance_KeepsTheMenuOpenFartherAway()
        {
            var drag = NewDragValue(200);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuInput.Request(drag);
            ContextMenuPointerAway.SetPointerAwayDistance(drag.ContextMenu!, 2000);

            PointerInput.Move(window,new Point(790, 490));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void AMenuWithTheRuleSwitchedOff_StaysOpen()
        {
            var drag = NewDragValue(200);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuPointerAway.SetClosesOnPointerAway(drag.ContextMenu!, false);
            ContextMenuInput.Request(drag);
            Assert.True(drag.ContextMenu.IsOpen);

            PointerInput.Move(window,new Point(790, 490));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void TheMenu_WithACompanion_StaysOpenWhileThePointerIsNearTheCompanion()
        {
            var drag = NewDragValue(200);
            var companion = new Border
            {
                Width = 100, Height = 100,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = Avalonia.Media.Brushes.Transparent,
            };
            var window = TestWindow.Show(new Panel { Children = { drag, companion } }, 800, 500);
            ContextMenuInput.Request(drag);
            ContextMenuPointerAway.SetCompanion(drag.ContextMenu!, companion);

            PointerInput.Move(window,new Point(790, 490));
            Assert.True(drag.ContextMenu.IsOpen);

            ContextMenuPointerAway.SetCompanion(drag.ContextMenu, null);
            PointerInput.Move(window,new Point(789, 489));
            Assert.False(drag.ContextMenu.IsOpen);
        }
    }
}
