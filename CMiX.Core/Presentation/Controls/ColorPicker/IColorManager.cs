// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentation.Controls
{
    public interface IColorManager
    {
        event Action<System.Windows.Media.Color> ColorChanged;

        System.Windows.Media.Color CurrentColor { get; set; }
        ColorState ColorState { get; set; }

        NotifyableColor Color { get; set; }
        void AddClient(params IColorClient[] clients);

        //void SetColorFromHsl(double hue, double saturation, double lightness);
    }
}
