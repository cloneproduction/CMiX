// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The focus after a press in the new tree: a press beside the rows focuses the cursor row, a row button gives the focus back.
    internal sealed class ListTreePressFocus : ListTreeBehavior
    {
        private sealed record FocusBefore(IInputElement? Control);

        private ProjectTree? tree;

        private bool pressWasBesideRows;

        private FocusBefore? focusBeforeRowButton;

        protected override void OnWatch(ProjectTree attached) => tree = attached;

        protected override void OnUnwatch()
        {
            pressWasBesideRows = false;
            focusBeforeRowButton = null;
            tree = null;
        }

        public void BeginPress() => focusBeforeRowButton = null;

        public void RememberFocusBeforeRowButton() => focusBeforeRowButton = new FocusBefore(Context.Focus.FocusedControl());

        public void FocusCursorRowAfterPress() => pressWasBesideRows = true;

        // A toggle cannot take the focus, so the focus goes back to the control before the press, else to the cursor row.
        public void EndPress()
        {
            if (pressWasBesideRows)
            {
                pressWasBesideRows = false;
                if (tree?.CursorNode is { } cursor)
                    Context.Lifetime.Post(() => Part<ListTreeFocusKeeper>().FocusRow(cursor), DispatcherPriority.Input);
                return;
            }

            if (focusBeforeRowButton is not { } before)
                return;

            focusBeforeRowButton = null;
            var focus = Context.Focus;
            if (ReferenceEquals(focus.FocusedControl(), before.Control))
                return;

            if (before.Control is InputElement { IsEffectivelyVisible: true } control && focus.IsInPanel(control) && control.GetVisualRoot() != null)
                control.Focus(control is RowListTree ? NavigationMethod.Tab : NavigationMethod.Unspecified);
            else if (tree?.CursorNode is { } cursor)
                Context.Rows.RowOf(cursor)?.Focus(NavigationMethod.Tab);
        }
    }
}
