// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace CMiX.Studio.Converters
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
            //FillColorNames();
        }

        static public String GetColorName(Color colorToSeek)
        {
            if (m_colorNames.ContainsKey(colorToSeek))
                return m_colorNames[colorToSeek];
            else
                return colorToSeek.ToString();
        }



        public static void FillColorNames()
        {
            Type colorsType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] colorsProperties = colorsType.GetProperties();

            foreach (PropertyInfo colorProperty in colorsProperties)
            {
                String colorName = colorProperty.Name;
                Color color = (Color)colorProperty.GetValue(null, null);

                // Path - Aqua is the same as Magenta - so we add 1 to red to avoid collision
                if (colorName == "Aqua")
                    color.R++;

                if (colorName == "Fuchsia")
                    color.G++;

                m_colorNames.Add(color, colorName);
            }
        }


        static private Dictionary<Color, String> m_colorNames;

    }
}