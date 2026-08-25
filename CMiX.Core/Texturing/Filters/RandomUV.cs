// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class RandomUV : TextureFilterBase, IBeatModifiable, IDisposable
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
            : base(prefabService, control, blend)
        {
            BeatModifierManager = beatModifierManager;
            SamplerState = samplerState;
            Location = location;
            Scale = scale;
            Rotation = rotation;
            Uniform = uniform;
        }

        public PrefabManager BeatModifierManager { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public SamplerState SamplerState { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomUVModel
            {
                BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
                Location = (Vector2Model)Location.ToModel(),
                Scale = (Vector2Model)Scale.ToModel(),
                Uniform = (GenericValueModel<float>)Uniform.ToModel(),
                Rotation = (GenericValueModel<float>)Rotation.ToModel(),
                SamplerState = (SamplerStateModel)SamplerState.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomUVModel)model;
            LoadBaseModel(m);
            this.LoadBeatModifier(m.BeatModifierManager);
            Location.FromModel(m.Location);
            Scale.FromModel(m.Scale);
            Uniform.FromModel(m.Uniform);
            Rotation.FromModel(m.Rotation);
            SamplerState.FromModel(m.SamplerState);
        }

        public void Dispose() => this.DisposeBeatModifier();
    }
}
