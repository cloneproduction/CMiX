using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // DragValueInteger is a DragValue that starts as a whole number editor. It takes the same bindings.
    public class DragValueIntegerTests
    {
        [AvaloniaFact]
        public void DragValueInteger_StartsAsWholeNumbers_WithStepsOfOne()
        {
            var box = new DragValueInteger();

            Assert.True(box.IsInteger);
            Assert.Equal(1.0, box.SmallChange);
            Assert.Equal(1.0, box.LargeChange);
            Assert.Equal(-10000.0, box.Minimum);
            Assert.Equal(10000.0, box.Maximum);
        }

        [AvaloniaFact]
        public void DragValue_IsNotAWholeNumberEditor_ByDefault()
        {
            var drag = new DragValue();

            Assert.False(drag.IsInteger);
            Assert.Equal(DragValue.DefaultSmallChange, drag.SmallChange);
        }

        [AvaloniaFact]
        public void DragValueInteger_FollowsItsModelBothWays()
        {
            var model = new NumberValue(4);
            var box = new DragValueInteger { DataContext = model };
            box.Bind(DragValue.ValueProperty, new Binding("Value", BindingMode.TwoWay));
            TestWindow.Show(box, 300, 100);

            model.Value += 7;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(model.Value, box.Value);

            box.Value += 3;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(box.Value, model.Value);
        }

        [AvaloniaFact]
        public void TheMenu_HasResetAll_OnlyWhileTheCommandIsSet()
        {
            var ran = "";
            var box = new DragValueInteger
            {
                ResetCommand = new RelayCommand(() => ran += "reset"),
                ResetAllCommand = new RelayCommand(() => ran += "all")
            };
            TestWindow.Show(box, 300, 100);

            var headers = box.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Select(i => i.Header).ToList();
            Assert.Equal(new object?[] { "Reset", "Reset All" }, headers);

            box.ContextMenu.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Last().Command!.Execute(null);
            Assert.Equal("all", ran);

            box.ResetAllCommand = null!;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new object?[] { "Reset" }, box.ContextMenu!.Items.OfType<MenuItem>().Where(i => i.Header is string h && h.StartsWith("Reset")).Select(i => i.Header).ToList());
        }
    }
}
