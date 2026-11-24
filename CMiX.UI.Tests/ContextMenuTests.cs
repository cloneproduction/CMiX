using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // The Reset menu of a value editor: it opens on a context request, but never in edit mode or for a request
    // from a popup below the editor, and it closes when the pointer goes far from it and from its owner.
    public class ContextMenuTests
    {
        // The Reset menu opens on a context request, but never in edit mode or for a request from a popup below the editor.
        // The headless mouse raises a context request only in edit mode. So the normal case and the popup case raise it by hand.

        // Presses and releases the right button on the center of the control. Returns how many context requests it raised.
        private static int RightClick(Window window, Control control)
        {
            var requests = 0;
            control.AddHandler(Control.ContextRequestedEvent, (_, _) => requests++, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);

            var center = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
            PointerInput.Click(window, center, MouseButton.Right);
            return requests;
        }

        private static SliderCMiX NewSlider() =>
            new() { Width = 200, Height = 30, ResetCommand = new RelayCommand(() => { }) };

        private static DragValue NewDragValue() =>
            new() { Width = 200, Height = 30, ResetCommand = new RelayCommand(() => { }) };

        [AvaloniaFact]
        public void Slider_ContextRequest_OpensTheMenu()
        {
            var slider = NewSlider();
            TestWindow.Show(slider, 300, 200);

            ContextMenuInput.Request(slider);

            Assert.True(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void Slider_RightClickInEditMode_EndsTheEditAndKeepsTheMenuClosed()
        {
            var slider = NewSlider();
            var window = TestWindow.Show(slider, 300, 200);
            slider.IsEditing = true;

            var requests = RightClick(window, slider);

            Assert.True(requests > 0, "The right-click raised no context request.");
            Assert.False(slider.IsEditing);
            Assert.False(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void Slider_ContextRequestWhileEditing_OpensNoMenu()
        {
            var slider = NewSlider();
            TestWindow.Show(slider, 300, 200);
            slider.IsEditing = true;

            ContextMenuInput.Request(slider);

            Assert.False(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void Slider_ContextRequestAfterTheEditModeClick_OpensTheMenu()
        {
            var slider = NewSlider();
            var window = TestWindow.Show(slider, 300, 200);
            slider.IsEditing = true;
            RightClick(window, slider);

            ContextMenuInput.Request(slider);

            Assert.True(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void DragValue_ContextRequest_OpensTheMenu()
        {
            var drag = NewDragValue();
            TestWindow.Show(drag, 300, 200);

            ContextMenuInput.Request(drag);

            Assert.True(drag.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void DragValue_ContextRequestWhileEditing_OpensNoMenu()
        {
            var drag = NewDragValue();
            TestWindow.Show(drag, 300, 200);
            drag.IsEditing = true;

            ContextMenuInput.Request(drag);

            Assert.False(drag.ContextMenu!.IsOpen);
        }

        private static (SelectorColor Selector, Popup Popup) ShowSelectorColorWithOpenPopup()
        {
            var color = new StringValue();
            var selector = new SelectorColor { DataContext = color, Width = 200, Height = 40 };
            TestWindow.Show(selector, 300, 200);

            selector.FindControl<ToggleButton>("popupToggle")!.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            var popup = selector.FindControl<Popup>("colorPickerPopup")!;
            Assert.True(popup.IsOpen);
            return (selector, popup);
        }

        [AvaloniaFact]
        public void SelectorColor_ContextRequestOnTheSwatch_OpensTheResetMenu()
        {
            var (selector, _) = ShowSelectorColorWithOpenPopup();

            ContextMenuInput.Request(selector.FindControl<ToggleButton>("popupToggle")!);

            Assert.True(selector.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void SelectorColor_ContextRequestInsideThePopup_OpensNoMenu()
        {
            var (selector, popup) = ShowSelectorColorWithOpenPopup();
            var insidePopup = popup.Child!.GetVisualDescendants().OfType<SliderCMiX>().First();

            ContextMenuInput.Request(insidePopup);

            Assert.False(selector.ContextMenu!.IsOpen);
        }

        // A context menu with the default theme, such as the Reset menu of a value editor, closes when the pointer
        // goes far from it and from the control that owns it.

        private static DragValue NewDragValueAtTheTop(double width) => new()
        {
            Width = width,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            ResetCommand = new RelayCommand(() => { }),
        };

        [AvaloniaFact]
        public void PointerAway_FromTheResetMenuOfADragValue_ClosesTheMenu()
        {
            var drag = NewDragValueAtTheTop(200);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuInput.Request(drag);
            Assert.True(drag.ContextMenu!.IsOpen);

            PointerInput.Move(window, new Point(790, 490));

            Assert.False(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_FromAMenuWrittenInXaml_ClosesTheMenu()
        {
            var selector = new SelectorColor { DataContext = new StringValue(), Width = 200, Height = 40, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var window = TestWindow.Show(selector, 800, 500);
            ContextMenuInput.Request(selector.FindControl<Control>("popupToggle")!);
            Assert.True(selector.ContextMenu!.IsOpen);

            PointerInput.Move(window, new Point(790, 490));

            Assert.False(selector.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_OnTheFarEndOfAWideOwner_KeepsTheMenuOpen()
        {
            var drag = NewDragValueAtTheTop(700);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuInput.Request(drag);
            Assert.True(drag.ContextMenu!.IsOpen);

            PointerInput.Move(window, new Point(690, 15));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_ABiggerDistance_KeepsTheMenuOpenFartherAway()
        {
            var drag = NewDragValueAtTheTop(200);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuInput.Request(drag);
            ContextMenuPointerAway.SetPointerAwayDistance(drag.ContextMenu!, 2000);

            PointerInput.Move(window, new Point(790, 490));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_WithTheRuleSwitchedOff_KeepsTheMenuOpen()
        {
            var drag = NewDragValueAtTheTop(200);
            var window = TestWindow.Show(drag, 800, 500);
            ContextMenuPointerAway.SetClosesOnPointerAway(drag.ContextMenu!, false);
            ContextMenuInput.Request(drag);
            Assert.True(drag.ContextMenu.IsOpen);

            PointerInput.Move(window, new Point(790, 490));

            Assert.True(drag.ContextMenu.IsOpen);
        }

        [AvaloniaFact]
        public void TheMenu_WithACompanion_StaysOpenWhileThePointerIsNearTheCompanion()
        {
            var drag = NewDragValueAtTheTop(200);
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

            PointerInput.Move(window, new Point(790, 490));
            Assert.True(drag.ContextMenu.IsOpen);

            ContextMenuPointerAway.SetCompanion(drag.ContextMenu, null);
            PointerInput.Move(window, new Point(789, 489));
            Assert.False(drag.ContextMenu.IsOpen);
        }
    }
}
