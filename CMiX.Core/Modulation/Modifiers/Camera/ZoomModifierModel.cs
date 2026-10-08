// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record ZoomModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Distance { get; set; } = ModulatableValueModel<float>.Of("Distance", 0f);
        public ModulatableValueModel<float> FOV { get; set; } = ModulatableValueModel<float>.Of("FOV", 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
