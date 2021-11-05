// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMiX.Core.Presentation.Controls
{
    public class ColorManager : IColorManager
    {
        private readonly List<IColorClient> _colorClients = new List<IColorClient>();
        public event Action<Color> ColorChanged;


        private Color _currentColor;
        public Color CurrentColor
        {
            get => _currentColor;
            set => UpdateClients(value);
        }


        public void AddClient(params IColorClient[] clients)
        {
            foreach (var client in clients)
            {
                if (client == null)
                    continue;

                _colorClients.Add(client);
                client.Init(this);
            }
        }


        public void SetColorFromHsl(double hue, double saturation, double lightness)
        {
            var alpha = CurrentColor.A;

            var hsl = new HslColor(alpha, hue, saturation, lightness);

            CurrentColor = hsl.ToRgbColor();
        }


        private void UpdateClients(Color color)
        {
            _currentColor = color;

            foreach (var colorClient in _colorClients)
                colorClient.ColorUpdated(color, colorClient);

            ColorChanged?.Invoke(color);
        }
    }
}
