// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Modulation = CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ChannelVectorXYZ : UserControl
    {
        public ChannelVectorXYZ()
        {
            InitializeComponent();
        }

        // A single Click handler both assigns and closes the flyout, rather than a Command
        // binding plus a separate Click handler - those raced, since hiding the flyout inside
        // the Click handler could tear the button down before Avalonia got to invoking its
        // Command. Matches ServerSettings.axaml.cs's own EditConnectionButton/SyncButton pattern,
        // which does its work directly in the Click handler for the same reason.
        //
        // Works for both the "None" button (its own DataContext is the ScaleModifier, so "as
        // IModulator" is null - exactly what SetModulator expects for unbinding) and each listed
        // modulator's button (its own DataContext is that modulator).
        //
        // "Modulation.ScaleModifier" is fully qualified on purpose: this file's own namespace
        // (CMiX.Studio.Avalonia.Views.Controls) is nested under CMiX.Studio.Avalonia.Views, which
        // also has its own ScaleModifier (the view, resolved by ViewModelToViewTemplate) - that
        // enclosing-namespace type wins over a bare "using" for the domain class, so the alias
        // avoids silently binding to the wrong one.
        private void AssignX(object sender, RoutedEventArgs e) => Assign(sender, assignButtonX, scale => scale.X);
        private void AssignY(object sender, RoutedEventArgs e) => Assign(sender, assignButtonY, scale => scale.Y);
        private void AssignZ(object sender, RoutedEventArgs e) => Assign(sender, assignButtonZ, scale => scale.Z);

        private void Assign(object sender, Button assignButton, Func<Modulation.ScaleModifier, Modulation.Channel> channel)
        {
            if (DataContext is Modulation.ScaleModifier scale)
            {
                var modulator = (sender as Control)?.DataContext as Modulation.IModulator;
                channel(scale).Binding.SetModulatorCommand.Execute(modulator);
            }
            assignButton.Flyout!.Hide();
        }
    }
}
