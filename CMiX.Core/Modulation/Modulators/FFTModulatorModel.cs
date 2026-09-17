// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
