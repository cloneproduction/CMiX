using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using CMiX.Studio.Mathematics;

namespace CMiX.Studio.Converters
{
    public class RemapDoubleConverter : MarkupExtension, IValueConverter
    {
        public double FromMin { get; set; }
        public double FromMax { get; set; }
        public double ToMin { get; set; }
        public double ToMax { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = (double)value;
            return MathUtils.Map(d, FromMin, FromMax, ToMin, ToMax);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = (double)value;
            return MathUtils.Map(d, ToMin, ToMax, FromMin, FromMax);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
