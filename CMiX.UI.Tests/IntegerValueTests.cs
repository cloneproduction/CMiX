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
    // IntegerValue is a DragValue that starts as a whole number editor. It takes the same bindings.
    public class IntegerValueTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 300, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void IntegerValue_StartsAsWholeNumbers_WithStepsOfOne()
        {
            var box = new IntegerValue();

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
        public void IntegerValue_FollowsItsModelBothWays()
        {
            var model = new NumberValue(4);
            var box = new IntegerValue { DataContext = model };
            box.Bind(DragValue.ValueProperty, new Binding("Value", BindingMode.TwoWay));
            Host(box);

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
            var box = new IntegerValue
            {
                ResetCommand = new RelayCommand(() => ran += "reset"),
                ResetAllCommand = new RelayCommand(() => ran += "all")
            };
            Host(box);

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
