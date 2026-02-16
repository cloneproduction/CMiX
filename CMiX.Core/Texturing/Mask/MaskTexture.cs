// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class MaskTexture : ObservableObject, IControl, ITexture
    {
        public MaskTexture(PrefabManager prefabManager, 
                           TransformTexture transformTexture, 
                           SamplerState samplerState, 
                           GenericValue<bool> invert, 
                           GenericValue<MaskChannel> maskChannel, 
                           GenericValue<bool> isEnabled)
        {
            TextureManager = prefabManager;
            TransformTexture = transformTexture;
            SamplerState = samplerState;
            Invert = invert;
            MaskChannel = maskChannel;
            IsEnabled = isEnabled;
        }

        public IControlModel ToModel() => this.ToModel();
        public Guid ID { get; set; } = Guid.NewGuid();

        public PrefabManager TextureManager { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public SamplerState SamplerState { get; set; }
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public GenericValue<bool> Invert { get; set; }


        [ObservableProperty]
        private bool isExpanded = false;
    }
}
