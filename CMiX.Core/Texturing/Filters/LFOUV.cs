// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : TextureFilterBase, IBeatModifiable, IDisposable
    {
        public LFOUV(PrefabManager beatModifierManager,
                     PrefabService prefabService,
                     GenericValue<TransformType> transformType,
                     ModifierModeSelector modifierModeSelector,
                     GenericValue<bool> pingPong,
                     DirectionXY directionXY,
                     GenericValue<float> from,
                     GenericValue<float> to,
                     SamplerState samplerState,
                     GenericValue<float> control,
                     Blend blend)
            : base(prefabService, control, blend)
        {
            BeatModifierManager = beatModifierManager;
            TransformType = transformType;
            ModifierModeSelector = modifierModeSelector;
            PingPong = pingPong;
            DirectionXY = directionXY;
            From = from;
            To = to;
            SamplerState = samplerState;
        }

        public GenericValue<TransformType> TransformType { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXY DirectionXY { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }
        public SamplerState SamplerState { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LFOUVModel
            {
                TransformType = (GenericValueModel<TransformType>)TransformType.ToModel(),
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                DirectionXY = (DirectionXYModel)DirectionXY.ToModel(),
                From = (GenericValueModel<float>)From.ToModel(),
                To = (GenericValueModel<float>)To.ToModel(),
                SamplerState = (SamplerStateModel)SamplerState.ToModel(),
                BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LFOUVModel)model;
            LoadBaseModel(m);
            this.LoadBeatModifier(m.BeatModifierManager);
            TransformType.FromModel(m.TransformType);
            PingPong.FromModel(m.PingPong);
            DirectionXY.FromModel(m.DirectionXY);
            From.FromModel(m.From);
            To.FromModel(m.To);
            SamplerState.FromModel(m.SamplerState);
        }

        public void Dispose() => this.DisposeBeatModifier();
    }
}
