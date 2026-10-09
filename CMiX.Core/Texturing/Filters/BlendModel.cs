// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Filters
{
    public record BlendModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsEnabled { get; set; } = new(false);
        public GenericValueModel<BlendModeEnum> BlendMode { get; set; } = new(BlendModeEnum.Normal);
        public GenericValueModel<float> Opacity { get; set; } = new(1.0f);
    }
}
