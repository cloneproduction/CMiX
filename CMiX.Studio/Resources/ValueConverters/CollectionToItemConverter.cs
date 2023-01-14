// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace CMiX.Studio.ValueConverters
{
    public class CollectionToItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<AnimatedDouble> col = new ObservableCollection<AnimatedDouble>();
            int index = (int)parameter;
            return ((ObservableCollection < AnimatedDouble > )value)[3].AnimationPosition;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}