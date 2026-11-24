// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // TogglesRow reads the row sizes by key. The tree gives it the ListTree sizes under those keys.
    public sealed record ListTreeToggleSizes(double Size, double Gap, double ColumnWidth)
    {
        public static ListTreeToggleSizes Of(Control control) =>
            new(Read(control, "ListTreeToggleSize"), Read(control, "ListTreeToggleGap"), Read(control, "ListTreeToggleColumnWidth"));

        // The gap comes once more before the first slot, between the name and the toggles.
        public void Write(IResourceDictionary resources)
        {
            resources[TreeRowResources.RowHeightKey] = Size;
            resources[TreeRowResources.RowSpacingKey] = Gap;
            resources[TreeRowResources.ToggleWidthKey] = Gap + ColumnWidth;
        }

        private static double Read(Control control, string key) =>
            control.TryFindResource(key, out var value) && value is double size ? size : 0;
    }
}
