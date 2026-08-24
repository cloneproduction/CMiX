// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class VideoPlayer : ObservableObject, IAssetTextureSource, IDisposable
    {
        public VideoPlayer(PrefabService prefabService,
                           PrefabManager textureModifierManager,
                           Integer2 resolution, 
                           GenericValue<int> seekFrame, 
                           GenericValue<bool> play, 
                           CMiXButton doSeek, 
                           AssetSelector assetSelector)
        {
            Resolution = resolution;
            SeekFrame = seekFrame;
            Play = play;
            DoSeek = doSeek;
            AssetSelector = assetSelector;
            PrefabService = prefabService;
            TextureModifierManager = textureModifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public CMiXButton DoSeek { get; set; }
        public GenericValue<int> SeekFrame { get; set; }
        public GenericValue<bool> Play { get; set; }
        public AssetSelector AssetSelector { get; set; }
        public Integer2 Resolution { get; set; }
        public PrefabManager TextureModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new VideoPlayerModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            DoSeek = (ButtonModel)DoSeek.ToModel(),
            SeekFrame = (GenericValueModel<int>)SeekFrame.ToModel(),
            Play = (GenericValueModel<bool>)Play.ToModel(),
            AssetSelector = (AssetSelectorModel)AssetSelector.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (VideoPlayerModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            DoSeek.FromModel(m.DoSeek);
            SeekFrame.FromModel(m.SeekFrame);
            Play.FromModel(m.Play);
            AssetSelector.FromModel(m.AssetSelector);
            Resolution.FromModel(m.Resolution);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
        }

        // The filter modifiers are reachable through this manager alone, so a texture torn down
        // without disposing it leaves their repository and their deleter registrations behind.
        public void Dispose() => DisposeAll(TextureModifierManager);
    }
}
