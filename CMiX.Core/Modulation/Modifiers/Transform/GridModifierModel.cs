// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record GridModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableVector3Model Width { get; set; } = ModulatableVector3Model.Of(0f, 0f, 0f);
        public ModulatableVector3Model Phase { get; set; } = ModulatableVector3Model.Of(0f, 0f, 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModulatableValueModel<int> CountX { get; set; } = new() { Value = new GenericValueModel<int>(1) };
        public ModulatableValueModel<int> CountY { get; set; } = new() { Value = new GenericValueModel<int>(1) };
        public ModulatableValueModel<int> CountZ { get; set; } = new() { Value = new GenericValueModel<int>(1) };
    }
}
