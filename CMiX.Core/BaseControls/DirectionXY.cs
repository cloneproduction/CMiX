// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class DirectionXY : ObservableRecipient, IControl
    {
        public DirectionXY(GenericValue<bool> directionX, GenericValue<bool> directionY)
        {
            DirectionX = directionX;
            DirectionY = directionY;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> DirectionX { get; set; }
        public GenericValue<bool> DirectionY { get; set; }

        public IControlModel ToModel() => new DirectionXYModel
        {
            ID = ID,
            DirectionX = (GenericValueModel<bool>)DirectionX.ToModel(),
            DirectionY = (GenericValueModel<bool>)DirectionY.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DirectionXYModel)model;
            ID = m.ID;
            DirectionX.FromModel(m.DirectionX);
            DirectionY.FromModel(m.DirectionY);
        }
    }
}
