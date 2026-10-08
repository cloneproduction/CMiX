// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record OrbitModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Yaw { get; set; } = ModulatableValueModel<float>.Of("Yaw", 0f);
        public ModulatableValueModel<float> Pitch { get; set; } = ModulatableValueModel<float>.Of("Pitch", 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
