// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing
{
    public partial class DiffuseTexture : ObservableObject, IControl, ITexture, IHasCompositionID
    {
        public DiffuseTexture(PrefabManager prefabManager,
                              Transform2D transform2D,
                              SamplerState samplerState)
        {
            TextureManager = prefabManager;
            Transform2D = transform2D;
            SamplerState = samplerState;
        }

        public Guid ID { get ; set; } = Guid.NewGuid();
        public PrefabManager TextureManager { get; set; }
        public Transform2D Transform2D { get; set; }
        public SamplerState SamplerState { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                TextureManager.CompositionID = value;
            }
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new DiffuseTextureModel
        {
            ID = ID,
            TextureManager = (PrefabManagerModel)TextureManager.ToModel(),
            Transform2D = (Transform2DModel)Transform2D.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DiffuseTextureModel)model;
            ID = m.ID;
            Transform2D.FromModel(m.Transform2D);
            SamplerState.FromModel(m.SamplerState);

            LoadManager(TextureManager, m.TextureManager);
        }
    }
}
