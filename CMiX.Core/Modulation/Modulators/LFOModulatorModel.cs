// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public record LFOModulatorModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Period { get; set; } = new(10.0f);
        public GenericValueModel<float> Minimum { get; set; } = new(-1.0f);
        public GenericValueModel<float> Maximum { get; set; } = new(1.0f);
        public GenericValueModel<WaveTypeEnum> WaveType { get; set; } = new(WaveTypeEnum.Triangle);
    }
}
