// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Views
{
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

        public bool IsRunning => timer.IsEnabled;

        public void Track(double? y)
        {
            if (disposed)
                return;

            dragY = y;
            if (!timer.IsEnabled)
                timer.Start();
        }

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
