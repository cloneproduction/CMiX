// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class DiffuseTexture : ObservableObject, ITexture
    {
        public DiffuseTexture(CompositionService compositionService)
        {
            TextureManager = compositionService.GetPrefabManager<ITexture>();
            TransformTexture = new TransformTexture();
            SamplerState = new SamplerState();
        }

        public Guid ID { get ; set; } = Guid.NewGuid();
        public PrefabManagerBase TextureManager { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
