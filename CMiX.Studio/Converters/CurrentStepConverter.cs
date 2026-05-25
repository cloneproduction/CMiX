using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.Converters
{
    public class CurrentStepConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine($"[CurrentStepConverter] values[0]={values[0]} values[1]={values[1]}");
            if (values[0] is int itemIndex && values[1] is int currentStep)
                return itemIndex == currentStep ? Brushes.OrangeRed : Brushes.Transparent;
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
