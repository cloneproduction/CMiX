// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public sealed record TreeRowResources(
        double RowHeight,
        double RowSpacing,
        double ToggleWidth,
        double RowGap,
        double DropLineThickness,
        Thickness DropLineGapMargin,
        Thickness DropLineTopMargin,
        Thickness SelectionThickness)
    {
        public const string RowHeightKey = "TreeRowHeight";
        public const string RowSpacingKey = "TreeRowSpacing";
        public const string ToggleWidthKey = "TreeToggleWidth";
        public const string RowGapKey = "TreeRowGap";
        public const string DropLineThicknessKey = "TreeDropLineThickness";
        public const string DropLineGapMarginKey = "TreeDropLineGapMargin";
        public const string DropLineTopMarginKey = "TreeDropLineTopMargin";
        public const string SelectionThicknessKey = "TreeSelectionThickness";

        public void Write(IResourceDictionary resources)
        {
            resources[RowHeightKey] = RowHeight;
            resources[RowSpacingKey] = RowSpacing;
            resources[ToggleWidthKey] = ToggleWidth;
            resources[RowGapKey] = RowGap;
            resources[DropLineThicknessKey] = DropLineThickness;
            resources[DropLineGapMarginKey] = DropLineGapMargin;
            resources[DropLineTopMarginKey] = DropLineTopMargin;
            resources[SelectionThicknessKey] = SelectionThickness;
        }
    }
}
