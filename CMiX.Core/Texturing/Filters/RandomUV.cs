// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class RandomUV : ObservableObject, IPrefab, IBeatModifiable, ITextureFilter
    {
        public RandomUV(PrefabService prefabService,
                        PrefabManager beatModifierManager,
                        SamplerState samplerState,
                        Vector2 location,
                        Vector2 scale,
                        GenericValue<float> rotation,
                        GenericValue<float> uniform,
                        GenericValue<float> control,
                        Blend blend)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
            SamplerState = samplerState;
            Location = location;
            Scale = scale;
            Rotation = rotation;
            Uniform = uniform;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public SamplerState SamplerState { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new RandomUVModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            Location = (Vector2Model)Location.ToModel(),
            Scale = (Vector2Model)Scale.ToModel(),
            Uniform = (GenericValueModel<float>)Uniform.ToModel(),
            Rotation = (GenericValueModel<float>)Rotation.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomUVModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Location.FromModel(m.Location);
            Scale.FromModel(m.Scale);
            Uniform.FromModel(m.Uniform);
            Rotation.FromModel(m.Rotation);
            Control.FromModel(m.Control);
            SamplerState.FromModel(m.SamplerState);
            Blend.FromModel(m.Blend);
            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
    }
}
