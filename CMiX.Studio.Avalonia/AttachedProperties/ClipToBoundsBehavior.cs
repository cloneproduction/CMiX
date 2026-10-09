// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.AttachedProperties
{
    public static class ClipToBoundsBehavior
    {
        public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.RegisterAttached<Border, CornerRadius>(
                "CornerRadius", typeof(ClipToBoundsBehavior));

        public static CornerRadius GetCornerRadius(Border border) => border.GetValue(CornerRadiusProperty);
        public static void SetCornerRadius(Border border, CornerRadius value) => border.SetValue(CornerRadiusProperty, value);

        private static readonly AttachedProperty<bool> IsSubscribedProperty =
            AvaloniaProperty.RegisterAttached<Border, bool>("IsSubscribed", typeof(ClipToBoundsBehavior));

        static ClipToBoundsBehavior()
        {
            CornerRadiusProperty.Changed.AddClassHandler<Border>((border, e) =>
            {
                if (!border.GetValue(IsSubscribedProperty))
                {
                    border.SetValue(IsSubscribedProperty, true);
                    border.PropertyChanged += (s, args) =>
                    {
                        if (args.Property == Visual.BoundsProperty)
                            UpdateClip(border);
                    };
                }
                UpdateClip(border);
            });
        }

        private static void UpdateClip(Border border)
        {
            var radius = GetCornerRadius(border);
            var rect = new Rect(border.Bounds.Size);
            border.Clip = BuildRoundedRectGeometry(rect, radius);
        }

        private static StreamGeometry BuildRoundedRectGeometry(Rect rect, CornerRadius radius)
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                double tl = radius.TopLeft;
                double tr = radius.TopRight;
                double br = radius.BottomRight;
                double bl = radius.BottomLeft;

                ctx.BeginFigure(new Point(rect.Left + tl, rect.Top), true);

                ctx.LineTo(new Point(rect.Right - tr, rect.Top));
                if (tr > 0)
                    ctx.ArcTo(new Point(rect.Right, rect.Top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise);

                ctx.LineTo(new Point(rect.Right, rect.Bottom - br));
                if (br > 0)
                    ctx.ArcTo(new Point(rect.Right - br, rect.Bottom), new Size(br, br), 0, false, SweepDirection.Clockwise);

                ctx.LineTo(new Point(rect.Left + bl, rect.Bottom));
                if (bl > 0)
                    ctx.ArcTo(new Point(rect.Left, rect.Bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise);

                ctx.LineTo(new Point(rect.Left, rect.Top + tl));
                if (tl > 0)
                    ctx.ArcTo(new Point(rect.Left + tl, rect.Top), new Size(tl, tl), 0, false, SweepDirection.Clockwise);

                ctx.EndFigure(true);
            }
            return geometry;
        }
    }
}
