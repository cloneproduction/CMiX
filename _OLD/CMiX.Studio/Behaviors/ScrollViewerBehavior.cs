// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CMiX.Studio.Behaviors
{
    public class ScrollViewerBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged; ;
        }

        double extendHeight = 0;
        double verticalOffset = 0;
        private void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //Console.WriteLine(" --------- ");
            //Console.WriteLine("VerticalOffset " + e.VerticalOffset);
            //Console.WriteLine("ViewportHeight " + e.ViewportHeight);
            //Console.WriteLine("ExtentHeight " + e.ExtentHeight);
            //Console.WriteLine("ExtentHeightChange " + e.ExtentHeightChange);
            //Console.WriteLine("ScrollableHeight " + AssociatedObject.ScrollableHeight);
            if (e.ViewportHeight == e.ExtentHeight) // this happen when creating new Transform Modifier for example...
                return;

            if (e.ExtentHeightChange == 0.0)
            {
                verticalOffset = e.VerticalOffset;
                extendHeight = e.ExtentHeight;
                return;
            }

            if (verticalOffset > 0.0 && verticalOffset < extendHeight - e.ViewportHeight)
            {
                this.AssociatedObject.ScrollToVerticalOffset(verticalOffset);
                verticalOffset = e.VerticalOffset;
                extendHeight = e.ExtentHeight;
                return;
            }

            if (verticalOffset == extendHeight - e.ViewportHeight) //resized and bottom
            {
                this.AssociatedObject.ScrollToVerticalOffset(verticalOffset);
                verticalOffset = e.VerticalOffset;
                extendHeight = e.ExtentHeight;
                return;
            }

            if (extendHeight < e.ViewportHeight) //not resized but at the bottom
            {
                AssociatedObject.ScrollToTop();
                verticalOffset = e.VerticalOffset;
                extendHeight = e.ExtentHeight;
                return;
            }

            verticalOffset = e.VerticalOffset;
            extendHeight = e.ExtentHeight;
        }
    }
}
