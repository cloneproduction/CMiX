using TreeDrag.Core;

namespace TreeDrag.Core.Tests;

public class DragMathTests
{
    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(3, 0, false)]
    [InlineData(4, 0, false)]
    [InlineData(3, 3, true)]
    [InlineData(0, -5, true)]
    public void PassedThreshold_Moves_TrueOnlyFartherThanTheThreshold(double dx, double dy, bool expected)
    {
        Assert.Equal(expected, DragMath.PassedThreshold(10, 10, 10 + dx, 10 + dy, 4));
    }
}
