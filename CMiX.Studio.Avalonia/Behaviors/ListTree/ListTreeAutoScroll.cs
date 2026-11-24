// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Scrolls the new tree while a drag holds the pointer near its top or bottom edge.
    internal sealed class ListTreeAutoScroll : ListTreeBehavior
    {
        // The zone at the top and at the bottom edge, and the biggest step of one tick, in pixels.
        internal const double EdgeZone = 28.0;
        internal const double MaxScrollStep = 18.0;

        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(30) };

        private double? pointerY;

        public bool IsRunning => timer.IsEnabled;

        // The steps of one turn of the wheel: at least one in the direction of the turn.
        public static int WheelSteps(PointerWheelEventArgs e)
        {
            var steps = (int)Math.Round(e.Delta.Y);
            return steps != 0 ? steps : Math.Sign(e.Delta.Y);
        }

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

        // Three rows for each step of the wheel.
        public void ScrollByWheel(int steps)
        {
            Context.ScrollTo(Scroller.Offset.Y - steps * 3 * Context.Rows.Sizes.Pitch);
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
            Context.ScrollTo(before + step);
            return Scroller.Offset.Y - before;
        }

        // Faster as the pointer goes deeper into the zone.
        private static double StepAt(double y, double height)
        {
            const double edge = EdgeZone;
            const double maxStep = MaxScrollStep;
            if (y < edge)
                return -maxStep * Math.Min(1, (edge - y) / edge);

            return y > height - edge ? maxStep * Math.Min(1, (y - (height - edge)) / edge) : 0;
        }
    }
}
