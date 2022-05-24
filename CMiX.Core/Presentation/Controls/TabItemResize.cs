// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CMiX.Core.Presentation.Controls
{
    public class UniformTabPanel : UniformGrid
    {
        public UniformTabPanel()
        {
            IsItemsHost = true;
            Rows = 1;
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var children = this.Children.OfType<TabItem>();
            var totalMaxWidth = children.Sum(tab => tab.MaxWidth);
            if (!double.IsInfinity(totalMaxWidth))
            {
                this.HorizontalAlignment = (constraint.Width > totalMaxWidth)
                                                    ? HorizontalAlignment.Left
                                                    : HorizontalAlignment.Stretch;
                //foreach (var child in children)
                //{
                //    child.Width = this.HorizontalAlignment == System.Windows.HorizontalAlignment.Left
                //            ? child.MaxWidth
                //            : Double.NaN;
                //}
            }
            return base.MeasureOverride(constraint);
        }
    }
}
