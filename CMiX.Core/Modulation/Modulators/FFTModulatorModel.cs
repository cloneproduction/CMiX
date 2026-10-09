// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public record FFTModulatorModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> FFT { get; set; } = new(0.0f);
        public GenericValueModel<float> Bass { get; set; } = new(0.0f);
        public GenericValueModel<float> LowerMid { get; set; } = new(0.0f);
        public GenericValueModel<float> HigherMid { get; set; } = new(0.0f);
        public GenericValueModel<float> High { get; set; } = new(0.0f);
    }
}
