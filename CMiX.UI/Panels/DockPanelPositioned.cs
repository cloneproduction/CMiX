// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CMiX.UI.Panels
{
    // A horizontal DockPanel. The controls take their positions in the order that they show on screen:
    // left side, then the control that fills, then the right side. A Popup is not part of the row.
    public class DockPanelPositioned : DockPanel
    {
        public DockPanelPositioned()
        {
            // Avalonia panels have no OnVisualChildrenChanged; the children collection raises changes instead.
            Children.CollectionChanged += (s, e) => UpdateChildPositions();
        }

        private void UpdateChildPositions() =>
            ChildPositions.Assign(ScreenOrder(), ControlPosition.Left, ControlPosition.Right);

        private List<Control> ScreenOrder()
        {
            var row = Children.OfType<Control>().Where(child => child is not Popup).ToList();
            var filler = LastChildFill ? row.LastOrDefault() : null;
            var docked = row.Where(child => child != filler).ToList();

            var left = docked.Where(child => GetDock(child) != Dock.Right);
            var right = docked.Where(child => GetDock(child) == Dock.Right).Reverse();

            return left.Concat(filler is null ? [] : [filler]).Concat(right).ToList();
        }
    }
}
