// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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

            // WPF ExtentHeightChange maps to ExtentDelta.Y and VerticalChange maps to OffsetDelta.Y.
            // The current vertical offset is read from the ScrollViewer Offset vector.
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
