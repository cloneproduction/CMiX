namespace TreeDrag.Core;

/// <summary>Pure functions for the pointer part of a drag.</summary>
public static class DragMath
{
    /// <summary>Tells if the pointer moved farther than the threshold from the press point.</summary>
    public static bool PassedThreshold(double pressX, double pressY, double x, double y, double threshold) =>
        (x - pressX) * (x - pressX) + (y - pressY) * (y - pressY) > threshold * threshold;
}
