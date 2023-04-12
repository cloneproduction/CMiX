// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CMiX.Core.Presentations.Controls;
using CMiX.Core.Presentations.Beat;

namespace CMiX.Studio.Converters
{
    public class PeriodToBPMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float Period = (float)value;

            float BPM = 60000 / Period;

            if (float.IsInfinity(BPM) || float.IsNaN(BPM))
                return 0;

            return BPM;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 60000 / (float)value;
        }
    }

    public class MultiPeriodToBPMConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null || values[2] == null)
                return "0";

            var beatIndex = (int)values[1];
            var masterBeat = values[2] as MasterBeat;

            if (masterBeat == null)
                return DependencyProperty.UnsetValue;

            return String.Format("{0:C0}", (60000 / masterBeat.Periods[beatIndex + masterBeat.BeatIndex.Value]).ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiPeriodToBeatAnimationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return DependencyProperty.UnsetValue;

            var beatIndex = (int)values[0];
            var masterBeat = values[1] as MasterBeat;

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
