// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Views
{
    // The ZIndex of each layer of the tree, from the bottom. The background is the Background of the panel.
    public static class ListTreeLayers
    {
        // A card area is one step higher for each level of depth, so an inner area draws over its outer area.
        public const int CardArea = 1;
        public const int Row = 1000;
        public const int DropOutline = 1003;
        public const int DraggedRow = 1004;
    }
}
