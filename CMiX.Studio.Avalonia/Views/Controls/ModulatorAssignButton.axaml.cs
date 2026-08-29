// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ModulatorAssignButton : ModulatorAssignableUserControl
    {
        public ModulatorAssignButton()
        {
            InitializeComponent();
        }

        private void Assign(object sender, RoutedEventArgs e)
        {
            AssignFromDataContext(sender);
            assignButton.Flyout!.Hide();
        }
    }
}
