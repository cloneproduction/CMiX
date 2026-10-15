// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.UI.Controls.Parts
{
    // A control that shows a caption: the short name that the user reads next to it.
    // ControlTitle uses it to name the menu of the control.
    public interface ICaptioned
    {
        string Caption { get; }
    }
}
