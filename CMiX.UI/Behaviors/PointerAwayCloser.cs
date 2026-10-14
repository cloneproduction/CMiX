// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CMiX.UI.Behaviors
{
    // Closes a popup, a flyout or a menu when the pointer goes away from it.
    // The owner calls Start when the content opens and Stop when it closes.
    // The pointer is away when it is farther than the distance from the content and from the control that opened it.
    // It is not away after the user typed in a text box of the content, because a close would lose the text.
    // It does not close while a control of the content holds the pointer, for example a slider that is dragged,
    // because the drag goes on far from the content. It does nothing on the release itself: the control that held
    // the pointer ends its drag first, for example a slider that puts the cursor back on its thumb. The next move
    // of the pointer decides.
    internal sealed class PointerAwayCloser
    {
        public const double DefaultDistance = 64;

        private readonly Func<double> distance;
        private readonly Func<Control?> content;
        private readonly Func<Control?> target;
        private readonly Action close;
        private TopLevel? topLevel;
        private Control? watchedContent;
        private bool keyPressed;

        public PointerAwayCloser(Func<double> distance, Func<Control?> content, Func<Control?> target, Action close)
        {
            this.distance = distance;
            this.content = content;
            this.target = target;
            this.close = close;
        }

        public void Start(TopLevel? pointerSource)
        {
            Stop();
            topLevel = pointerSource;
            topLevel?.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);

            watchedContent = content();
            keyPressed = false;
            watchedContent?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        }

        public void Stop()
        {
            topLevel?.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            topLevel = null;
            watchedContent?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            watchedContent = null;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e) => keyPressed = true;

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var popupContent = content();
            if (popupContent == null)
                return;

            if (IsHeldByContent(e, popupContent))
                return;

            if (IsNear(popupContent, e) || IsNear(target(), e) || IsTyping(popupContent))
                return;

            close();
        }

        private static bool IsHeldByContent(PointerEventArgs e, Control popupContent) =>
            e.Pointer.Captured is Visual held && (held == popupContent || held.GetVisualAncestors().Contains(popupContent));

        private bool IsNear(Control? control, PointerEventArgs e)
        {
            if (control == null)
                return false;

            var size = control.Bounds.Size;
            var pos = e.GetPosition(control);
            var far = distance();

            return pos.X >= -far && pos.Y >= -far && pos.X <= size.Width + far && pos.Y <= size.Height + far;
        }

        // Some popups focus a text box when they open, so the focus alone does not mean typing.
        private bool IsTyping(Control popupContent) =>
            keyPressed && popupContent.GetVisualDescendants().OfType<TextBox>().Any(box => box.IsFocused);
    }
}
