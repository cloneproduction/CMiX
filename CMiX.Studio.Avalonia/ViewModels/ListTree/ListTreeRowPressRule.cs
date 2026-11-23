// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public static class ListTreeRowPressRule
    {
        // The keys that are down when a row is pressed.
        public sealed record Keys(bool IsControlDown, bool IsShiftDown);

        // The part of a row that a press is on.
        public enum Part
        {
            Row,
            CardIcon
        }

        // What a left press on a row does. AddOrRemove adds the row to the selection or takes it out. Stop ends the press with no action, PassOn lets the row control have it.
        public enum Action
        {
            Select,
            AddOrRemove,
            SelectRange,
            PassOn,
            Stop
        }

        // The icon of a card opens or closes the card on its tap and never changes the selection. Ctrl wins over Shift.
        public static Action ActionOf(Keys keys, Part part) =>
            part == Part.CardIcon ? (keys.IsControlDown ? Action.Stop : Action.PassOn)
            : keys.IsControlDown ? Action.AddOrRemove
            : keys.IsShiftDown ? Action.SelectRange
            : Action.Select;
    }
}
