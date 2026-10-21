// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CMiX.UI.Controls.Parts
{
    // The frame of a dropdown panel that hangs under a toggle or a combo box: the panel color, the bottom corners
    // and the window shadow. The padding is the room for the shadow. A popup clips the shadow at its edge, so the room
    // is part of the popup. The frame moves its popup by that room, so the panel stays where it would be without the shadow.
    // A popup of a fixed width must add the room on both sides (see TogglePopup).
    public class DropdownFrame : ContentControl
    {
        private static readonly ConditionalWeakTable<Popup, Moved> moved = new();

        protected override Type StyleKeyOverride => typeof(DropdownFrame);

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (Parent is Popup popup)
                MoveByTheRoomOfTheShadow(popup);
        }

        // The popup is moved once. Later calls only follow a change of the padding.
        private void MoveByTheRoomOfTheShadow(Popup popup)
        {
            var wanted = ShiftFor(popup.Placement, Padding);
            var done = moved.GetOrCreateValue(popup);

            popup.HorizontalOffset += wanted.X - done.Offset.X;
            popup.VerticalOffset += wanted.Y - done.Offset.Y;
            done.Offset = wanted;
        }

        // The widened popup grows to both sides. A popup that is centered under its target stays centered,
        // so it needs no sideways shift. A popup that is aligned to an edge must move by the room at that edge.
        // Under the target (or over it), the room at the top (or the bottom) must be given back.
        internal static Point ShiftFor(PlacementMode placement, Thickness room) => placement switch
        {
            PlacementMode.Bottom => new Point(0, -room.Top),
            PlacementMode.BottomEdgeAlignedLeft => new Point(-room.Left, -room.Top),
            PlacementMode.BottomEdgeAlignedRight => new Point(room.Right, -room.Top),
            PlacementMode.Top => new Point(0, room.Bottom),
            PlacementMode.TopEdgeAlignedLeft => new Point(-room.Left, room.Bottom),
            PlacementMode.TopEdgeAlignedRight => new Point(room.Right, room.Bottom),
            _ => default
        };

        private sealed class Moved
        {
            public Point Offset { get; set; }
        }
    }
}
