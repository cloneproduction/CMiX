// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ModulatableDragValue : ModulatorAssignableUserControl
    {
        public ModulatableDragValue()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<ModulatableDragValue, double>(nameof(Minimum), -10000.0);
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<ModulatableDragValue, double>(nameof(Maximum), 10000.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
    }
}
