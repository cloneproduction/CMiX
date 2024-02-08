// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class DirectionXYZ : ObservableRecipient, IControl
    {
        public DirectionXYZ(GenericValue<bool> directionX, GenericValue<bool> directionY, GenericValue<bool> directionZ)
        {
            ID = Guid.NewGuid();
            DirectionX = directionX;
            DirectionY = directionY;
            DirectionZ = directionZ;
        }

        public Guid ID { get; set; }
        public GenericValue<bool> DirectionX { get; set; }
        public GenericValue<bool> DirectionY { get; set; }
        public GenericValue<bool> DirectionZ { get; set; }
    }
}
