// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CMiX.UI.Behaviors
{
    // Tells that the user types in a text box of a popup. A key press in the content since it opened
    // and a text box with the keyboard focus mean typing. Some popups focus a text box when they open,
    // so the focus alone does not mean typing.
    internal sealed class TypingGuard
    {
        private Control? content;
        private bool keyPressed;

        public bool IsTyping =>
            keyPressed && content != null && content.GetVisualDescendants().OfType<TextBox>().Any(box => box.IsFocused);

        public void Watch(Control? control)
        {
            Stop();
            content = control;
            keyPressed = false;
            content?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        }

        public void Stop()
        {
            content?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            content = null;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e) => keyPressed = true;
    }
}
