// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;

namespace CMiX.UI.AttachedProperties
{
    // Where every tooltip of the app opens: on one side of the control that has it, 8 pixels away, never over it (see PopupPlacement).
    // The card opens at the pointer along the control, and its tip points at the pointer. It stays where it opened.
    // Near the edge of the screen it stops 8 pixels from the edge and the tip keeps pointing at the pointer.
    // With no room on its side, it opens on the other side.
    // A view does not set ToolTip.Placement or the offsets. It sets ToolTip.Tip only, and Side for a control whose tooltip goes
    // to the right or to the left of it.
    // The default value of the placement cannot be changed for all controls, so a control gets it when it gets a tip.
    // A view can still set its own placement for one control, and that placement stays.
    public static class ToolTipDefaults
    {
        public const double Gap = 8;

        // The distance from the start of the card to the pointer, so the tip is never at a corner of the card.
        public const double PointerAlign = 24;

        private static bool applied;

        private static readonly ConditionalWeakTable<Control, Follower> followers = new();

        // The side of the control where its tooltip opens. Top and bottom tooltips follow the pointer in x. Left and right tooltips follow it in y.
        public static readonly AttachedProperty<PopupSide> SideProperty =
            AvaloniaProperty.RegisterAttached<Control, PopupSide>("Side", typeof(ToolTipDefaults), PopupSide.Bottom);

        public static void SetSide(Control control, PopupSide value) => control.SetValue(SideProperty, value);
        public static PopupSide GetSide(Control control) => control.GetValue(SideProperty);

        // The theme calls this when it is made. A second call does nothing.
        public static void Apply()
        {
            if (applied)
                return;

            applied = true;
            ToolTip.TipProperty.Changed.AddClassHandler<Control>((control, e) =>
            {
                if (e.NewValue != null)
                    Place(control);
            });
            SideProperty.Changed.AddClassHandler<Control>((control, _) =>
            {
                if (followers.TryGetValue(control, out var follower))
                    follower.Refresh();
            });
        }

        // The place of the tooltip of a control, or null when the control has none. A test reads it.
        internal static PopupAnchoring? AnchoringOf(Control control) => followers.TryGetValue(control, out var follower) ? follower.Anchoring : null;

        // Makes the tooltip of a control follow the rule, once. A control with its own placement keeps it.
        public static void Place(Control control)
        {
            if (followers.TryGetValue(control, out _) || control.IsSet(ToolTip.PlacementProperty))
                return;

            followers.Add(control, new Follower(control));
        }

        // The place of the tooltip of one control. It listens to the pointer on the control and updates the point that the card follows.
        private sealed class Follower
        {
            private readonly Control control;
            private readonly PopupAnchoring anchoring;

            public PopupAnchoring Anchoring => anchoring;

            public Follower(Control control)
            {
                this.control = control;
                anchoring = PopupAnchoring.Use(control);
                anchoring.Gap = Gap;
                anchoring.AlignOffset = PointerAlign;
                anchoring.Placed += result => ToolTipCard.SetResult(control, result);
                Refresh();

                control.PointerEntered += OnPointer;
                control.AddHandler(InputElement.PointerMovedEvent, OnPointer, Avalonia.Interactivity.RoutingStrategies.Bubble, handledEventsToo: true);
            }

            public void Refresh() => anchoring.Side = GetSide(control);

            private bool Vertical => anchoring.Side is PopupSide.Top or PopupSide.Bottom;

            // The point that the tooltip follows is the pointer as the tooltip opens. It follows the pointer until it opens, then it stays
            // where it opened, so it never moves under the pointer while the user reads it.
            private void OnPointer(object? sender, PointerEventArgs e)
            {
                if (ToolTip.GetIsOpen(control) || TopLevel.GetTopLevel(control) is not { } top)
                    return;

                var point = e.GetPosition(top);
                anchoring.Along = Vertical ? point.X : point.Y;
            }
        }
    }
}
