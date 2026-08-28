// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CMiX.Core.Prefabs.Managers;
using Modulation = CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Single-channel counterpart to ChannelVectorXYZ/ChannelVectorXY - same row shape, just one.
    // DataContext is the Channel itself (not the owning Modifier), so a Modifier with more than
    // one standalone channel (e.g. CircularSpread's separate Phase and Factor) can point several
    // ChannelValue instances at different channels while all sharing the same ModulatorManager -
    // see ChannelVectorXYZ.axaml.cs for the same reasoning applied to a 3-channel group.
    public partial class ChannelValue : CaptionedUserControl
    {
        public ChannelValue()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<ChannelValue, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
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
