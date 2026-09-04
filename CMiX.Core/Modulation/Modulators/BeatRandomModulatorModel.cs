// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public record BeatRandomModulatorModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public EasingModel Easing { get; set; } = new();
        public GenericValueModel<int> BeatIndex { get; set; } = new(0);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BeatStepsModel BeatSteps { get; set; } = new();

        public GenericValueModel<float> Center { get; set; } = new(0.0f);
        public GenericValueModel<float> Width { get; set; } = new(1.0f);
    }
}
