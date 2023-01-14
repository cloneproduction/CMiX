// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public class ColorManager : IColorManager, IColorStateStorage
    {
        private readonly List<IColorClient> _colorClients = new List<IColorClient>();
        public event Action<Color> ColorChanged;


        private ColorState _colorState;
        public ColorState ColorState
        {
            get { return _colorState; }
            set { _colorState = value; }
        }


        private Color _currentColor;
        public Color CurrentColor
        {
            get => _currentColor;
            set => _currentColor = value;
        }


        public NotifyableColor Color
        {
            get;
            set;
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
            Color = new NotifyableColor(this);
            Color.PropertyChanged += Color_PropertyChanged;
        }


        private void Color_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var newColor = System.Windows.Media.Color.FromArgb((byte)Color.A, (byte)Color.RGB_R, (byte)Color.RGB_G, (byte)Color.RGB_B);
            ColorChanged?.Invoke(newColor);

            CurrentColor = newColor;
        }
    }
}
