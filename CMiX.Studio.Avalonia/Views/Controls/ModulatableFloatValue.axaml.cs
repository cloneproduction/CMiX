// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ModulatableFloatValue : ModulatorAssignableUserControl
    {
        public ModulatableFloatValue()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<ModulatableFloatValue, double>(nameof(Minimum), -10000.0);
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<ModulatableFloatValue, double>(nameof(Maximum), 10000.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
    }
}
