// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ModulatableFloatSlider : ModulatorAssignableUserControl
    {
        public ModulatableFloatSlider()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<ModulatableFloatSlider, double>(nameof(Minimum));
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<ModulatableFloatSlider, double>(nameof(Maximum), 1.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
    }
}
