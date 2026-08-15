// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : ObservableObject, IPrefab, IBeatModifiable, ITextureFilter, IDisposable
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
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            TransformType = transformType;
            ModifierModeSelector = modifierModeSelector;
            PingPong = pingPong;
            DirectionXY = directionXY;
            From = from;
            To = to;
            SamplerState = samplerState;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXY DirectionXY { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }
        public SamplerState SamplerState { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new LFOUVModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TransformType = (GenericValueModel<TransformType>)TransformType.ToModel(),
            PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
            DirectionXY = (DirectionXYModel)DirectionXY.ToModel(),
            From = (GenericValueModel<float>)From.ToModel(),
            To = (GenericValueModel<float>)To.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            Blend = (BlendModel)Blend.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (LFOUVModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            TransformType.FromModel(m.TransformType);
            PingPong.FromModel(m.PingPong);
            DirectionXY.FromModel(m.DirectionXY);
            From.FromModel(m.From);
            To.FromModel(m.To);
            Control.FromModel(m.Control);
            SamplerState.FromModel(m.SamplerState);
            Blend.FromModel(m.Blend);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.Dispose();
        }
    }
}
