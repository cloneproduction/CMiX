// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The line under the tree during a drag: where the row goes and what the grey rows mean.
    public static class ListTreeDragNote
    {
        public static string Of(ListTreeDragLook look) => $"Move into {NameOf(look)}. Grey rows can not take it.";

        // The key is the gesture that opens the card at once.
        public static string OfNearCard(ListTreeDragLook look, TreeNode card, string key) => $"{Of(look)} {key} opens {card.Header} now.";

        private static string NameOf(ListTreeDragLook look) => look.HolderNode?.Header ?? "the project";
    }
}
