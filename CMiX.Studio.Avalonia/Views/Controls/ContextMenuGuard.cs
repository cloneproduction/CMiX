// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
