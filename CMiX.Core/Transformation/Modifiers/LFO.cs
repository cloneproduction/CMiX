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
    [ModifierPanel(typeof(Entity))]
    public partial class LFO : BeatModifiableModifierBase, ISpreadableModifier
    {
        public LFO(PrefabManager beatModifierManager,
                   PrefabService prefabService,
                   GenericValue<bool> pingPong,
                   GenericValue<float> randomizePhase,
                   DirectionXYZ directionXYZ,
                   GenericValue<TransformType> transformType,
                   GenericValue<float> from,
                   GenericValue<float> to,
                   ModifierModeSelector modifierModeSelector)
            : base(prefabService, beatModifierManager)
        {
            RandomizePhase = randomizePhase;
            ModifierModeSelector = modifierModeSelector;
            PingPong = pingPong;
            DirectionXYZ = directionXYZ;
            TransformType = transformType;
            From = from;
            To = to;
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<float> RandomizePhase { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LFOModel
            {
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                RandomizePhase = (GenericValueModel<float>)RandomizePhase.ToModel(),
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                TransformType = (GenericValueModel<TransformType>)TransformType.ToModel(),
                From = (GenericValueModel<float>)From.ToModel(),
                To = (GenericValueModel<float>)To.ToModel(),
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LFOModel)model;
            LoadBaseModel(m);
            PingPong.FromModel(m.PingPong);
            RandomizePhase.FromModel(m.RandomizePhase);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            TransformType.FromModel(m.TransformType);
            From.FromModel(m.From);
            To.FromModel(m.To);
            DirectionXYZ.FromModel(m.DirectionXYZ);
        }
    }
}
