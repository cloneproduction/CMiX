// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class CMiXListBox : ListBox
    {
        protected override Type StyleKeyOverride => typeof(CMiXListBox);

        Border Border { get; set; }
        Border Bd { get; set; }

        private bool _resizing;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            Border = e.NameScope.Find<Border>("resizeBorder");
            Bd = e.NameScope.Find<Border>("Bd");

            if (Border != null)
            {
                Border.PointerMoved += Border_PointerMoved;
                Border.AddHandler(PointerPressedEvent, Border_PointerPressed, RoutingStrategies.Tunnel);
                Border.AddHandler(PointerReleasedEvent, Border_PointerReleased, RoutingStrategies.Tunnel);
            }

            if (Bd != null)
            {
                Bd.Height = Bd.MinHeight;
                Bd.AddHandler(PointerWheelChangedEvent, Bd_PointerWheelChanged, RoutingStrategies.Tunnel);
            }
        }

        // Scroll the parent when the inner scroll viewer is at its limit.
        private void Bd_PointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            var scrollViewer = Bd.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

            if (scrollViewer == null)
                return;

            var scrollPos = scrollViewer.Offset.Y;
            var scrollableHeight = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);

            if ((scrollPos >= scrollableHeight && e.Delta.Y < 0)
                || (scrollPos <= 0 && e.Delta.Y > 0))
            {
                e.Handled = true;
                var parentScrollViewer = this.FindAncestorOfType<ScrollViewer>();
                if (parentScrollViewer != null)
                {
                    var offset = parentScrollViewer.Offset;
                    parentScrollViewer.Offset = offset.WithY(offset.Y - e.Delta.Y * 50);
                }
            }
        }

        Point initPoint = new();
        double currentHeight;

        private void Border_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            e.Pointer.Capture(Border);
            _resizing = true;
            initPoint = e.GetPosition(this);
            currentHeight = Bd.Bounds.Height;
        }

        private void Border_PointerMoved(object sender, PointerEventArgs e)
        {
            if (!_resizing)
                return;

            var current = e.GetPosition(this);
            var pos = current.Y - initPoint.Y + currentHeight;

            if (pos > 0)
                Bd.Height = pos;

            if (pos <= Bd.MinHeight)
                Bd.Height = Bd.MinHeight;
        }

        private void Border_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            _resizing = false;
            e.Pointer.Capture(null);
        }
    }
}
