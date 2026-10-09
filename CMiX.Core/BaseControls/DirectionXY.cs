// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
