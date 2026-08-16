// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Converters
{
    public class CurrentStepConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var activeBrush = parameter as IBrush ?? Brushes.OrangeRed;

            if (values.Count > 1 && values[0] is int itemIndex && values[1] is int currentStep)
                return itemIndex == currentStep ? activeBrush : Brushes.Transparent;
            return Brushes.Transparent;
        }
    }
}
