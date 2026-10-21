// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace CMiX.UI.Controls.Parts
{
    // Keeps a drag going when the real cursor reaches the edge of the screen, like the continuous grab of Blender.
    // The cursor is hidden during the drag. At the edge it is placed near the opposite edge, and the jump is
    // taken out of the positions that this class returns, so the control sees one continuous movement.
    // One instance for each drag.
    internal sealed class DragCursorWrap
    {
        // The cursor is moved when it is this close to an edge of its screen, in pixels.
        private const int TriggerDistance = 1;

        // And it is placed this far from the opposite edge, so it does not trigger again at once.
        private const int WrapInset = 16;

        // After the cursor was placed, the moves that were already queued still come from the old side. They are
        // ignored until one comes from the new side. A fast mouse can be far from the new place at the first move,
        // so a move counts as old when it is nearer to the old place than to the new one. This is the number of
        // old moves to wait for before giving up.
        private const int MovesToWait = 8;

        private readonly Visual visual;
        private global::Avalonia.Vector shift;
        private global::Avalonia.Vector pendingJump;
        private PixelPoint? arrival;
        private PixelPoint departure;
        private int waited;
        private Point last;

        public DragCursorWrap(Visual visual, Point start)
        {
            this.visual = visual;
            last = start;
        }

        // The position of the pointer in the units of the visual, as if the cursor had never been moved by a wrap.
        public Point Position(PointerEventArgs e)
        {
            var raw = e.GetPosition(visual);
            var screen = visual.PointToScreen(raw);

            if (arrival is { } placedAt)
            {
                if (DistanceSquared(screen, placedAt) < DistanceSquared(screen, departure))
                {
                    shift += pendingJump;
                    arrival = null;
                }
                else if (++waited > MovesToWait)
                {
                    // The cursor did not go where it was placed. Nothing changes and the drag goes on.
                    arrival = null;
                    pendingJump = default;
                }
                else
                {
                    return last;
                }
            }

            last = raw - shift;

            if (arrival == null && TryFindWrap(screen, out var destination))
            {
                var scale = TopLevel.GetTopLevel(visual)?.RenderScaling ?? 1;
                pendingJump = new global::Avalonia.Vector((destination.X - screen.X) / scale, (destination.Y - screen.Y) / scale);
                arrival = destination;
                departure = screen;
                waited = 0;
                DragEditHelper.PlaceCursorAt(new Point(destination.X, destination.Y));
            }

            return last;
        }

        private static double DistanceSquared(PixelPoint a, PixelPoint b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        private bool TryFindWrap(PixelPoint screen, out PixelPoint destination)
        {
            destination = screen;

            if (DragEditHelper.ScreenBoundsAt(visual, screen) is not { } bounds)
                return false;

            var x = screen.X;
            var y = screen.Y;

            if (x >= bounds.Right - 1 - TriggerDistance)
                x = bounds.X + WrapInset;
            else if (x <= bounds.X + TriggerDistance)
                x = bounds.Right - 1 - WrapInset;

            if (y >= bounds.Bottom - 1 - TriggerDistance)
                y = bounds.Y + WrapInset;
            else if (y <= bounds.Y + TriggerDistance)
                y = bounds.Bottom - 1 - WrapInset;

            destination = new PixelPoint(x, y);
            return destination != screen;
        }
    }
}
