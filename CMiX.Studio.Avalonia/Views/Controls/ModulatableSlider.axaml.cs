// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Same shape/reasoning as ModulatableValue - DataContext is the Modulatable itself, ModulatorManager
    // is inherited - except a bounded CMiXSlider instead of an unbounded DragValue, for a channel
    // whose old counterpart used a slider (e.g. RandomVisibility's 0-1 "Control"). Minimum/Maximum
    // are bindable since different sliders need different ranges (unlike VectorXYZ/Vector2, which
    // are always unbounded DragValues, so didn't need this). The assign button itself is
    // ModulatorAssignButton, not defined here.
    public partial class ModulatableSlider : ModulatorAssignableUserControl
    {
        public ModulatableSlider()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<ModulatableSlider, double>(nameof(Minimum));
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<ModulatableSlider, double>(nameof(Maximum), 1.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
    }
}
