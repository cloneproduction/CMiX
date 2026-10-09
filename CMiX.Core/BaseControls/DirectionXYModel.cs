// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public record DirectionXYModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> DirectionX { get; set; } = new(true);
        public GenericValueModel<bool> DirectionY { get; set; } = new(false);
    }
}
