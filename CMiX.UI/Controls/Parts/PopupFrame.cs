// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Parts
{
    // The frame of every popup and menu: shadow, rounded translucent panel and padding.
    public class PopupFrame : ContentControl
    {
        public static readonly StyledProperty<bool> CentersOnPointerProperty =
            AvaloniaProperty.Register<PopupFrame, bool>(nameof(CentersOnPointer));
        public bool CentersOnPointer
        {
            get => GetValue(CentersOnPointerProperty);
            set => SetValue(CentersOnPointerProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(PopupFrame);

        // A menu that opens at the pointer hangs to the bottom right. This puts its middle on the pointer.
        // When a row is the anchor (see MenuAnchor), the middle of that row goes on the pointer.
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            if (CentersOnPointer && TemplatedParent is Control { Parent: Popup { Placement: PlacementMode.Pointer } popup })
            {
                var offset = PointerOffset(size);
                popup.HorizontalOffset = offset.X;
                popup.VerticalOffset = offset.Y;
            }

            return size;
        }

        // The offset of the popup from the pointer. The frame is at the corner of the popup, so a point of the frame
        // has the same coordinates in the popup.
        internal Point PointerOffset(Size size)
        {
            var y = -size.Height / 2;
            var anchor = this.GetVisualDescendants().OfType<Control>().FirstOrDefault(row => row.IsVisible && MenuAnchor.GetIsAnchor(row));
            if (anchor != null && anchor.TranslatePoint(new Point(anchor.Bounds.Width / 2, anchor.Bounds.Height / 2), this) is { } middle)
                y = -middle.Y;

            return new Point(-size.Width / 2, y);
        }
    }
}
