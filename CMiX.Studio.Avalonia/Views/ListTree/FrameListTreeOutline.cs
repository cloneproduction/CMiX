// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The outline of the drop target of the new tree, in the kind colour, a little outside the card or the row of the target.
    public sealed class FrameListTreeOutline : Border
    {
        public const string ThicknessKey = "ListTreeDropOutlineThickness";
        public const string OffsetKey = "ListTreeDropOutlineOffset";
        public const string CornerKey = "CornerRadiusAll";

        private readonly ListTreeKindBrushConverter kindBrush = new();

        private double offset;

        public ListTreePlace? Place { get; private set; }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            BorderThickness = new Thickness(Read(ThicknessKey));
            offset = Read(OffsetKey);
            CornerRadius = this.TryFindResource(CornerKey, out var corner) && corner is CornerRadius radius ? radius : default;
        }

        public void Show(TreeNode? target, ListTreePlace? place)
        {
            Place = target != null ? place : null;
            IsVisible = Place != null;
            BorderBrush = target != null ? kindBrush.Convert(target, typeof(IBrush), ListTreeKindBrushConverter.DropOutline, CultureInfo.InvariantCulture) as IBrush : null;
        }

        // A card is framed with its rows. The frame is full width, as in the mockup.
        public Rect RectIn(double width)
        {
            if (Place is not { } place)
                return default;

            var height = place.CanToggle ? place.AreaHeight : place.RowHeight;
            return new Rect(-offset, place.Y - offset, width + 2 * offset, height + 2 * offset);
        }

        private double Read(string key) => this.TryFindResource(key, out var value) && value is double size ? size : 0;
    }
}
