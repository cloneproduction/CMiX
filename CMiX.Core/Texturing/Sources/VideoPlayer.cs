// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class VideoPlayer : TextureSourceBase, IAssetTextureSource
    {
        public VideoPlayer(PrefabService prefabService,
                           PrefabManager textureModifierManager,
                           PrefabManager modulatorManager,
                           Integer2 resolution,
                           GenericValue<bool> useCompositionResolution,
                           GenericValue<int> seekFrame,
                           ModulatableValue<bool> playing,
                           CMiXButton doSeek,
                           AssetSelector assetSelector)
            : base(prefabService, textureModifierManager, modulatorManager, useCompositionResolution)
        {
            Resolution = resolution;
            SeekFrame = seekFrame;
            Playing = playing;
            DoSeek = doSeek;
            AssetSelector = assetSelector;

            Modulators.Add(Playing);
        }

        public CMiXButton DoSeek { get; set; }
        public GenericValue<int> SeekFrame { get; set; }
        public ModulatableValue<bool> Playing { get; set; }
        public AssetSelector AssetSelector { get; set; }
        public Integer2 Resolution { get; set; }

        public override IControlModel ToModel()
        {
            var model = new VideoPlayerModel
            {
                DoSeek = (ButtonModel)DoSeek.ToModel(),
                SeekFrame = (GenericValueModel<int>)SeekFrame.ToModel(),
                Playing = (ModulatableValueModel<bool>)Playing.ToModel(),
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
            Playing.FromModel(m.Playing);
            AssetSelector.FromModel(m.AssetSelector);
            Resolution.FromModel(m.Resolution);
        }
    }
}
