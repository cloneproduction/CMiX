using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CMiX.UI.Controls
{
    public partial class ListBoxCMiX : ListBox
    {
        protected override Type StyleKeyOverride => typeof(ListBoxCMiX);

        static ListBoxCMiX()
        {
            MinItemCountProperty.Changed.AddClassHandler<ListBoxCMiX>((list, _) => list.UpdateHeight());
        }

        // The least number of whole rows that the list shows, for example the token ListVisibleItems. The user
        // can make the list taller, row by row, but not below this. Zero leaves the height to MinHeight.
        public static readonly StyledProperty<int> MinItemCountProperty =
            AvaloniaProperty.Register<ListBoxCMiX, int>(nameof(MinItemCount));

        public int MinItemCount
        {
            get => GetValue(MinItemCountProperty);
            set => SetValue(MinItemCountProperty, value);
        }

        // The height of a row and the space above and below the rows, from the tokens ControlHeightMD and
        // ListScrollMargin. Before the list is in a window the tokens are not found.
        private bool TryGetMetrics(out double rowHeight, out double verticalMargin)
        {
            rowHeight = 0;
            verticalMargin = 0;

            if (!this.TryFindResource("ControlHeightMD", ActualThemeVariant, out var row) || row is not double height)
                return false;

            if (!this.TryFindResource("ListScrollMargin", ActualThemeVariant, out var margin) || margin is not Thickness thickness)
                return false;

            rowHeight = height;
            verticalMargin = thickness.Top + thickness.Bottom;
            return true;
        }

        private void UpdateHeight()
        {
            if (MinItemCount <= 0 || !TryGetMetrics(out var rowHeight, out var verticalMargin))
                return;

            MinHeight = MinItemCount * rowHeight + verticalMargin;

            if (Bd != null)
                Bd.Height = MinHeight;
        }

        // The nearest height that shows a whole number of rows, and not less than the minimum.
        private double SnapHeight(double height, double minimum)
        {
            if (TryGetMetrics(out var rowHeight, out var verticalMargin) && rowHeight > 0)
            {
                var rows = Math.Max(0, Math.Round((height - verticalMargin) / rowHeight));
                height = rows * rowHeight + verticalMargin;
            }

            return Math.Max(height, minimum);
        }

        Border? Border { get; set; }
        Border? Bd { get; set; }

        private bool _resizing;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (Border != null)
            {
                Border.PointerMoved -= Border_PointerMoved;
                Border.PointerCaptureLost -= Border_PointerCaptureLost;
                Border.RemoveHandler(PointerPressedEvent, Border_PointerPressed);
                Border.RemoveHandler(PointerReleasedEvent, Border_PointerReleased);
            }

            if (Bd != null)
                Bd.RemoveHandler(PointerWheelChangedEvent, Bd_PointerWheelChanged);

            Border = e.NameScope.Find<Border>("resizeBorder");
            Bd = e.NameScope.Find<Border>("Bd");

            if (Border != null)
            {
                Border.PointerMoved += Border_PointerMoved;
                Border.PointerCaptureLost += Border_PointerCaptureLost;
                Border.AddHandler(PointerPressedEvent, Border_PointerPressed, RoutingStrategies.Tunnel);
                Border.AddHandler(PointerReleasedEvent, Border_PointerReleased, RoutingStrategies.Tunnel);
            }

            if (Bd != null)
            {
                UpdateHeight();
                Bd.Height = Bd.MinHeight;
                Bd.AddHandler(PointerWheelChangedEvent, Bd_PointerWheelChanged, RoutingStrategies.Tunnel);
            }
        }

        // Scroll the parent when the inner scroll viewer is at its limit.
        private void Bd_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            var scrollViewer = Bd?.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

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

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            e.Pointer.Capture(Border);
            _resizing = true;
            initPoint = e.GetPosition(this);
            currentHeight = Bd?.Bounds.Height ?? 0;
        }

        private void Border_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_resizing || Bd == null)
                return;

            var current = e.GetPosition(this);
            var pos = current.Y - initPoint.Y + currentHeight;

            Bd.Height = SnapHeight(pos, Bd.MinHeight);
        }

        // The resize ends when the pointer is taken away without a release.
        private void Border_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => _resizing = false;

        private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _resizing = false;
            e.Pointer.Capture(null);
        }
    }
}
