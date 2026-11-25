// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    // One Redis address of the list. A Redis modulator selects one of them.
    public class RedisAddress : GenericValue<string>
    {
        public RedisAddress(IValueSync sync, ControlRegistry registry)
            : base(sync, registry)
        {
        }

        public override IControlModel ToModel() => new RedisAddressModel
        {
            ID = ID,
            Value = Value
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisAddressModel)model;
            ID = m.ID;
            Value = m.Value;
        }
    }
}
