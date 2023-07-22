// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class DiffuseTextureModel : IControlModel
    {
        public DiffuseTextureModel()
        {
            TextureManager = new PrefabManagerModel();
            TransformTexture = new TransformTextureModel();
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get ; set; } = Guid.NewGuid();
        public PrefabManagerModel TextureManager { get; set; }
        public TransformTextureModel TransformTexture { get; set; }
        public SamplerStateModel SamplerState { get; set; }
    }
}
