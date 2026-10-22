// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CMiX.UI.Panels
{
    public class UniformGridPositioned : UniformGrid
    {
        public UniformGridPositioned()
        {
            // Avalonia panels have no OnVisualChildrenChanged; the children collection raises changes instead.
            Children.CollectionChanged += (s, e) => UpdateChildPositions();
        }

        private void UpdateChildPositions()
        {
            bool vertical = Rows > Columns;
            ChildPositions.Assign(
                Children.OfType<Control>().ToList(),
                vertical ? ControlPosition.Top : ControlPosition.Left,
                vertical ? ControlPosition.Bottom : ControlPosition.Right);
        }
    }
}
