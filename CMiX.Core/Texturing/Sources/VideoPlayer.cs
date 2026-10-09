// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class VideoPlayer : TextureSourceBase, IAssetTextureSource
    {
        public VideoPlayer(PrefabService prefabService,
                           PrefabManager textureModifierManager,
                           Integer2 resolution,
                           GenericValue<bool> useCompositionResolution,
                           GenericValue<int> seekFrame,
                           GenericValue<bool> play,
                           CMiXButton doSeek,
                           AssetSelector assetSelector)
            : base(prefabService, textureModifierManager, useCompositionResolution)
        {
            Resolution = resolution;
            SeekFrame = seekFrame;
            Play = play;
            DoSeek = doSeek;
            AssetSelector = assetSelector;
        }

        public CMiXButton DoSeek { get; set; }
        public GenericValue<int> SeekFrame { get; set; }
        public GenericValue<bool> Play { get; set; }
        public AssetSelector AssetSelector { get; set; }
        public Integer2 Resolution { get; set; }

        public override IControlModel ToModel()
        {
            var model = new VideoPlayerModel
            {
                DoSeek = (ButtonModel)DoSeek.ToModel(),
                SeekFrame = (GenericValueModel<int>)SeekFrame.ToModel(),
                Play = (GenericValueModel<bool>)Play.ToModel(),
                AssetSelector = (AssetSelectorModel)AssetSelector.ToModel(),
                Resolution = (Integer2Model)Resolution.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (VideoPlayerModel)model;
            LoadBaseModel(m);
            DoSeek.FromModel(m.DoSeek);
            SeekFrame.FromModel(m.SeekFrame);
            Play.FromModel(m.Play);
            AssetSelector.FromModel(m.AssetSelector);
            Resolution.FromModel(m.Resolution);
        }
    }
}
