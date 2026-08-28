// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
        // Works for both the "None" button (its own DataContext is the Modifier, so "as
        // IModulator" is null - exactly what SetModulator expects for unbinding) and each listed
        // modulator's button (its own DataContext is that modulator).
        //
        // Uses the base Modifier type and Channels by index, not a concrete type's X/Y/Z
        // accessors - this control is shared across every concrete Modifier (ScaleModifier,
        // PositionModifier, ...), and each only has its OWN X/Y/Z properties, not a common one.
        // Channels is the one thing every concrete Modifier actually shares.
        //
        // "Modulation.Modifier" is fully qualified on purpose: this file's own namespace
        // (CMiX.Studio.Avalonia.Views.Controls) is nested under CMiX.Studio.Avalonia.Views, which
        // also has its own ScaleModifier/PositionModifier (the views, resolved by
        // ViewModelToViewTemplate) - an enclosing-namespace type can win over a bare "using" for
        // the domain classes, so the alias avoids silently binding to the wrong one.
        private void AssignX(object sender, RoutedEventArgs e) => Assign(sender, assignButtonX, 0);
        private void AssignY(object sender, RoutedEventArgs e) => Assign(sender, assignButtonY, 1);
        private void AssignZ(object sender, RoutedEventArgs e) => Assign(sender, assignButtonZ, 2);

        private void Assign(object sender, Button assignButton, int channelIndex)
        {
            if (DataContext is Modulation.Modifier modifier)
            {
                var modulator = (sender as Control)?.DataContext as Modulation.IModulator;
                modifier.Channels[channelIndex].Binding.SetModulatorCommand.Execute(modulator);
            }
            assignButton.Flyout!.Hide();
        }
    }
}
