// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class SequencePlayer : TextureSourceBase, IAssetTextureSource
    {
        public SequencePlayer(PrefabService prefabService,
                              PrefabManager textureModifierManager,
                              PrefabManager modulatorManager,
                              Integer2 resolution,
                              GenericValue<bool> useCompositionResolution,
                              ModulatableValue<int> seekTarget,
                              ModulatableValue<bool> playing,
                              GenericValue<float> fps,
                              ModulatableValue<bool> looping,
                              CMiXButton doSeek,
                              AssetSelector assetSelector)
            : base(prefabService, textureModifierManager, modulatorManager, useCompositionResolution)
        {
            Resolution = resolution;
            SeekTarget = seekTarget;
            Playing = playing;
            FPS = fps;
            Looping = looping;
            DoSeek = doSeek;
            AssetSelector = assetSelector;

            Modulators.Add(Playing, Looping, SeekTarget);
        }

        public CMiXButton DoSeek { get; set; }
        public ModulatableValue<int> SeekTarget { get; set; }
        public ModulatableValue<bool> Playing { get; set; }
        public GenericValue<float> FPS { get; set; }
        public ModulatableValue<bool> Looping { get; set; }
        public AssetSelector AssetSelector { get; set; }
        public Integer2 Resolution { get; set; }

        public override IControlModel ToModel()
        {
            var model = new SequencePlayerModel
            {
                DoSeek = (ButtonModel)DoSeek.ToModel(),
                SeekTarget = (ModulatableValueModel<int>)SeekTarget.ToModel(),
                Playing = (ModulatableValueModel<bool>)Playing.ToModel(),
                FPS = (GenericValueModel<float>)FPS.ToModel(),
                Looping = (ModulatableValueModel<bool>)Looping.ToModel(),
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
            SeekTarget.FromModel(m.SeekTarget);
            Playing.FromModel(m.Playing);
            FPS.FromModel(m.FPS);
            Looping.FromModel(m.Looping);
            AssetSelector.FromModel(m.AssetSelector);
            Resolution.FromModel(m.Resolution);
        }
    }
}
