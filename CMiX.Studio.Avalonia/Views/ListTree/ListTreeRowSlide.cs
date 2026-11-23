// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media.Transformation;
using CMiX.UI.Themes;

namespace CMiX.Studio.Avalonia.Views
{
    // Slides a row of the new tree with a render offset: the row has its new top for the layout, and only the render moves, so a frame costs no layout.
    internal static class ListTreeRowSlide
    {
        // The offset of the render from the top of the row, also while it slides.
        public static double OffsetOf(Visual row) => row.RenderTransform?.Value.M32 ?? 0;

        // The top where the row shows now.
        public static double ShownTopOf(Control row) => PanelListTreeRows.GetTop(row) + OffsetOf(row);

        // The row shows at shownTop at once, then slides to its top in the given time.
        public static void From(Control row, double shownTop, TimeSpan duration)
        {
            var offset = shownTop - PanelListTreeRows.GetTop(row);
            row.Transitions = null;
            if (duration <= TimeSpan.Zero || offset == 0)
            {
                row.RenderTransform = null;
                return;
            }

            row.RenderTransform = TranslateBy(offset);
            row.Transitions = new Transitions
            {
                new TransformOperationsTransition { Property = Visual.RenderTransformProperty, Duration = duration, Easing = Motion.Easing }
            };
            row.RenderTransform = TransformOperations.Identity;
        }

        public static void Snap(Control row)
        {
            row.Transitions = null;
            row.RenderTransform = null;
        }

        private static TransformOperations TranslateBy(double y)
        {
            var builder = TransformOperations.CreateBuilder(1);
            builder.AppendTranslate(0, y);
            return builder.Build();
        }
    }
}
