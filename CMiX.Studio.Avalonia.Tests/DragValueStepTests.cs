using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Views.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The drag steps have one source. Shift must stay slower than the normal drag.
    public class DragValueStepTests
    {
        [AvaloniaFact]
        public void DragValue_ShiftStepIsSmallerThanTheNormalStep()
        {
            var drag = new DragValue();

            Assert.Equal(DragValue.DefaultLargeChange, drag.LargeChange);
            Assert.Equal(DragValue.DefaultSmallChange, drag.SmallChange);
            Assert.True(drag.SmallChange < drag.LargeChange);
        }

        [AvaloniaFact]
        public void RangeValue_UsesTheDragValueSteps()
        {
            var range = new RangeValue();

            Assert.Equal(DragValue.DefaultLargeChange, range.LargeChange);
            Assert.Equal(DragValue.DefaultSmallChange, range.SmallChange);
        }
    }
}
