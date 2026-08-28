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
    public partial class ChannelVectorXYZ : CaptionedUserControl
    {
        public ChannelVectorXYZ()
        {
            InitializeComponent();
        }

        // Set explicitly by the consumer rather than read off DataContext - a Modifier that owns
        // more than one XYZ group (e.g. Grid's separate Width and Phase) needs each group's own
        // DataContext for its own X/Y/Z, but all groups share the one Modifier-level modulator
        // stack, so "which channels" and "which stack" can no longer be assumed to be the same
        // object. For a single-group Modifier (Scale, Position, Rotation), this is just
        // {Binding ModulatorManager} same as before.
        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<ChannelVectorXYZ, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
        }

        // A single Click handler both assigns and closes the flyout, rather than a Command
        // binding plus a separate Click handler - those raced, since hiding the flyout inside
        // the Click handler could tear the button down before Avalonia got to invoking its
        // Command. Matches ServerSettings.axaml.cs's own EditConnectionButton/SyncButton pattern,
        // which does its work directly in the Click handler for the same reason.
        //
        // Works for both the "None" button (its own DataContext is the channel group, so "as
        // IModulator" is null - exactly what SetModulator expects for unbinding) and each listed
        // modulator's button (its own DataContext is that modulator).
        //
        // DataContext is IChannelGroup, not a concrete Modifier - this control is shared across
        // every concrete Modifier (ScaleModifier, PositionModifier, ...) and also across multiple
        // groups within one Modifier (Grid's Width/Phase), so it only assumes X/Y/Z exist, not
        // that DataContext is the whole Modifier.
        //
        // "Modulation.*" is fully qualified on purpose: this file's own namespace
        // (CMiX.Studio.Avalonia.Views.Controls) is nested under CMiX.Studio.Avalonia.Views, which
        // also has its own ScaleModifier/PositionModifier (the views, resolved by
        // ViewModelToViewTemplate) - an enclosing-namespace type can win over a bare "using" for
        // the domain classes, so the alias avoids silently binding to the wrong one.
        private void AssignX(object sender, RoutedEventArgs e) => Assign(sender, assignButtonX, g => g.X);
        private void AssignY(object sender, RoutedEventArgs e) => Assign(sender, assignButtonY, g => g.Y);
        private void AssignZ(object sender, RoutedEventArgs e) => Assign(sender, assignButtonZ, g => g.Z);

        private void Assign(object sender, Button assignButton, Func<Modulation.IChannelGroup, Modulation.Channel> channel)
        {
            if (DataContext is Modulation.IChannelGroup group)
            {
                var modulator = (sender as Control)?.DataContext as Modulation.IModulator;
                channel(group).Binding.SetModulatorCommand.Execute(modulator);
            }
            assignButton.Flyout!.Hide();
        }
    }
}
