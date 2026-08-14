// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CMiX.Studio.Avalonia.AttachedProperties;

namespace CMiX.Studio.Avalonia.Views.Controls.Panels
{
    public class PositionedUniformGrid : UniformGrid
    {
        public PositionedUniformGrid()
        {
            // Avalonia panels have no OnVisualChildrenChanged; the children collection raises changes instead.
            Children.CollectionChanged += (s, e) => UpdateChildPositions();
        }

        private void UpdateChildPositions()
        {
            var children = Children;
            int count = children.Count;

            for (int i = 0; i < count; i++)
            {
                if (children[i] is Control child)
                {
                    ControlPosition position;

                    if (count == 1)
                        position = ControlPosition.All;
                    else if (i == 0)
                        position = Rows > Columns ? ControlPosition.Top : ControlPosition.Left;
                    else if (i == count - 1)
                        position = Rows > Columns ? ControlPosition.Bottom : ControlPosition.Right;
                    else
                        position = ControlPosition.None;

                    PositionedControl.SetPosition(child, position);
                }
            }
        }
    }
}
