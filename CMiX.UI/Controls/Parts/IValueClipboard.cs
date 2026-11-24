// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;

namespace CMiX.UI.Controls.Parts
{
    // The clipboard of the Copy and Paste rows of ValueMenu. By default it is the clipboard of the system.
    internal interface IValueClipboard
    {
        Task<string?> GetTextAsync();

        Task SetTextAsync(string text);
    }
}
