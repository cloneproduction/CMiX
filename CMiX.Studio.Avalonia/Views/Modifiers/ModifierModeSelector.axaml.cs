// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Base class change from plain UserControl to ModulatorAssignableUserControl gains the
    // ModulatorManager pass-through property for free, mirroring how ModulatableVectorXYZ reuses
    // the same base purely for that plumbing (its own DataContext isn't IModulatorBindable
    // either) - lets Count's ModulatableIntegerValue reach the owning Modifier's ModulatorManager
    // even though this view's own DataContext switches to the ModifierModeSelector VM.
    public partial class ModifierModeSelector : ModulatorAssignableUserControl
    {
        public ModifierModeSelector()
        {
            InitializeComponent();
        }
    }
}
