// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Converters
{
    public class MultiPeriodToBeatAnimationConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2
                || values[0] == AvaloniaProperty.UnsetValue
                || values[1] is not BeatModulator { MasterBeat.AnimatedDoubleProvider: not null } beatModulator)
            {
                return AvaloniaProperty.UnsetValue;
            }

            var index = beatModulator.BeatIndex.Value + beatModulator.MasterBeat.BeatIndex.Value;
            var animatedDouble = beatModulator.MasterBeat.AnimatedDoubleProvider(index);
            return animatedDouble?.AnimationPosition ?? (object)AvaloniaProperty.UnsetValue;
        }
    }
}
