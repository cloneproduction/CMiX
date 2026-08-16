// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Services
{
    // Replacement for the gong wpf dragdrop list reorder wiring.
    // Attach on a ListBox: Services:ListBoxReorderDragDrop.Service="{Binding ManagerReorderService}".
    // Drag starts from an item whose drag handle sets DragHandlerIsPressed on the service.
    public static class ListBoxReorderDragDrop
    {
        private const string ReorderFormat = "cmix/reorder";
        private const double DragThreshold = 4.0;

        public static readonly AttachedProperty<ManagerReorderService> ServiceProperty =
            AvaloniaProperty.RegisterAttached<ItemsControl, ManagerReorderService>(
                "Service",
                typeof(ListBoxReorderDragDrop));

        public static void SetService(ItemsControl element, ManagerReorderService value) => element.SetValue(ServiceProperty, value);
        public static ManagerReorderService GetService(ItemsControl element) => element.GetValue(ServiceProperty);

        static ListBoxReorderDragDrop()
        {
            ServiceProperty.Changed.AddClassHandler<ItemsControl>(OnServiceChanged);
        }

        private static void OnServiceChanged(ItemsControl listBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DragDrop.SetAllowDrop(listBox, true);
                listBox.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, global::Avalonia.Interactivity.RoutingStrategies.Tunnel);
                listBox.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, global::Avalonia.Interactivity.RoutingStrategies.Tunnel);
                listBox.AddHandler(DragDrop.DragOverEvent, OnDragOver);
                listBox.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
                listBox.AddHandler(DragDrop.DropEvent, OnDrop);
                listBox.DetachedFromVisualTree += OnListBoxDetachedFromVisualTree;
            }
            else
            {
                DragDrop.SetAllowDrop(listBox, false);
                listBox.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
                listBox.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
                listBox.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
                listBox.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
                listBox.RemoveHandler(DragDrop.DropEvent, OnDrop);
                listBox.DetachedFromVisualTree -= OnListBoxDetachedFromVisualTree;
                RemoveIndicator(listBox);
            }
        }

        // A ListBox that is realized and unrealized repeatedly (tab content, virtualized rows)
        // otherwise leaves its InsertionIndicator behind on the adorner layer every time.
        private static void OnListBoxDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            RemoveIndicator((ItemsControl)sender);
        }

        private static readonly AttachedProperty<Point?> PressPointProperty =
            AvaloniaProperty.RegisterAttached<ItemsControl, Point?>("PressPoint", typeof(ListBoxReorderDragDrop));

        private static readonly AttachedProperty<int> PressIndexProperty =
            AvaloniaProperty.RegisterAttached<ItemsControl, int>("PressIndex", typeof(ListBoxReorderDragDrop), -1);

        private static void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var listBox = (ItemsControl)sender;
            if (!e.GetCurrentPoint(listBox).Properties.IsLeftButtonPressed)
                return;

            // Walk up until an ancestor is recognized as an item container of this items control.
            var index = -1;
            for (var visual = e.Source as Visual; visual != null; visual = visual.GetVisualParent())
            {
                if (visual is Control candidate)
                {
                    var candidateIndex = listBox.IndexFromContainer(candidate);
                    if (candidateIndex >= 0)
                    {
                        index = candidateIndex;
                        break;
                    }
                }
            }

            if (index < 0)
                return;

            listBox.SetValue(PressPointProperty, e.GetPosition(listBox));
            listBox.SetValue(PressIndexProperty, index);
        }

        private static async void OnPointerMoved(object sender, PointerEventArgs e)
        {
            var listBox = (ItemsControl)sender;
            var service = GetService(listBox);
            var pressPoint = listBox.GetValue(PressPointProperty);
            var sourceIndex = listBox.GetValue(PressIndexProperty);

            if (service == null || pressPoint == null || sourceIndex < 0)
                return;

            if (!e.GetCurrentPoint(listBox).Properties.IsLeftButtonPressed)
            {
                listBox.SetValue(PressPointProperty, null);
                listBox.SetValue(PressIndexProperty, -1);
                return;
            }

            // The drag handle sets DragHandlerIsPressed, matching the WPF gong StartDrag guard.
            if (!service.DragHandlerIsPressed)
                return;

            var current = e.GetPosition(listBox);
            var dx = current.X - pressPoint.Value.X;
            var dy = current.Y - pressPoint.Value.Y;
            if (Math.Sqrt(dx * dx + dy * dy) < DragThreshold)
                return;

            listBox.SetValue(PressPointProperty, null);
            listBox.SetValue(PressIndexProperty, -1);

            var data = new DataObject();
            data.Set(ReorderFormat, sourceIndex);
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);

            HideIndicator(listBox);
            service.DragHandlerIsPressed = false;
        }

        private static int ComputeInsertIndex(ItemsControl listBox, Point position)
        {
            var count = listBox.ItemCount;
            for (var i = 0; i < count; i++)
            {
                if (listBox.ContainerFromIndex(i) is not Control container)
                    continue;

                var bounds = container.Bounds;
                var topLeft = container.TranslatePoint(new Point(0, 0), listBox) ?? bounds.TopLeft;
                var midY = topLeft.Y + bounds.Height / 2;
                if (position.Y < midY)
                    return i;
            }
            return count;
        }

        private static double IndicatorY(ItemsControl listBox, int insertIndex)
        {
            if (listBox.ItemCount == 0)
                return 0;

            if (insertIndex < listBox.ItemCount)
            {
                if (listBox.ContainerFromIndex(insertIndex) is Control container)
                    return container.TranslatePoint(new Point(0, 0), listBox)?.Y ?? 0;
            }

            if (listBox.ContainerFromIndex(listBox.ItemCount - 1) is Control last)
            {
                var topLeft = last.TranslatePoint(new Point(0, 0), listBox) ?? default;
                return topLeft.Y + last.Bounds.Height;
            }
            return 0;
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            var listBox = (ItemsControl)sender;
            var service = GetService(listBox);

            if (service == null || !e.Data.Contains(ReorderFormat))
            {
                e.DragEffects = DragDropEffects.None;
                return;
            }

            var sourceIndex = Convert.ToInt32(e.Data.Get(ReorderFormat));
            var insertIndex = ComputeInsertIndex(listBox, e.GetPosition(listBox));

            if (!service.CanDrop(sourceIndex, insertIndex))
            {
                e.DragEffects = DragDropEffects.None;
                HideIndicator(listBox);
                return;
            }

            e.DragEffects = DragDropEffects.Move;
            ShowIndicator(listBox, IndicatorY(listBox, insertIndex));
        }

        private static void OnDragLeave(object sender, DragEventArgs e)
        {
            HideIndicator((ItemsControl)sender);
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            var listBox = (ItemsControl)sender;
            var service = GetService(listBox);
            HideIndicator(listBox);

            if (service == null || !e.Data.Contains(ReorderFormat))
                return;

            var sourceIndex = Convert.ToInt32(e.Data.Get(ReorderFormat));
            var insertIndex = ComputeInsertIndex(listBox, e.GetPosition(listBox));

            if (!service.CanDrop(sourceIndex, insertIndex))
                return;

            service.Dropped(sourceIndex, insertIndex);
            e.Handled = true;
        }

        // Insertion indicator drawn on the adorner layer, replacing the WPF DropTargetAdorners.Insert.
        private static readonly AttachedProperty<InsertionIndicator> IndicatorProperty =
            AvaloniaProperty.RegisterAttached<ItemsControl, InsertionIndicator>("Indicator", typeof(ListBoxReorderDragDrop));

        private static void ShowIndicator(ItemsControl listBox, double y)
        {
            var indicator = listBox.GetValue(IndicatorProperty);
            if (indicator == null)
            {
                var layer = AdornerLayer.GetAdornerLayer(listBox);
                if (layer == null)
                    return;

                indicator = new InsertionIndicator();
                AdornerLayer.SetAdornedElement(indicator, listBox);
                layer.Children.Add(indicator);
                listBox.SetValue(IndicatorProperty, indicator);
            }

            indicator.LineY = y;
            indicator.IsVisible = true;
            indicator.InvalidateVisual();
        }

        private static void HideIndicator(ItemsControl listBox)
        {
            var indicator = listBox.GetValue(IndicatorProperty);
            if (indicator != null)
                indicator.IsVisible = false;
        }

        // The adorner layer is a different subtree from the listBox, so its ancestor lookup
        // (AdornerLayer.GetAdornerLayer) cannot be relied on once the listBox itself has already
        // detached; the indicator's own former parent is used instead.
        private static void RemoveIndicator(ItemsControl listBox)
        {
            var indicator = listBox.GetValue(IndicatorProperty);
            if (indicator == null)
                return;

            if (indicator.Parent is Panel layer)
                layer.Children.Remove(indicator);
            listBox.ClearValue(IndicatorProperty);
        }

        private class InsertionIndicator : Control
        {
            public double LineY { get; set; }

            public InsertionIndicator()
            {
                IsHitTestVisible = false;
            }

            public override void Render(DrawingContext context)
            {
                var brush = this.TryFindResource("Grey600Brush", out var resource) && resource is IBrush greyBrush
                    ? greyBrush
                    : Brushes.Gray;
                var pen = new Pen(brush, 2);
                context.DrawLine(pen, new Point(0, LineY), new Point(Bounds.Width, LineY));
            }
        }
    }
}
