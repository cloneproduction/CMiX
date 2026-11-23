// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views.Managers;
using CompositionModel = CMiX.Core.Compositing.Composition;
using LayerModel = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    internal static class ListTreeMenuKind
    {
        // A group row gets the inert menu, a layer or a composition the menu of a composable, every other item the menu of a slot.
        public static RowMenuKind Of(TreeNode node) =>
            node.Item == null ? RowMenuKind.Group
            : node.Item is CompositionModel or LayerModel ? RowMenuKind.Composable
            : RowMenuKind.Slot;
    }
}
