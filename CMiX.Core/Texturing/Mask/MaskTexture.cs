// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing
{
    public partial class MaskTexture : ObservableObject, IControl, ITexture
    {
        public MaskTexture(PrefabManager prefabManager,
                           TextureTexCoord textureTexCoord,
                           SamplerState samplerState,
                           GenericValue<bool> invert,
                           GenericValue<MaskChannel> maskChannel,
                           GenericValue<bool> isEnabled)
        {
            TextureManager = prefabManager;
            TextureTexCoord = textureTexCoord;
            SamplerState = samplerState;
            Invert = invert;
            MaskChannel = maskChannel;
            IsEnabled = isEnabled;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public PrefabManager TextureManager { get; set; }
        public TextureTexCoord TextureTexCoord { get; set; }
        public SamplerState SamplerState { get; set; }
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public GenericValue<bool> Invert { get; set; }


        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new MaskTextureModel
        {
            ID = ID,
            TextureManager = (PrefabManagerModel)TextureManager.ToModel(),
            TextureTexCoord = (TextureTexCoordModel)TextureTexCoord.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel(),
            IsEnabled = (GenericValueModel<bool>)IsEnabled.ToModel(),
            MaskChannel = (GenericValueModel<MaskChannel>)MaskChannel.ToModel(),
            Invert = (GenericValueModel<bool>)Invert.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MaskTextureModel)model;
            ID = m.ID;
            TextureTexCoord.FromModel(m.TextureTexCoord);
            SamplerState.FromModel(m.SamplerState);
            IsEnabled.FromModel(m.IsEnabled);
            MaskChannel.FromModel(m.MaskChannel);
            Invert.FromModel(m.Invert);

            LoadManager(TextureManager, m.TextureManager);
        }
    }
}
