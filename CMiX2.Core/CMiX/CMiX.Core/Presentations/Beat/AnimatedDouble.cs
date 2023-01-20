// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;

namespace CMiX.Core.Presentation.Controls
{
    public class AnimatedDouble : DependencyObject
    {
        public AnimatedDouble()
        {

        }

        public static readonly DependencyProperty AnimationPositionProperty =
        DependencyProperty.Register("AnimationPosition", typeof(double), typeof(AnimatedDouble), new FrameworkPropertyMetadata(0.0));
        public double AnimationPosition
        {
            get { return (double)GetValue(AnimationPositionProperty); }
            set { SetValue(AnimationPositionProperty, value); }
        }
    }
}
