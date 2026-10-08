// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // The frame of every popup and menu: shadow, rounded translucent panel and padding.
    public class PopupFrame : ContentControl
    {
        public static readonly StyledProperty<bool> CentersOnPointerProperty =
            AvaloniaProperty.Register<PopupFrame, bool>(nameof(CentersOnPointer));
        public bool CentersOnPointer
        {
            get => GetValue(CentersOnPointerProperty);
            set => SetValue(CentersOnPointerProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(PopupFrame);

        // A menu that opens at the pointer hangs to the bottom right. This puts its middle on the pointer.
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            if (CentersOnPointer && TemplatedParent is Control { Parent: Popup { Placement: PlacementMode.Pointer } popup })
            {
                popup.HorizontalOffset = -size.Width / 2;
                popup.VerticalOffset = -size.Height / 2;
            }

            return size;
        }
    }
}
