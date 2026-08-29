// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Single-channel counterpart to ModulatableVectorXYZ/ModulatableVectorXY - same row shape, just one.
    // DataContext is the Modulatable itself (not the owning Modifier), so a Modifier with more than
    // one standalone channel (e.g. CircularSpreadModifier's separate Phase and Factor) can point several
    // ModulatableValue instances at different channels while all sharing the same ModulatorManager.
    // ModulatableVectorXY/ModulatableVectorXYZ are themselves built out of one ModulatableValue per axis - see
    // those files. The assign button itself is ModulatorAssignButton, not defined here.
    public partial class ModulatableValue : ModulatorAssignableUserControl
    {
        public ModulatableValue()
        {
            InitializeComponent();
        }
    }
}
