// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class LFOModifier : Modifier, ISpreadableModifier
    {
        public LFOModifier(PrefabService prefabService,
                           PrefabManager modulatorManager,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<bool> pingPong,
                           DirectionXYZ directionXYZ,
                           GenericValue<TransformType> transformType,
                           ModulatableFloat from,
                           ModulatableFloat to,
                           ModulatableFloat randomizePhase)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            PingPong = pingPong;
            DirectionXYZ = directionXYZ;
            TransformType = transformType;
            from.Label = "From";
            to.Label = "To";
            randomizePhase.Label = "Randomize Phase";
            to.Value.Value = 1.0f;
            Bindables = new List<ModulatableFloat> { from, to, randomizePhase };
        }

        public ModulatableFloat From => Bindables[0];
        public ModulatableFloat To => Bindables[1];
        public ModulatableFloat RandomizePhase => Bindables[2];

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LFOModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel(),
                TransformType = (GenericValueModel<TransformType>)TransformType.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LFOModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            PingPong.FromModel(m.PingPong);
            DirectionXYZ.FromModel(m.DirectionXYZ);
            TransformType.FromModel(m.TransformType);
        }
    }
}
