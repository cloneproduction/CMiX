// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public static class ListTreeRowPressRule
    {
        // The tree has one selection, so a Ctrl press on a row does nothing. The icon of a card opens or closes the card on its tap.
        public static ListTreeRowPressAction ActionOf(bool isControlDown, bool isOnCardIcon) =>
            isControlDown ? ListTreeRowPressAction.Stop
            : isOnCardIcon ? ListTreeRowPressAction.PassOn
            : ListTreeRowPressAction.Select;
    }
}
