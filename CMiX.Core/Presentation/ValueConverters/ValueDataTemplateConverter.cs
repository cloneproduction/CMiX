// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Presentation.ValueConverters
{
    public class ValueDataTemplateConverter : IValueConverter
    {
        public DataTemplate MeshEntityTemplate { get; set; }
        public DataTemplate LightEntityTemplate { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Mesh)
                return MeshEntityTemplate;

            if(value is LightEntity)
                return LightEntityTemplate;
            //if (value is ValueType valueType)
            //    switch (valueType)
            //    {
            //        case ValueType.TypeA:
            //            return TemplateA;
            //        case ValueType.TypeB:
            //            return TemplateB;
            //    }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
