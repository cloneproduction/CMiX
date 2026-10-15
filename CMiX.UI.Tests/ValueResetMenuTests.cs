using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // A value editor shows a Reset menu only when it can reset. The menu must always work.
    public class ValueResetMenuTests
    {
        private static bool HasResetItem(Control c) =>
            c.ContextMenu?.Items.OfType<MenuItem>().Any(i => Equals(i.Header, "Reset")) == true;

        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 600, Height = 900 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void ColorPicker_HasNoResetMenu()
        {
            var window = Host(new ColorPickerFull());

            var withReset = window.GetVisualDescendants().OfType<Control>().Where(HasResetItem).ToList();

            Assert.Empty(withReset);
        }

        [AvaloniaFact]
        public void Slider_WithoutResetCommand_HasNoMenu()
        {
            var slider = new SliderCMiX();
            Host(slider);

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void Slider_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var slider = new SliderCMiX { ResetCommand = new RelayCommand(() => ran = true) };
            Host(slider);

            var reset = slider.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Slider_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var slider = new SliderCMiX { ResetCommand = new RelayCommand(() => { }) };
            Host(slider);
            Assert.NotNull(slider.ContextMenu);

            slider.ResetCommand = null!;

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithoutResetCommand_HasNoMenu()
        {
            var drag = new DragValue();
            Host(drag);

            Assert.Null(drag.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var drag = new DragValue { ResetCommand = new RelayCommand(() => ran = true) };
            Host(drag);

            var reset = drag.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void DragValue_KeepsAMenuSetInXaml()
        {
            var own = new ContextMenu();
            var drag = new DragValue { ContextMenu = own, ResetCommand = new RelayCommand(() => { }) };
            Host(drag);

            Assert.Same(own, drag.ContextMenu);
        }
    }
}
