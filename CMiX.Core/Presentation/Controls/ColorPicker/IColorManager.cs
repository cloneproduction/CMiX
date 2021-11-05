// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Drawing;

namespace CMiX.Core.Presentation.Controls
{
    public interface IColorManager
    {
        event Action<Color> ColorChanged;

        Color CurrentColor { get; set; }

        void AddClient(params IColorClient[] clients);

        void SetColorFromHsl(double hue, double saturation, double lightness);
    }
}
