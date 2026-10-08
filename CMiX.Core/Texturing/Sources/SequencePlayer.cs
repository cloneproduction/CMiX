// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class SequencePlayer : TextureSourceBase, IAssetTextureSource
    {
        public SequencePlayer(PrefabService prefabService,
                              PrefabManager textureModifierManager,
                              Integer2 resolution,
                              GenericValue<bool> useCompositionResolution,
                              GenericValue<int> seekFrame,
                              GenericValue<bool> play,
                              GenericValue<float> fps,
                              GenericValue<bool> loop,
                              CMiXButton doSeek,
                              AssetSelector assetSelector)
            : base(prefabService, textureModifierManager, useCompositionResolution)
        {
            Resolution = resolution;
            SeekFrame = seekFrame;
            Play = play;
            FPS = fps;
            Loop = loop;
            DoSeek = doSeek;
            AssetSelector = assetSelector;
        }

        public CMiXButton DoSeek { get; set; }
        public GenericValue<int> SeekFrame { get; set; }
        public GenericValue<bool> Play { get; set; }
        public GenericValue<float> FPS { get; set; }
        public GenericValue<bool> Loop { get; set; }
        public AssetSelector AssetSelector { get; set; }
        public Integer2 Resolution { get; set; }

        public override IControlModel ToModel()
        {
            var model = new SequencePlayerModel
            {
                DoSeek = (ButtonModel)DoSeek.ToModel(),
                SeekFrame = (GenericValueModel<int>)SeekFrame.ToModel(),
                Play = (GenericValueModel<bool>)Play.ToModel(),
                FPS = (GenericValueModel<float>)FPS.ToModel(),
                Loop = (GenericValueModel<bool>)Loop.ToModel(),
                AssetSelector = (AssetSelectorModel)AssetSelector.ToModel(),
                Resolution = (Integer2Model)Resolution.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (SequencePlayerModel)model;
            LoadBaseModel(m);
            DoSeek.FromModel(m.DoSeek);
            SeekFrame.FromModel(m.SeekFrame);
            Play.FromModel(m.Play);
            FPS.FromModel(m.FPS);
            Loop.FromModel(m.Loop);
            AssetSelector.FromModel(m.AssetSelector);
            Resolution.FromModel(m.Resolution);
        }
    }
}
