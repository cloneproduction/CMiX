// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The line under the tree during a drag: where the row goes and what the grey rows mean.
    public static class ListTreeDragNote
    {
        public static string Of(ListTreeDragLook look) => $"Move into {NameOf(look)}. Grey rows can not take it.";

        private static string NameOf(ListTreeDragLook look) => look.HolderNode?.Header ?? "the project";
    }
}
