// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Views.BaseControl
{
    public class NotifyableColor : ObservableObject
    {
        private readonly IColorStateStorage storage;
        public NotifyableColor(IColorStateStorage colorStateStorage)
        {
            storage = colorStateStorage;
        }


        public void UpdateARGB(Color color)
        {
            if (RGB_R != color.R)
                RGB_R = color.R;
            if (RGB_G != color.G)
                RGB_G = color.G;
            if (RGB_B != color.B)
                RGB_B = color.B;
            if (A != color.A)
                A = color.A;
        }


        public Color GetColor()
        {
            return Color.FromArgb((byte)A, (byte)RGB_R, (byte)RGB_G, (byte)RGB_B);
        }


        public double A
        {
            get => storage.ColorState.A * 255;
            set
            {
                var state = storage.ColorState;
                state.A = value / 255;
                storage.ColorState = state;
                OnPropertyChanged(nameof(A));
            }
        }

        public double RGB_R
        {
            get => storage.ColorState.RGB_R * 255;
            set
            {
                var state = storage.ColorState;
                state.RGB_R = value / 255;
                storage.ColorState = state;
                OnPropertyChanged(nameof(RGB_R));
            }
        }

        public double RGB_G
        {
            get => storage.ColorState.RGB_G * 255;
            set
            {
                var state = storage.ColorState;
                state.RGB_G = value / 255;
                storage.ColorState = state;
                OnPropertyChanged(nameof(RGB_G));
            }
        }

        public double RGB_B
        {
            get => storage.ColorState.RGB_B * 255;
            set
            {
                var state = storage.ColorState;
                state.RGB_B = value / 255;
                storage.ColorState = state;
                OnPropertyChanged(nameof(RGB_B));
            }
        }

        public double HSV_H
        {
            get => storage.ColorState.HSV_H;
            set
            {
                var state = storage.ColorState;
                state.HSV_H = value;
                storage.ColorState = state;
                OnPropertyChanged(nameof(HSV_H));
            }
        }

        public double HSV_S
        {
            get => storage.ColorState.HSV_S;
            set
            {
                var state = storage.ColorState;
                state.HSV_S = value;
                storage.ColorState = state;
                OnPropertyChanged(nameof(HSV_S));
            }
        }

        public double HSV_V
        {
            get => storage.ColorState.HSV_V * 100;
            set
            {
                var state = storage.ColorState;
                state.HSV_V = value / 100;
                storage.ColorState = state;
                OnPropertyChanged(nameof(HSV_V));
            }
        }
        public double HSL_H
        {
            get => storage.ColorState.HSL_H;
            set
            {
                var state = storage.ColorState;
                state.HSL_H = value;
                storage.ColorState = state;
                OnPropertyChanged(nameof(HSL_H));
            }
        }

        public double HSL_S
        {
            get => storage.ColorState.HSL_S * 100;
            set
            {
                var state = storage.ColorState;
                state.HSL_S = value / 100;
                storage.ColorState = state;
                OnPropertyChanged(nameof(HSL_S));
            }
        }

        public double HSL_L
        {
            get => storage.ColorState.HSL_L * 100;
            set
            {
                var state = storage.ColorState;
                state.HSL_L = value / 100;
                storage.ColorState = state;
                OnPropertyChanged(nameof(HSL_L));
            }
        }
    }
}
