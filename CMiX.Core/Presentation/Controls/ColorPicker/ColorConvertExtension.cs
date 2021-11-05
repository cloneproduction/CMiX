// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public static class ColorConvertExtensions
    {
        public static System.Drawing.Color ToColor(this Color color)
            => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        public static Color ToColor(this System.Drawing.Color color)
            => Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}
