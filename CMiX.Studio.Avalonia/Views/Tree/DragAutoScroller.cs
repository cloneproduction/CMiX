// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Views
{
    // Scrolls a view during a drag. A timer scrolls while the pointer is near the top or bottom edge.
    // The wheel of a row drag comes as a pointer event (see TreeRowDrag). A wheel step is three rows.
    // This class is the only owner of the timer.
    public sealed class DragAutoScroller : IDisposable
    {
        // The tokens DragScrollEdge and DragScrollStep. A test pins them.
        internal const double EdgeZone = 28.0;
        internal const double MaxScrollStep = 18.0;

        private readonly TreeViewContext context;
        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(30) };
        private double? dragY;
        private bool disposed;

        public DragAutoScroller(TreeViewContext context)
        {
            this.context = context;
            timer.Tick += (_, _) => AutoScrollStep();
        }

        // True while the timer runs.
        public bool IsRunning => timer.IsEnabled;

        // The pointer of a drag, in the coordinates of the scroll viewer. Null when the pointer is not known.
        // The timer starts if it does not run.
        public void Track(double? y)
        {
            if (disposed)
                return;

            dragY = y;
            if (!timer.IsEnabled)
                timer.Start();
        }

        // Stops the timer at once. The next Track starts it again.
        public void StopNow()
        {
            timer.Stop();
            dragY = null;
        }

        public void ScrollByWheel(int steps)
        {
            if (dragY == null || context.Scroller is not { } view)
                return;

            var maxOffset = Math.Max(0, view.Extent.Height - view.Viewport.Height);
            var rows = steps * 3 * context.RowPitch;
            view.Offset = view.Offset.WithY(Math.Clamp(view.Offset.Y - rows, 0, maxOffset));
        }

        // One step of the scroll of a drag. The step is larger when the pointer is closer to the edge, or beyond it.
        // Returns the distance that the view moved.
        public double AutoScrollStep()
        {
            if (dragY is not { } y || context.Scroller is not { } view)
                return 0;

            var step = 0.0;
            if (y < EdgeZone)
                step = -MaxScrollStep * Math.Min(1, (EdgeZone - y) / EdgeZone);
            else if (y > view.Bounds.Height - EdgeZone)
                step = MaxScrollStep * Math.Min(1, (y - (view.Bounds.Height - EdgeZone)) / EdgeZone);

            if (step == 0)
                return 0;

            var before = view.Offset.Y;
            var maxOffset = Math.Max(0, view.Extent.Height - view.Viewport.Height);
            view.Offset = view.Offset.WithY(Math.Clamp(before + step, 0, maxOffset));
            return view.Offset.Y - before;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            StopNow();
            disposed = true;
        }
    }
}
