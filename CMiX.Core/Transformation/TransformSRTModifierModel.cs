// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation
{
    public record TransformSRTModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> TranslateX { get; set; } = ModulatableValueModel<float>.Of("X", 0f);
        public ModulatableValueModel<float> TranslateY { get; set; } = ModulatableValueModel<float>.Of("Y", 0f);
        public ModulatableValueModel<float> TranslateZ { get; set; } = ModulatableValueModel<float>.Of("Z", 0f);
        public ModulatableValueModel<float> ScaleX { get; set; } = ModulatableValueModel<float>.Of("X", 1f);
        public ModulatableValueModel<float> ScaleY { get; set; } = ModulatableValueModel<float>.Of("Y", 1f);
        public ModulatableValueModel<float> ScaleZ { get; set; } = ModulatableValueModel<float>.Of("Z", 1f);
        public ModulatableValueModel<float> ScaleUniform { get; set; } = ModulatableValueModel<float>.Of("Uniform", 1f);
        public ModulatableValueModel<float> RotationX { get; set; } = ModulatableValueModel<float>.Of("X", 0f);
        public ModulatableValueModel<float> RotationY { get; set; } = ModulatableValueModel<float>.Of("Y", 0f);
        public ModulatableValueModel<float> RotationZ { get; set; } = ModulatableValueModel<float>.Of("Z", 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new(ModifierMode.PerInstance, 1);
        public DirectionXYZModel DirectionXYZ { get; set; } = new(false, false, false);
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.ToSpread);
    }
}
