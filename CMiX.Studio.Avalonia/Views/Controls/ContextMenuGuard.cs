// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    internal static class ContextMenuGuard
    {
        // A popup below a control sends its right-clicks up to that control. The menu of the control must not open for them.
        public static bool FromPopup(Control owner, ContextRequestedEventArgs e) =>
            e.Source is Visual source && source != owner && !owner.IsVisualAncestorOf(source);
    }
}
