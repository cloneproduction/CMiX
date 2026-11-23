// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The focus after a press in the new tree: a press beside the rows focuses the cursor row, a row button gives the focus back.
    internal sealed class ListTreePressFocus
    {
        private sealed record FocusBefore(IInputElement? Control);

        private readonly PanelListTreeRows rows;
        private readonly ListTreeFocus focus;
        private readonly ListTreeFocusKeeper focusKeeper;
        private readonly TreeViewLifetime lifetime;

        private ProjectTree? tree;

        private bool pressWasBesideRows;

        private FocusBefore? focusBeforeRowButton;

        public ListTreePressFocus(PanelListTreeRows rows, ListTreeFocus focus, ListTreeFocusKeeper focusKeeper, TreeViewLifetime lifetime)
        {
            this.rows = rows;
            this.focus = focus;
            this.focusKeeper = focusKeeper;
            this.lifetime = lifetime;
        }

        public void Attach(ProjectTree attached) => tree = attached;

        public void Detach()
        {
            pressWasBesideRows = false;
            focusBeforeRowButton = null;
            tree = null;
        }

        public void BeginPress() => focusBeforeRowButton = null;

        public void RememberFocusBeforeRowButton() => focusBeforeRowButton = new FocusBefore(focus.FocusedControl());

        public void FocusCursorRowAfterPress() => pressWasBesideRows = true;

        // A toggle cannot take the focus, so the focus goes back to the control before the press, else to the cursor row.
        public void EndPress()
        {
            if (pressWasBesideRows)
            {
                pressWasBesideRows = false;
                if (tree?.CursorNode is { } cursor)
                    lifetime.Post(() => focusKeeper.FocusRow(cursor), DispatcherPriority.Input);
                return;
            }

            if (focusBeforeRowButton is not { } before)
                return;

            focusBeforeRowButton = null;
            if (ReferenceEquals(focus.FocusedControl(), before.Control))
                return;

            if (before.Control is InputElement { IsEffectivelyVisible: true } control && focus.IsInPanel(control) && control.GetVisualRoot() != null)
                control.Focus(control is RowListTree ? NavigationMethod.Tab : NavigationMethod.Unspecified);
            else if (tree?.CursorNode is { } cursor)
                rows.RowOf(cursor)?.Focus(NavigationMethod.Tab);
        }
    }
}
