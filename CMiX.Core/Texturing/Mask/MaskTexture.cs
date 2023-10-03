// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class MaskTexture : ObservableObject, ITexture
    {
        public MaskTexture(CompositionService compositionService)
        {
            TextureManager = new PrefabManagerBase(compositionService.EntityRepository, compositionService.PrefabFactory);

            TransformTexture = new TransformTexture();
            SamplerState = new SamplerState();

            Invert = new BooleanValue();
            MaskChannel = new GenericValue<MaskChannel>();
            IsEnabled = new BooleanValue(false);
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public PrefabManagerBase TextureManager { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public SamplerState SamplerState { get; set; }

        public BooleanValue IsEnabled { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public BooleanValue Invert { get; set; }


        [ObservableProperty]
        private bool isExpanded = false;
    }
}
