// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;

namespace CMiX.Studio.Avalonia.Behaviors
{
    public class ScrollViewerBehavior : Behavior<ScrollViewer>
    {
        private double _savedOffset = 0;
        private bool _isRestoring = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
                return;

            AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject == null)
                return;

            AssociatedObject.ScrollChanged -= AssociatedObject_ScrollChanged;
        }

        private void AssociatedObject_ScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (AssociatedObject == null)
                return;

            if (e.ExtentDelta.Y > 0 && e.OffsetDelta.Y != 0)
            {
                _isRestoring = true;
                AssociatedObject.Offset = new Vector(AssociatedObject.Offset.X, _savedOffset);
            }
            else if (e.ExtentDelta.Y < 0)
            {
                _isRestoring = false;
                if (AssociatedObject.Offset.Y > 0)
                    _savedOffset = AssociatedObject.Offset.Y;
            }
            else
            {
                if (_isRestoring)
                    _isRestoring = false;
                else if (AssociatedObject.Offset.Y > 0)
                    _savedOffset = AssociatedObject.Offset.Y;
            }
        }
    }
}
