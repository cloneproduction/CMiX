// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CMiX.UI.Controls.Parts
{
    // Blocks the context requests that must not open the menu of a control:
    // the ones that come from a popup below the control, the ones that come from its leading or trailing content,
    // and the ones of a right-click that ends the edit mode.
    internal sealed class ContextMenuGuard
    {
        private readonly Control owner;
        private readonly Func<bool> isEditing;
        private readonly Func<IEnumerable<object?>> sideContent;
        private bool suppress;

        private ContextMenuGuard(Control owner, Func<bool> isEditing, Func<IEnumerable<object?>> sideContent)
        {
            this.owner = owner;
            this.isEditing = isEditing;
            this.sideContent = sideContent;
        }

        // Call it once in the constructor of the owner. An owner with an edit mode passes it as isEditing.
        // An owner with leading or trailing content passes it as sideContent.
        public static ContextMenuGuard Install(Control owner, Func<bool>? isEditing = null, Func<IEnumerable<object?>>? sideContent = null)
        {
            var guard = new ContextMenuGuard(owner, isEditing ?? (() => false), sideContent ?? (() => Array.Empty<object?>()));
            owner.AddHandler(Control.ContextRequestedEvent, guard.OnContextRequested, RoutingStrategies.Tunnel);
            return guard;
        }

        // A popup below a control sends its right-clicks up to that control. The menu of the control must not open for them.
        public static bool FromPopup(Control owner, ContextRequestedEventArgs e) =>
            e.Source is Visual source && source != owner && !owner.IsVisualAncestorOf(source);

        // The side content is inside the control, but a right-click on it is not for the control.
        private bool FromSideContent(ContextRequestedEventArgs e) =>
            e.Source is Visual source && sideContent().OfType<Visual>().Any(side => side == source || side.IsVisualAncestorOf(source));

        // A right-click that ends editing must not open the menu when the button is released.
        // The owner calls this when a pointer is pressed on it, before it leaves the edit mode.
        public void PointerPressed(bool rightButton) => suppress = rightButton && isEditing();

        private void OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            if (!suppress && !isEditing() && !FromPopup(owner, e) && !FromSideContent(e))
                return;

            suppress = false;
            e.Handled = true;
        }
    }
}
