// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views
{
    // The last place of the pointer over the new tree, in the control that holds its menus, as TreePointerInput.LastPointer.
    internal sealed class ListTreeLastPointer
    {
        private readonly Control owner;
        private readonly Control host;

        public ListTreeLastPointer(Control owner, Control host)
        {
            this.owner = owner;
            this.host = host;
        }

        public Point Last { get; private set; }

        public void Attach() => owner.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);

        public void Detach() => owner.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);

        private void OnPointerMoved(object? sender, PointerEventArgs e) => Last = e.GetPosition(host);
    }
}
