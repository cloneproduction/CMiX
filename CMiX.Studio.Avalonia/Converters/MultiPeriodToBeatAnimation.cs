// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Animations;

namespace CMiX.Studio.Avalonia.Converters
{
    public class MultiPeriodToBeatAnimationConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2
                || values[0] == AvaloniaProperty.UnsetValue
                || values[1] is not BeatModifier { MasterBeat.AnimatedDoubleProvider: not null } beatModifier)
            {
                return AvaloniaProperty.UnsetValue;
            }

            var index = beatModifier.BeatIndex.Value + beatModifier.MasterBeat.BeatIndex.Value;
            var animatedDouble = beatModifier.MasterBeat.AnimatedDoubleProvider(index);
            return animatedDouble?.AnimationPosition ?? (object)AvaloniaProperty.UnsetValue;
        }
    }
}
