// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public record DirectionXYZModel : IControlModel
    {
        public DirectionXYZModel()
        {
            DirectionX = new(true);
            DirectionY = new(false);
            DirectionZ = new(false);
        }

        public DirectionXYZModel(bool x, bool y, bool z)
        {
            DirectionX = new(x);
            DirectionY = new(y);
            DirectionZ = new(z);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> DirectionX { get; set; }
        public GenericValueModel<bool> DirectionY { get; set; }
        public GenericValueModel<bool> DirectionZ { get; set; }
    }
}
