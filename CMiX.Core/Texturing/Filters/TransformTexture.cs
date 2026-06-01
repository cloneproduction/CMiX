// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : ObservableObject, IPrefab, ITextureFilter
    {
        public TransformTexture(PrefabService prefabService, 
                                SamplerState samplerState, 
                                Transform2D transform2D,
                                GenericValue<float> control,
                                Blend blend)
        {
            PrefabService = prefabService;
            SamplerState = samplerState;
            Transform2D = transform2D;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public SamplerState SamplerState { get; set; }
        public Transform2D Transform2D { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new TransformTextureModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel(),
            Transform2D = (Transform2DModel)Transform2D.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TransformTextureModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SamplerState.FromModel(m.SamplerState);
            Transform2D.FromModel(m.Transform2D);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
