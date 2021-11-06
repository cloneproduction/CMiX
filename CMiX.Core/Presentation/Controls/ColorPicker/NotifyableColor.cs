// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.Controls
{
    public class NotifyableColor : ObservableObject
    {
        private readonly IColorStateStorage storage;
        public NotifyableColor(IColorStateStorage colorStateStorage)
        {
            storage = colorStateStorage;
        }

        public void UpdateEverything(ColorState oldValue)
        {
            ColorState currentValue = storage.ColorState;
            if (currentValue.A != oldValue.A) OnPropertyChanged(nameof(A));

            if (currentValue.RGB_R != oldValue.RGB_R) OnPropertyChanged(nameof(RGB_R));
            if (currentValue.RGB_G != oldValue.RGB_G) OnPropertyChanged(nameof(RGB_G));
            if (currentValue.RGB_B != oldValue.RGB_B) OnPropertyChanged(nameof(RGB_B));

            if (currentValue.HSV_H != oldValue.HSV_H) OnPropertyChanged(nameof(HSV_H));
            if (currentValue.HSV_S != oldValue.HSV_S) OnPropertyChanged(nameof(HSV_S));
            if (currentValue.HSV_V != oldValue.HSV_V) OnPropertyChanged(nameof(HSV_V));

            if (currentValue.HSL_H != oldValue.HSL_H) OnPropertyChanged(nameof(HSL_H));
            if (currentValue.HSL_S != oldValue.HSL_S) OnPropertyChanged(nameof(HSL_S));
            if (currentValue.HSL_L != oldValue.HSL_L) OnPropertyChanged(nameof(HSL_L));



        }

        public double A
        {
            get => storage.ColorState.A * 255;
            set
            {
                var state = storage.ColorState;
                state.A = value / 255;
                storage.ColorState = state;
                OnPropertyChanged();
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
                OnPropertyChanged();
                System.Console.WriteLine("NotifyableColor R" + RGB_R);

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
                OnPropertyChanged();
                System.Console.WriteLine("NotifyableColor G" + RGB_G);
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
                OnPropertyChanged();
                System.Console.WriteLine("NotifyableColor B" + RGB_B);
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
                OnPropertyChanged();
                System.Console.WriteLine("NotifyableColor H" + HSV_H);
            }
        }

        public double HSV_S
        {
            get => storage.ColorState.HSV_S * 100;
            set
            {
                var state = storage.ColorState;
                state.HSV_S = value / 100;
                storage.ColorState = state;
                OnPropertyChanged();
                System.Console.WriteLine("NotifyableColor S" + HSV_S);
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
                System.Console.WriteLine("NotifyableColor V" + HSV_V);
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }
}
