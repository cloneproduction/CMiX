// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Views
{
    // The tooltip service of Avalonia does not open the tooltips of the rows in the running app,
    // so the tree switches it off for its rows and does the same work here.
    internal sealed class TreeTooltips
    {
        internal const double WakeDistance = 4;

        private readonly DispatcherTimer timer = new();
        private readonly Control rows;
        private Control? hovered;
        private Control? open;
        private TopLevel? keySource;

        private Point? lastPoint;

        private Point? keyPoint;

        private bool isMuted;

        public TreeTooltips(Control rows)
        {
            this.rows = rows;
            ToolTip.SetServiceEnabled(rows, false);
            rows.AddHandler(InputElement.PointerMovedEvent, OnMoved, RoutingStrategies.Bubble, handledEventsToo: true);
            rows.AddHandler(InputElement.PointerPressedEvent, (_, _) => Hover(null), RoutingStrategies.Tunnel, handledEventsToo: true);
            rows.PointerExited += (_, _) => Hover(null);
            rows.AttachedToVisualTree += (_, _) => ListenToKeys(TopLevel.GetTopLevel(rows));
            rows.DetachedFromVisualTree += (_, _) => ListenToKeys(null);
            ListenToKeys(TopLevel.GetTopLevel(rows));
            timer.Tick += (_, _) => Show();
        }

        // For the tests.
        internal Control? Hovered => hovered;

        internal Control? Open => open;

        internal bool IsMuted => isMuted;

        // Also the keys that a key binding or the tree handles close the tooltip.
        private void ListenToKeys(TopLevel? topLevel)
        {
            if (ReferenceEquals(topLevel, keySource))
                return;

            keySource?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            keySource = topLevel;
            keySource?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        }

        private void OnKeyDown(object? sender, KeyEventArgs e) => Mute();

        internal void Mute()
        {
            Hover(null);
            isMuted = true;
            keyPoint = lastPoint;
        }

        private void OnMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(rows);
            lastPoint = point;
            if (isMuted)
            {
                if (keyPoint is not { } from)
                {
                    keyPoint = point;
                    return;
                }

                var delta = point - from;
                if (delta.X * delta.X + delta.Y * delta.Y <= WakeDistance * WakeDistance)
                    return;

                isMuted = false;
                keyPoint = null;
            }

            Hover(HostOf(e.Source as Visual));
        }

        internal static Control? HostOf(Visual? visual)
        {
            for (var current = visual; current != null; current = current.GetVisualParent())
                if (current is Control control && ToolTip.GetTip(control) != null && (control.IsEffectivelyEnabled || ToolTip.GetShowOnDisabled(control)))
                    return control;

            return null;
        }

        internal void Hover(Control? host)
        {
            if (ReferenceEquals(host, hovered))
                return;

            timer.Stop();
            Close();
            hovered = host;
            if (host == null)
                return;

            var delay = ToolTip.GetShowDelay(host);
            if (delay <= 0)
            {
                Show();
                return;
            }

            timer.Interval = TimeSpan.FromMilliseconds(delay);
            timer.Start();
        }

        internal void Show()
        {
            timer.Stop();
            if (hovered is not { } host || !host.IsAttachedToVisualTree())
                return;

            ToolTip.SetIsOpen(host, true);
            open = host;
        }

        private void Close()
        {
            if (open == null)
                return;

            ToolTip.SetIsOpen(open, false);
            open = null;
        }
    }
}
