// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Single-channel counterpart to ChannelVectorXYZ/ChannelVectorXY - same row shape, just one.
    // DataContext is the Channel itself (not the owning Modifier), so a Modifier with more than
    // one standalone channel (e.g. CircularSpread's separate Phase and Factor) can point several
    // ChannelValue instances at different channels while all sharing the same ModulatorManager.
    // ChannelVectorXY/ChannelVectorXYZ are themselves built out of one ChannelValue per axis - see
    // those files. The assign button itself is ModulatorAssignButton, not defined here.
    public partial class ChannelValue : ModulatorAssignableUserControl
    {
        public ChannelValue()
        {
            InitializeComponent();
        }
    }
}
