// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Networking
{
    // Gives a new sync for one value. For the controls that build their values by hand.
    public delegate IValueSync ValueSyncFactory();
}
