// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public record VisitorDataModulatorModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<int> Count { get; set; } = new(0);
        public GenericValueModel<float> HeartRate { get; set; } = new(0);
        public GenericValueModel<float> Temperature { get; set; } = new(0f);
        public GenericValueModel<float> Conductivity { get; set; } = new(0f);
        public GenericValueModel<float> ETemp { get; set; } = new(0f);
        public GenericValueModel<float> BlueIOTPosX { get; set; } = new(0f);
        public GenericValueModel<float> BlueIOTPosY { get; set; } = new(0f);
    }
}
