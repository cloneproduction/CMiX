// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Views.BaseControl
{
    public class NotifyableColor : ObservableObject
    {

        public NotifyableColor(IColorStateStorage colorStateStorage)
        {
            storage = colorStateStorage;
        }

        private readonly IColorStateStorage storage;
        private bool isUpdating = false;


        public Color GetColor()
        {
            return Color.FromArgb((byte)A, (byte)RGB_R, (byte)RGB_G, (byte)RGB_B);
        }


        public void UpdateEverything()
        {
            if (isUpdating) return;
            isUpdating = true;

            OnPropertyChanged(nameof(A));
            OnPropertyChanged(nameof(RGB_R));
            OnPropertyChanged(nameof(RGB_G));
            OnPropertyChanged(nameof(RGB_B));
            OnPropertyChanged(nameof(HSV_H));
            OnPropertyChanged(nameof(HSV_S));
            OnPropertyChanged(nameof(HSV_V));
            OnPropertyChanged(nameof(HSL_H));
            OnPropertyChanged(nameof(HSL_S));
            OnPropertyChanged(nameof(HSL_L));

            isUpdating = false;
        }


        public double A
        {
            get => storage.ColorState.A * 255;
            set 
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.A = value / 255;
                storage.ColorState = state;
            } 
        }

        public double RGB_R
        {
            get => storage.ColorState.RGB_R * 255;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.RGB_R = value / 255;
                storage.ColorState = state;
            }
        }

        public double RGB_G
        {
            get => storage.ColorState.RGB_G * 255;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.RGB_G = value / 255;
                storage.ColorState = state;
            }
        }

        public double RGB_B
        {
            get => storage.ColorState.RGB_B * 255;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.RGB_B = value / 255;
                storage.ColorState = state;
            }
        }

        public double HSV_H
        {
            get => storage.ColorState.HSV_H;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.HSV_H = value;
                storage.ColorState = state;
            }
        }

        public double HSV_S
        {
            get => storage.ColorState.HSV_S;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.HSV_S = value;
                storage.ColorState = state;
            }
        }

        public double HSV_V
        {
            get => storage.ColorState.HSV_V * 100;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.HSV_V = value / 100;
                storage.ColorState = state;
            }
        }
        public double HSL_H
        {
            get => storage.ColorState.HSL_H;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.HSL_H = value;
                storage.ColorState = state;
            }
        }

        public double HSL_S
        {
            get => storage.ColorState.HSL_S * 100;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.HSL_S = value / 100;
                storage.ColorState = state;
            }
        }

        public double HSL_L
        {
            get => storage.ColorState.HSL_L * 100;
            set
            {
                if (isUpdating) return;
                var state = storage.ColorState;
                state.HSL_L = value / 100;
                storage.ColorState = state;
            }
        }
    }
}
