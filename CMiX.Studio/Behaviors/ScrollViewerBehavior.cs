// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CMiX.Studio.Behaviors
{
    public class ScrollViewerBehavior : Behavior<ScrollViewer>
    {
        private double _savedOffset = 0;
        private bool _isRestoring = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ScrollChanged -= AssociatedObject_ScrollChanged;
        }

        private void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0 && e.VerticalChange != 0)
            {
                _isRestoring = true;
                AssociatedObject.ScrollToVerticalOffset(_savedOffset);
            }
            else if (e.ExtentHeightChange < 0)
            {
                _isRestoring = false;
                if (e.VerticalOffset > 0)
                    _savedOffset = e.VerticalOffset;
            }
            else
            {
                if (_isRestoring)
                    _isRestoring = false;
                else if (e.VerticalOffset > 0)
                    _savedOffset = e.VerticalOffset;
            }
        }
    }
}
