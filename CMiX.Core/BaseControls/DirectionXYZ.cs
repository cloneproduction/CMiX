// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class DirectionXYZ : ObservableRecipient, IControl
    {
        public DirectionXYZ()
        {
            DirectionX = new BooleanValue();
            DirectionY = new BooleanValue();
            DirectionZ = new BooleanValue();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue DirectionX { get; set; }
        public BooleanValue DirectionY { get; set; }
        public BooleanValue DirectionZ { get; set; }
    }
}
