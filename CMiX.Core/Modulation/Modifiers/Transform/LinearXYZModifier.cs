// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class LinearXYZModifier : Modifier, ISpreadableModifier
    {
        public LinearXYZModifier(PrefabService prefabService,
                                 PrefabManager modulatorManager,
                                 ModifierModeSelector modifierModeSelector,
                                 GenericValue<TransformType> transformTypeSelector,
                                 DirectionXYZ directionXYZ,
                                 ModulatableFloat width,
                                 ModulatableFloat phase)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            TransformTypeSelector = transformTypeSelector;
            DirectionXYZ = directionXYZ;
            width.Label = "Width";
            phase.Label = "Phase";
            Bindables = new List<ModulatableFloat> { width, phase };
        }

        public ModulatableFloat Width => Bindables[0];
        public ModulatableFloat Phase => Bindables[1];

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LinearXYZModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                TransformTypeSelector = (GenericValueModel<TransformType>)TransformTypeSelector.ToModel(),
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LinearXYZModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            TransformTypeSelector.FromModel(m.TransformTypeSelector);
            DirectionXYZ.FromModel(m.DirectionXYZ);
        }
    }
}
