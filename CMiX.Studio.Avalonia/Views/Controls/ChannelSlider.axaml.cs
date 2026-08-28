// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CMiX.Core.Prefabs.Managers;
using Modulation = CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Same shape/reasoning as ChannelValue - DataContext is the Channel itself, ModulatorManager
    // is set explicitly - except a bounded CMiXSlider instead of an unbounded DragValue, for a
    // channel whose old counterpart used a slider (e.g. RandomVisibility's 0-1 "Control").
    // Minimum/Maximum are bindable since different sliders need different ranges (unlike
    // VectorXYZ/Vector2, which are always unbounded DragValues, so didn't need this).
    public partial class ChannelSlider : CaptionedUserControl
    {
        public ChannelSlider()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<ChannelSlider, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<ChannelSlider, double>(nameof(Minimum));
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<ChannelSlider, double>(nameof(Maximum), 1.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private void Assign(object sender, RoutedEventArgs e)
        {
            if (DataContext is Modulation.Channel channel)
            {
                var modulator = (sender as Control)?.DataContext as Modulation.IModulator;
                channel.Binding.SetModulatorCommand.Execute(modulator);
            }
            assignButton.Flyout!.Hide();
        }
    }
}
