using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.BaseControls;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Reset menu opens on a context request, but never in edit mode or for a request from a popup below the editor.
    // The headless mouse raises a context request only in edit mode. So the normal case and the popup case raise it by hand.
    public class ContextMenuTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        // Presses and releases the right button on the center of the control. Returns how many context requests it raised.
        private static int RightClick(Window window, Control control)
        {
            var requests = 0;
            control.AddHandler(Control.ContextRequestedEvent, (_, _) => requests++, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);

            var center = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(center, MouseButton.Right);
            window.MouseUp(center, MouseButton.Right);
            Dispatcher.UIThread.RunJobs();
            return requests;
        }

        private static void RequestMenu(Control source)
        {
            source.RaiseEvent(new ContextRequestedEventArgs { Source = source });
            Dispatcher.UIThread.RunJobs();
        }

        private static CMiXSlider NewSlider() =>
            new() { Width = 200, Height = 30, ResetCommand = new RelayCommand(() => { }) };

        private static DragValue NewDragValue() =>
            new() { Width = 200, Height = 30, ResetCommand = new RelayCommand(() => { }) };

        [AvaloniaFact]
        public void Slider_ContextRequest_OpensTheMenu()
        {
            var slider = NewSlider();
            Host(slider);

            RequestMenu(slider);

            Assert.True(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void Slider_RightClickInEditMode_EndsEditing_AndTheMenuStaysClosed()
        {
            var slider = NewSlider();
            var window = Host(slider);
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
            Host(slider);
            slider.IsEditing = true;

            RequestMenu(slider);

            Assert.False(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void Slider_ContextRequestAfterTheEditModeClick_OpensTheMenu()
        {
            var slider = NewSlider();
            var window = Host(slider);
            slider.IsEditing = true;
            RightClick(window, slider);

            RequestMenu(slider);

            Assert.True(slider.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void DragValue_ContextRequest_OpensTheMenu()
        {
            var drag = NewDragValue();
            Host(drag);

            RequestMenu(drag);

            Assert.True(drag.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void DragValue_ContextRequestWhileEditing_OpensNoMenu()
        {
            var drag = NewDragValue();
            Host(drag);
            drag.IsEditing = true;

            RequestMenu(drag);

            Assert.False(drag.ContextMenu!.IsOpen);
        }

        private static (ColorSelector Selector, Popup Popup) ShowColorSelectorWithOpenPopup()
        {
            var color = TestServiceProviderFactory.Create().GetRequiredService<GenericValue<string>>();
            var selector = new ColorSelector { DataContext = color, Width = 200, Height = 40 };
            Host(selector);

            selector.FindControl<ToggleButton>("PopupToggle")!.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            var popup = selector.FindControl<Popup>("colorPickerPopup")!;
            Assert.True(popup.IsOpen);
            return (selector, popup);
        }

        [AvaloniaFact]
        public void ColorSelector_ContextRequestOnTheSwatch_OpensTheResetMenu()
        {
            var (selector, _) = ShowColorSelectorWithOpenPopup();

            RequestMenu(selector.FindControl<ToggleButton>("PopupToggle")!);

            Assert.True(selector.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ColorSelector_ContextRequestInsideThePopup_OpensNoMenu()
        {
            var (selector, popup) = ShowColorSelectorWithOpenPopup();
            var insidePopup = popup.Child!.GetVisualDescendants().OfType<CMiXSlider>().First();

            RequestMenu(insidePopup);

            Assert.False(selector.ContextMenu!.IsOpen);
        }
    }
}
