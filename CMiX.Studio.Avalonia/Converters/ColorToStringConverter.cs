// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Converters
{
    public class ColorToStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Color colorValue = (Color)value;
            return ColorNames.GetColorName(colorValue);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class ColorNames
    {
        static ColorNames()
        {
            m_colorNames = new Dictionary<Color, string>();
        }

        static public String GetColorName(Color colorToSeek)
        {
            if (m_colorNames.ContainsKey(colorToSeek))
                return m_colorNames[colorToSeek];
            else
                return colorToSeek.ToString();
        }

        static private Dictionary<Color, String> m_colorNames;
    }
}
