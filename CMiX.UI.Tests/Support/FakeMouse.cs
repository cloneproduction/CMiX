using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace CMiX.UI.Tests.Support
{
    // A Windows that moves the cursor where the app places it.
    internal sealed class FakeMouse
    {
        private readonly Window window;
        private readonly Control target;
        private readonly IPointer pointer;
        private readonly CursorSpy spy;

        public FakeMouse(Window window, Control target, IPointer pointer, CursorSpy spy, PixelRect bounds, PixelPoint cursor)
        {
            this.window = window;
            this.target = target;
            this.pointer = pointer;
            this.spy = spy;
            Bounds = bounds;
            Cursor = cursor;
        }

        public PixelRect Bounds { get; }
        public IPointer Pointer => pointer;
        public PixelPoint Cursor { get; private set; }

        // How far the mouse really moved, without the jumps of the placements.
        public double MovedX { get; private set; }
        public double MovedY { get; private set; }

        // Moves that were queued before a placement, and still come from the old side.
        public int StaleMoves { get; set; }

        // False: the placement does not work, and the cursor stays where it is.
        public bool HonorsPlacement { get; set; } = true;

        // False: Windows sends no move for the placement itself. The next move comes from the new place.
        public bool ReportsPlacement { get; set; } = true;

        public int Placements => spy.Placed.Count;

        public void Move(int dx, int dy, KeyModifiers modifiers = KeyModifiers.Shift)
        {
            var old = Cursor;
            var moved = new PixelPoint(
                Math.Clamp(old.X + dx, Bounds.X, Bounds.Right - 1),
                Math.Clamp(old.Y + dy, Bounds.Y, Bounds.Bottom - 1));
            MovedX += moved.X - old.X;
            MovedY += moved.Y - old.Y;
            Cursor = moved;

            var before = spy.Placed.Count;
            Raise(moved, modifiers);

            if (spy.Placed.Count > before && HonorsPlacement)
            {
                for (var i = 0; i < StaleMoves; i++)
                    Raise(moved, modifiers);

                var to = spy.Placed[^1];
                Cursor = new PixelPoint((int)to.X, (int)to.Y);
                if (ReportsPlacement)
                    Raise(Cursor, modifiers);
            }
        }

        private void Raise(PixelPoint screen, KeyModifiers modifiers) =>
            PointerInput.Move(target, pointer, window.PointToClient(screen), window, modifiers, RawInputModifiers.LeftMouseButton);
    }
}
