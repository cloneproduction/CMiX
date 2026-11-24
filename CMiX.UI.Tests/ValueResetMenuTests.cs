using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // A value editor shows a Reset menu only when it can reset. The menu must always work.
    public class ValueResetMenuTests
    {
        private static bool HasResetItem(Control c) =>
            c.ContextMenu?.Items.OfType<MenuItem>().Any(i => Equals(i.Header, "Reset")) == true;

        [AvaloniaFact]
        public void ColorPicker_HasNoResetMenu()
        {
            var window = TestWindow.Show(new ColorPickerFull(), 600, 900);

            var withReset = window.GetVisualDescendants().OfType<Control>().Where(HasResetItem).ToList();

            Assert.Empty(withReset);
        }

        [AvaloniaFact]
        public void Slider_WithoutResetCommand_HasNoMenu()
        {
            var slider = new SliderCMiX();
            TestWindow.Show(slider, 600, 900);

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void Slider_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var slider = new SliderCMiX { ResetCommand = new RelayCommand(() => ran = true) };
            TestWindow.Show(slider, 600, 900);

            var reset = slider.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Slider_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var slider = new SliderCMiX { ResetCommand = new RelayCommand(() => { }) };
            TestWindow.Show(slider, 600, 900);
            Assert.NotNull(slider.ContextMenu);

            slider.ResetCommand = null!;

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithoutResetCommand_HasNoMenu()
        {
            var drag = new DragValue();
            TestWindow.Show(drag, 600, 900);

            Assert.Null(drag.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var drag = new DragValue { ResetCommand = new RelayCommand(() => ran = true) };
            TestWindow.Show(drag, 600, 900);

            var reset = drag.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void DragValue_KeepsAMenuSetInXaml()
        {
            var own = new ContextMenu();
            var drag = new DragValue { ContextMenu = own, ResetCommand = new RelayCommand(() => { }) };
            TestWindow.Show(drag, 600, 900);

            Assert.Same(own, drag.ContextMenu);
        }
    }
}
