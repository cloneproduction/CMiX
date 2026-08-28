// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CMiX.Core.Prefabs.Managers;
using Modulation = CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ChannelVectorXY : CaptionedUserControl
    {
        public ChannelVectorXY()
        {
            InitializeComponent();
        }

        // See ChannelVectorXYZ.axaml.cs for why this is set explicitly rather than read off
        // DataContext.
        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<ChannelVectorXY, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
        }

        private void AssignX(object sender, RoutedEventArgs e) => Assign(sender, assignButtonX, g => g.X);
        private void AssignY(object sender, RoutedEventArgs e) => Assign(sender, assignButtonY, g => g.Y);

        private void Assign(object sender, Button assignButton, Func<Modulation.IChannelGroupXY, Modulation.Channel> channel)
        {
            if (DataContext is Modulation.IChannelGroupXY group)
            {
                var modulator = (sender as Control)?.DataContext as Modulation.IModulator;
                channel(group).Binding.SetModulatorCommand.Execute(modulator);
            }
            assignButton.Flyout!.Hide();
        }
    }
}
