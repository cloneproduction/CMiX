// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class DirectionXYZ : ObservableRecipient, IControl
    {
        public DirectionXYZ(DirectionXYZModel directionXYZModel)
        {
            ID = directionXYZModel.ID;
            DirectionX = new BooleanValue(directionXYZModel.DirectionX);
            DirectionY = new BooleanValue(directionXYZModel.DirectionY);
            DirectionZ = new BooleanValue(directionXYZModel.DirectionZ);
        }

        public Guid ID { get; set; }
        public BooleanValue DirectionX { get; set; }
        public BooleanValue DirectionY { get; set; }
        public BooleanValue DirectionZ { get; set; }
    }
}
