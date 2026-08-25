// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class RandomRotation : BeatModifiableModifierBase, ISpreadableModifier
    {
        public RandomRotation(PrefabManager beatModifierManager,
                              PrefabService prefabService,
                              ModifierModeSelector modifierModeSelector,
                              Vector3 rotation)
            : base(prefabService, beatModifierManager)
        {
            ModifierModeSelector = modifierModeSelector;
            Rotation = rotation;
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector3 Rotation { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomRotationModel
            {
                Rotation = (Vector3Model)Rotation.ToModel(),
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomRotationModel)model;
            LoadBaseModel(m);
            Rotation.FromModel(m.Rotation);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
