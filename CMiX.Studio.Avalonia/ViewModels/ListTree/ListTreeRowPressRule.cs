// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public static class ListTreeRowPressRule
    {
        // The icon of a card opens or closes the card on its tap and never changes the selection. Ctrl wins over Shift.
        public static ListTreeRowPressAction ActionOf(bool isControlDown, bool isShiftDown, bool isOnCardIcon) =>
            isOnCardIcon ? (isControlDown ? ListTreeRowPressAction.Stop : ListTreeRowPressAction.PassOn)
            : isControlDown ? ListTreeRowPressAction.AddOrRemove
            : isShiftDown ? ListTreeRowPressAction.SelectRange
            : ListTreeRowPressAction.Select;
    }
}
