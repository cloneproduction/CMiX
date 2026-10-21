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
            var wanted = new Point(-Padding.Left, -Padding.Top);
            var done = moved.GetOrCreateValue(popup);

            popup.HorizontalOffset += wanted.X - done.Offset.X;
            popup.VerticalOffset += wanted.Y - done.Offset.Y;
            done.Offset = wanted;
        }

        private sealed class Moved
        {
            public Point Offset { get; set; }
        }
    }
}
