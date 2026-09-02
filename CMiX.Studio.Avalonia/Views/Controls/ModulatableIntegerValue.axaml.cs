// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Int-valued counterpart to ModulatableFloatValue, for a ModulatableInteger DataContext (e.g.
    // ModifierModeSelector's Count). Same row shape, just IsInteger with a Minimum of 1 - a Count
    // of 0 or negative doesn't mean anything, matching the bare DragValue this replaces.
    public partial class ModulatableIntegerValue : ModulatorAssignableUserControl
    {
        public ModulatableIntegerValue()
        {
            InitializeComponent();
        }
    }
}
