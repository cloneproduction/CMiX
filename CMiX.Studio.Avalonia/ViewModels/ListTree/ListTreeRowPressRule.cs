// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public static class ListTreeRowPressRule
    {
        // The icon of a card opens or closes the card on its tap and never changes the selection. Ctrl wins over Shift.
        public static ListTreeRowPressAction ActionOf(ListTreeRowPressKeys keys, ListTreeRowPressPart part) =>
            part == ListTreeRowPressPart.CardIcon ? (keys.IsControlDown ? ListTreeRowPressAction.Stop : ListTreeRowPressAction.PassOn)
            : keys.IsControlDown ? ListTreeRowPressAction.AddOrRemove
            : keys.IsShiftDown ? ListTreeRowPressAction.SelectRange
            : ListTreeRowPressAction.Select;
    }
}
