// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
