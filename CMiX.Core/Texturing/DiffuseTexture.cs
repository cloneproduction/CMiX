// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing
{
    public partial class DiffuseTexture : ObservableObject, IControl, ITexture
    {
        public DiffuseTexture(PrefabManager prefabManager,
                              TextureTexCoord textureTexCoord,
                              SamplerState samplerState)
        {
            TextureManager = prefabManager;
            TextureTexCoord = textureTexCoord;
            SamplerState = samplerState;
        }

        public Guid ID { get ; set; } = Guid.NewGuid();
        public PrefabManager TextureManager { get; set; }
        public TextureTexCoord TextureTexCoord { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new DiffuseTextureModel
        {
            ID = ID,
            TextureManager = (PrefabManagerModel)TextureManager.ToModel(),
            TextureTexCoord = (TextureTexCoordModel)TextureTexCoord.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DiffuseTextureModel)model;
            ID = m.ID;
            TextureTexCoord.FromModel(m.TextureTexCoord);
            SamplerState.FromModel(m.SamplerState);

            LoadManager(TextureManager, m.TextureManager);
        }
    }
}
