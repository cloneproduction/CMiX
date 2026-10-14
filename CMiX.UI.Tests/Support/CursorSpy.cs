using Avalonia;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Tests.Support
{
    // Takes the place of the cursor placement of the controls, so a test sees where the cursor would go and the
    // real cursor of the person who runs the tests does not jump.
    internal sealed class CursorSpy : IDisposable
    {
        private readonly Action<Point> previous;

        public CursorSpy()
        {
            previous = DragEditHelper.CursorPlacer;
            DragEditHelper.CursorPlacer = Placed.Add;
        }

        public List<Point> Placed { get; } = new();

        public void Dispose() => DragEditHelper.CursorPlacer = previous;
    }
}
