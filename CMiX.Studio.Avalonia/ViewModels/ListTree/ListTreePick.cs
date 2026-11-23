// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The pointer math of a drag: the start threshold and the candidate nearest the dragged row.
    public sealed class ListTreePick
    {
        private readonly ListTreeDragNumbers numbers;

        public ListTreePick(ListTreeDragNumbers numbers)
        {
            this.numbers = numbers;
        }

        public bool PassedThreshold(ListTreePress press, ListTreePoint pointer)
        {
            var dx = pointer.X - press.Pointer.X;
            var dy = pointer.Y - press.Pointer.Y;
            return dx * dx + dy * dy > numbers.Threshold * numbers.Threshold;
        }

        public ListTreeCandidate Nearest(IReadOnlyList<ListTreeCandidate> candidates, ListTreePress press, ListTreePoint pointer) =>
            candidates.MinBy(candidate => DistanceOf(candidate, press, pointer))
            ?? throw new ArgumentException("No candidates.", nameof(candidates));

        // A hand that shakes does not change the pick.
        public ListTreeCandidate NearestOrCurrent(IReadOnlyList<ListTreeCandidate> candidates, ListTreeCandidate current, ListTreePress press, ListTreePoint pointer)
        {
            var nearest = Nearest(candidates, press, pointer);
            if (!candidates.Any(candidate => ReferenceEquals(candidate, current)))
                return nearest;

            return DistanceOf(nearest, press, pointer) > DistanceOf(current, press, pointer) - numbers.Hysteresis ? current : nearest;
        }

        // Every row has the full width, so the pointer x gives the depth.
        public double DistanceOf(ListTreeCandidate candidate, ListTreePress press, ListTreePoint pointer)
        {
            var dx = pointer.X - press.Pointer.X;
            var dy = pointer.Y - press.GrabOffsetY - candidate.Spot.Y;
            var levelX = (candidate.Spot.Depth - press.Depth) * numbers.PixelsPerLevel;
            return Math.Sqrt(Square((levelX - dx) * numbers.HorizontalWeight) + Square(dy));
        }

        private static double Square(double value) => value * value;
    }
}
