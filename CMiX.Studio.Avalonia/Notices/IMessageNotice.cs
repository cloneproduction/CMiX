// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;

namespace CMiX.Studio.Avalonia.Notices
{
    // Tells the user that an operation failed. The title names the operation, for example "Open Project".
    public interface IMessageNotice
    {
        Task ShowErrorAsync(string text, string title);
    }
}
