// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Behaviors
{
    // A drag over toggle buttons that sets them all to one state, as the eye column of a layer list does.
    // Install it once on a window. A press on a button with a ToggleDrag.Group starts a stroke. The press itself
    // toggles the button (the group puts the button in ClickMode.Press). The state of the button after the press is
    // the state that the stroke paints on every button of the same group that the pointer passes over.
    // The stroke ends when the mouse button is released or the first button loses the pointer.
    // StrokeStarted comes before the first button toggles, and StrokeEnded after the last button changed,
    // so a listener can make the whole stroke one undo step.
    public class ToggleDragBehavior : Behavior<Control>
    {
        // The pointer is checked at least every few pixels, so a fast move does not skip a row.
        private const double SampleStep = 6.0;

        private ToggleButton? first;
        private string? group;
        private bool paintValue;
        private Point last;

        public event EventHandler? StrokeStarted;
        public event EventHandler? StrokeEnded;

        public bool IsStroking => first != null;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
                return;

            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, OnMoved, RoutingStrategies.Bubble, true);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnReleased, RoutingStrategies.Bubble, true);
        }

        protected override void OnDetaching()
        {
            EndStroke();
            if (AssociatedObject != null)
            {
                AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPressed);
                AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, OnMoved);
                AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnReleased);
            }

            base.OnDetaching();
        }

        // The press is not handled here: the button toggles on it, and the other handlers of the window see it as before.
        private void OnPressed(object? sender, PointerPressedEventArgs e)
        {
            EndStroke();

            if (AssociatedObject == null || !e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
                return;

            if (GroupButtonAt(e.Source as Visual, null) is not { } button)
                return;

            first = button;
            group = ToggleDrag.GetGroup(button);
            paintValue = !(button.IsChecked ?? false);
            last = e.GetPosition(AssociatedObject);
            button.PointerCaptureLost += OnFirstCaptureLost;
            StrokeStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnMoved(object? sender, PointerEventArgs e)
        {
            if (first == null || AssociatedObject == null)
                return;

            if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
            {
                EndStroke();
                return;
            }

            var point = e.GetPosition(AssociatedObject);
            var distance = point - last;
            var steps = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y) / SampleStep));
            for (var step = 1; step <= steps; step++)
                Paint(last + distance * ((double)step / steps));

            last = point;
        }

        private void OnReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
                EndStroke();
        }

        private void OnFirstCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndStroke();

        // Sets the button at the point to the painted state. The first button is not touched: the press did it.
        private void Paint(Point point)
        {
            if (AssociatedObject is not IInputElement root)
                return;

            if (GroupButtonAt(root.InputHitTest(point) as Visual, group) is { } button
                && !ReferenceEquals(button, first)
                && button.IsChecked != paintValue)
                button.IsChecked = paintValue;
        }

        // The toggle button with a group that holds the visual. With a group name, only a button of that group counts.
        private static ToggleButton? GroupButtonAt(Visual? visual, string? onlyGroup)
        {
            for (var current = visual; current != null; current = current.GetVisualParent())
            {
                if (current is not ToggleButton button)
                    continue;

                var buttonGroup = ToggleDrag.GetGroup(button);
                if (string.IsNullOrEmpty(buttonGroup) || !button.IsEffectivelyEnabled)
                    return null;

                return onlyGroup == null || buttonGroup == onlyGroup ? button : null;
            }

            return null;
        }

        private void EndStroke()
        {
            if (first == null)
                return;

            first.PointerCaptureLost -= OnFirstCaptureLost;
            first = null;
            group = null;
            StrokeEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}
