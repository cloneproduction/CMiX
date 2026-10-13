// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Modulation
{
    public partial class SliderModulatable : UserControlModulatable
    {
        public SliderModulatable()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<SliderModulatable, double>(nameof(Minimum));
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<SliderModulatable, double>(nameof(Maximum), 1.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
    }
}
