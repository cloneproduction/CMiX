// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace CMiX.Studio.Avalonia.Views
{
    // Controls assigned to plain styled properties, such as the SelectionPanel and
    // EditingPanel content declared in consuming views, are not part of any logical
    // tree until a ContentControl presents them. Avalonia never applies the deferred
    // XAML bindings of such foreign subtrees when they first attach lazily, for
    // example inside an unselected tab, which left the panel buttons without their
    // commands. Making the panel a logical child of its host at assignment time
    // roots the subtree at load, matching how regular content behaves, so the
    // bindings apply normally.
    public static class LogicalPanelContent
    {
        public static void Track<TOwner>(AvaloniaProperty property) where TOwner : Control
        {
            property.Changed.AddClassHandler<TOwner>((owner, e) =>
            {
                if (e.OldValue is ILogical oldChild && oldChild.LogicalParent == owner)
                    ((ISetLogicalParent)oldChild).SetParent(null);

                if (e.NewValue is ILogical newChild && newChild.LogicalParent == null)
                    ((ISetLogicalParent)newChild).SetParent(owner);
            });
        }
    }
}
