// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class DirectionXYZ : ObservableRecipient, IControl
    {
        public DirectionXYZ(GenericValue<bool> directionX, 
                            GenericValue<bool> directionY, 
                            GenericValue<bool> directionZ)
        {
            DirectionX = directionX;
            DirectionY = directionY;
            DirectionZ = directionZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> DirectionX { get; set; }
        public GenericValue<bool> DirectionY { get; set; }
        public GenericValue<bool> DirectionZ { get; set; }

        public IControlModel ToModel() => new DirectionXYZModel
        {
            ID = ID,
            DirectionX = (GenericValueModel<bool>)DirectionX.ToModel(),
            DirectionY = (GenericValueModel<bool>)DirectionY.ToModel(),
            DirectionZ = (GenericValueModel<bool>)DirectionZ.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DirectionXYZModel)model;
            ID = m.ID;
            DirectionX.FromModel(m.DirectionX);
            DirectionY.FromModel(m.DirectionY);
            DirectionZ.FromModel(m.DirectionZ);
        }
    }
}
