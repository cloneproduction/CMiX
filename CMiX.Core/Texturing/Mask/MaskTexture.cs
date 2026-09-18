// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing
{
    public partial class MaskTexture : ObservableObject, IControl, ITexture, IHasCompositionID
    {
        public MaskTexture(PrefabManager prefabManager,
                           Transform2D transform2D,
                           SamplerState samplerState,
                           GenericValue<bool> invert,
                           GenericValue<MaskChannel> maskChannel,
                           GenericValue<bool> isEnabled)
        {
            TextureManager = prefabManager;
            Transform2D = transform2D;
            SamplerState = samplerState;
            Invert = invert;
            MaskChannel = maskChannel;
            IsEnabled = isEnabled;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public PrefabManager TextureManager { get; set; }
        public Transform2D Transform2D { get; set; }
        public SamplerState SamplerState { get; set; }
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public GenericValue<bool> Invert { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                new CompositionIDAssigner(TextureManager, value);
            }
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new MaskTextureModel
        {
            ID = ID,
            TextureManager = (PrefabManagerModel)TextureManager.ToModel(),
            Transform2D = (Transform2DModel)Transform2D.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel(),
            IsEnabled = (GenericValueModel<bool>)IsEnabled.ToModel(),
            MaskChannel = (GenericValueModel<MaskChannel>)MaskChannel.ToModel(),
            Invert = (GenericValueModel<bool>)Invert.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MaskTextureModel)model;
            ID = m.ID;
            Transform2D.FromModel(m.Transform2D);
            SamplerState.FromModel(m.SamplerState);
            IsEnabled.FromModel(m.IsEnabled);
            MaskChannel.FromModel(m.MaskChannel);
            Invert.FromModel(m.Invert);

            LoadManager(TextureManager, m.TextureManager);
        }
    }
}
