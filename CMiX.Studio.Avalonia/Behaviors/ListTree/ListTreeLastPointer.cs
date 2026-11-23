// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The last place of the pointer over the new tree, in the control that holds its menus, as TreePointerInput.LastPointer.
    internal sealed class ListTreeLastPointer : ListTreeBehavior
    {
        public Point Last { get; private set; }

        protected override void OnWatch(ProjectTree tree) =>
            Context.Owner.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);

        protected override void OnUnwatch() => Context.Owner.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);

        private void OnPointerMoved(object? sender, PointerEventArgs e) => Last = e.GetPosition(Context.Scroller);
    }
}
