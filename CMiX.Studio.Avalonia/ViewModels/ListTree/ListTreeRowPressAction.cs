// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // What a left press on a row does. AddOrRemove adds the row to the selection or takes it out. Stop ends the press with no action, PassOn lets the row control have it.
    public enum ListTreeRowPressAction
    {
        Select,
        AddOrRemove,
        SelectRange,
        PassOn,
        Stop
    }
}
