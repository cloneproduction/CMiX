// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;

namespace CMiX.Studio.Avalonia.Notices
{
    // Tells the user that a move in the project tree is not possible, and why.
    public interface IMoveBlockedNotice
    {
        Task ShowAsync(string message);
    }
}
