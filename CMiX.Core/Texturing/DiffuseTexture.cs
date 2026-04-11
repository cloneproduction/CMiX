// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing
{
    public partial class DiffuseTexture : ObservableObject, IControl, ITexture
    {
        public DiffuseTexture(PrefabManager prefabManager, 
                              TransformTexture transformTexture, 
                              SamplerState samplerState)
        {
            TextureManager = prefabManager;
            TransformTexture = transformTexture;
            SamplerState = samplerState;
        }

        public Guid ID { get ; set; } = Guid.NewGuid();
        public PrefabManager TextureManager { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new DiffuseTextureModel
        {
            ID = ID,
            TextureManager = (PrefabManagerModel)TextureManager.ToModel(),
            TransformTexture = (TransformTextureModel)TransformTexture.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DiffuseTextureModel)model;
            ID = m.ID;
            TransformTexture.FromModel(m.TransformTexture);
            SamplerState.FromModel(m.SamplerState);

            LoadManager(TextureManager, m.TextureManager);
        }
    }
}
