// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CMiX.Core.Animations;

namespace CMiX.Studio.Converters
{
    public class MultiPeriodToBeatAnimationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            if (values[0] == null || values[1] == null)
                    return DependencyProperty.UnsetValue;

            var beatIndex = (int)values[0];
            var masterBeat = (MasterBeat)values[1];

            if(masterBeat == null)
                return DependencyProperty.UnsetValue;

            return masterBeat.BeatAnimations.AnimatedDoubles[beatIndex + masterBeat.BeatIndex.Value].AnimationPosition;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
