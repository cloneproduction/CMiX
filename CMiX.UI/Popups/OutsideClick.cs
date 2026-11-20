// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CMiX.UI.Behaviors;

namespace CMiX.UI.Popups
{
    // A press outside the open popups (a panel, a dropdown, a menu, a flyout) only ends them, and does nothing else: the control
    // under the pointer gets neither the press nor its release, so a click does not select a row and a right click opens no menu.
    // When a popup that the press is outside of has a value in edit mode, the press ends the edit and the popup stays open. Else
    // the press closes every popup that it is outside of. A popup that holds the point stays open, and so do the popups that were
    // open before it: a press in a menu closes the panel that opened beside the menu, not the menu. A tooltip is not a popup here.
    // Install it once for the application. It listens on every top level, the windows and the popups.
    public static class OutsideClick
    {
        // The open popups, in the order that they opened.
        private static readonly List<Popup> open = new();
        private static bool installed;

        // The pointer of a press that was taken. Its release is taken too.
        private static IPointer? taken;

        public static void Install()
        {
            if (installed)
                return;

            installed = true;
            Popup.IsOpenProperty.Changed.AddClassHandler<Popup>((popup, _) =>
            {
                var wasOpen = open.Remove(popup);
                if (popup.IsOpen)
                    open.Add(popup);
                else if (wasOpen && Counts(popup, ignoreOpen: true))
                    ClosedNow(popup);
            });
            InputElement.PointerPressedEvent.AddClassHandler<TopLevel>(OnPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
            InputElement.PointerReleasedEvent.AddClassHandler<TopLevel>(OnReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
        }

        // Takes the press and its release, so no control sees them. A control uses it when a press only ends what the control does.
        public static void Take(PointerPressedEventArgs e)
        {
            e.Handled = true;
            taken = e.Pointer;
        }

        private static void OnPressed(TopLevel root, PointerPressedEventArgs e)
        {
            if (e.Handled)
                return;

            var point = root.PointToScreen(e.GetPosition(root));
            var window = WindowOf(root);

            // A menu closes itself on the press, before the press reaches the windows. The press closed it, so it does nothing else.
            if (closedNow.Contains(window))
            {
                Take(e);
                return;
            }

            var popups = open.Where(popup => Counts(popup) && ReferenceEquals(WindowOf(popup), window)).ToList();
            var inside = popups.FindLastIndex(popup => Holds(popup, point));
            var outside = popups.Skip(inside + 1).ToList();
            if (outside.Count == 0)
                return;

            // The editors end their edit, so the text is kept, and the popup stays open until the next press outside: the pointer
            // going away does not close it any more.
            var editing = outside.Where(popup => PointerAwayCloser.EndEdits(popup.Child)).ToList();
            foreach (var behavior in editing.SelectMany(popup => global::Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(popup).OfType<ClosePopupBehavior>()))
                behavior.EditEndedOutside();

            if (editing.Count == 0)
            {
                for (var i = outside.Count - 1; i >= 0; i--)
                    Close(outside[i]);
            }

            Take(e);
        }

        // The popup of a menu item, a drop-down or a context menu takes IsOpen from its owner with a binding of its template. A value set
        // with IsOpen = false is stronger than that binding and stays: the owner opens again, but the popup does not. SetCurrentValue
        // keeps the binding, so the owner closes with the popup and can open it again. A popup of the menu bar of the window closes the
        // whole menu, so the bar leaves its open mode.
        private static void Close(Popup popup)
        {
            if (popup.TemplatedParent is MenuItem item && item.FindLogicalAncestorOfType<Menu>() is { } bar)
            {
                bar.Close();
                if (!popup.IsOpen)
                    return;
            }

            popup.SetCurrentValue(Popup.IsOpenProperty, false);
        }

        private static void OnReleased(TopLevel root, PointerReleasedEventArgs e)
        {
            if (taken == null || !ReferenceEquals(taken, e.Pointer))
                return;

            taken = null;
            e.Handled = true;
        }

        // The window of a popup. A popup of a context menu or a flyout is not in the tree of a window: its target or its content tells.
        private static TopLevel? WindowOf(Popup popup) =>
            WindowOf(TopLevel.GetTopLevel(popup) ?? (popup.PlacementTarget is { } target ? TopLevel.GetTopLevel(target) : null)
                ?? (popup.Child is { } child ? TopLevel.GetTopLevel(child) : null));

        // The window under a top level: the top level itself, or the window that opened a popup, through every popup of the chain.
        // Only the popups of the window that got the press count, so a popup of a window that closed does not.
        private static TopLevel? WindowOf(TopLevel? topLevel)
        {
            while (topLevel is PopupRoot { ParentTopLevel: { } parent })
                topLevel = parent;

            return topLevel;
        }

        // The windows whose popups closed in the input that runs now. The list is cleared after that input.
        private static readonly List<TopLevel> closedNow = new();

        private static void ClosedNow(Popup popup)
        {
            if (WindowOf(popup) is not { } window)
                return;

            if (closedNow.Count == 0)
                Dispatcher.UIThread.Post(closedNow.Clear, DispatcherPriority.Background);
            closedNow.Add(window);
        }

        // A tooltip and a popup that takes no pointer do not count.
        private static bool Counts(Popup popup) => Counts(popup, ignoreOpen: false);

        private static bool Counts(Popup popup, bool ignoreOpen) =>
            (ignoreOpen || popup.IsOpen) && popup.Child is Control child && child is not ToolTip && child.IsHitTestVisible && child.IsEffectivelyVisible;

        private static bool Holds(Popup popup, PixelPoint point)
        {
            if (popup.Child is not Control child || TopLevel.GetTopLevel(child) == null)
                return false;

            var topLeft = child.PointToScreen(new Point(0, 0));
            var bottomRight = child.PointToScreen(new Point(child.Bounds.Width, child.Bounds.Height));
            return new PixelRect(topLeft, bottomRight).Contains(point);
        }
    }
}
