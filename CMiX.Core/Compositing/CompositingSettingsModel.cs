// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public record CompositingSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<BlendMode> BlendMode { get; set; } = new(CMiX.Core.Texturing.BlendMode.Normal);
        public GenericValueModel<float> Opacity { get; set; } = new(1.0f);
        public GenericValueModel<string> BackgroundColor { get; set; } = new("#FF000000");
    }
}
