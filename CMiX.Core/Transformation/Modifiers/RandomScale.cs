// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.ScaleModifier. No longer addable via the
    // picker; kept so already-saved Project data referencing RandomScale still loads.
    public partial class RandomScale : BeatModifiableModifierBase, ISpreadableModifier
    {
        public RandomScale(PrefabManager beatModifierManager,
                           PrefabService prefabService,
                           ModifierModeSelector modifierModeSelector,
                           Vector3 scale,
                           GenericValue<float> uniformXYZ)
            : base(prefabService, beatModifierManager)
        {
            ModifierModeSelector = modifierModeSelector;
            Scale = scale;
            UniformXYZ = uniformXYZ;
        }

        public Vector3 Scale { get; set; }
        public GenericValue<float> UniformXYZ { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomScaleModel
            {
                Scale = (Vector3Model)Scale.ToModel(),
                UniformXYZ = (GenericValueModel<float>)UniformXYZ.ToModel(),
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomScaleModel)model;
            LoadBaseModel(m);
            Scale.FromModel(m.Scale);
            UniformXYZ.FromModel(m.UniformXYZ);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
