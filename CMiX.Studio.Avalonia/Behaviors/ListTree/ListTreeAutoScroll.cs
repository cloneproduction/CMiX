// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Scrolls the new tree while a drag holds the pointer near its top or bottom edge, with the zone and the step of DragAutoScroller.
    internal sealed class ListTreeAutoScroll : ListTreeBehavior
    {
        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(30) };

        private double? pointerY;

        public bool IsRunning => timer.IsEnabled;

        private ScrollViewer Scroller => Context.Scroller;

        protected override void OnWatch(ProjectTree tree) => timer.Tick += OnTick;

        protected override void OnUnwatch()
        {
            Stop();
            timer.Tick -= OnTick;
        }

        // The y is in the scroller.
        public void Track(double y)
        {
            pointerY = y;
            if (!timer.IsEnabled)
                timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            pointerY = null;
        }

        private void OnTick(object? sender, EventArgs e) => ScrollStep();

        // The distance that the step scrolled.
        public double ScrollStep()
        {
            if (pointerY is not { } y)
                return 0;

            var step = StepAt(y, Scroller.Bounds.Height);
            if (step == 0)
                return 0;

            var before = Scroller.Offset.Y;
            var maxOffset = Math.Max(0, Scroller.Extent.Height - Scroller.Viewport.Height);
            Scroller.Offset = Scroller.Offset.WithY(Math.Clamp(before + step, 0, maxOffset));
            return Scroller.Offset.Y - before;
        }

        // Faster as the pointer goes deeper into the zone.
        private static double StepAt(double y, double height)
        {
            const double edge = DragAutoScroller.EdgeZone;
            const double maxStep = DragAutoScroller.MaxScrollStep;
            if (y < edge)
                return -maxStep * Math.Min(1, (edge - y) / edge);

            return y > height - edge ? maxStep * Math.Min(1, (y - (height - edge)) / edge) : 0;
        }
    }
}
