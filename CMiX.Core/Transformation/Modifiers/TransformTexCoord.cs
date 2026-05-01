// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class TransformTexCoord : ObservableObject, IModifier
    {
        public TransformTexCoord(PrefabService prefabService,
                           SamplerState samplerState,
                           Vector2 location,
                           Vector2 scale,
                           GenericValue<float> rotation,
                           GenericValue<float> uniform)
        {
            PrefabService = prefabService;
            SamplerState = samplerState;
            Location = location;
            Scale = scale;
            Rotation = rotation;
            Uniform = uniform;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new TransformTexCoordModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Location = (Vector2Model)Location.ToModel(),
            Scale = (Vector2Model)Scale.ToModel(),
            Uniform = (GenericValueModel<float>)Uniform.ToModel(),
            Rotation = (GenericValueModel<float>)Rotation.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TransformTexCoordModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Location.FromModel(m.Location);
            Scale.FromModel(m.Scale);
            Uniform.FromModel(m.Uniform);
            Rotation.FromModel(m.Rotation);
            SamplerState.FromModel(m.SamplerState);
        }
    }
}
