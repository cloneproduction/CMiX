// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;
using CMiX.Studio.AttachedProperties;

namespace CMiX.Studio.Views.BaseControl.Panels
{
    public class PositionedUniformGrid : UniformGrid
    {
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            UpdateChildPositions();
        }

        private void UpdateChildPositions()
        {
            var children = InternalChildren;
            int count = children.Count;

            for (int i = 0; i < count; i++)
            {
                if (children[i] is UIElement child)
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
