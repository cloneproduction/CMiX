// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoPlayer : ObservableObject, ITextureSource, IPrefab
    {
        public VideoPlayer(PrefabService prefabService,
                           Integer2 resolution, 
                           GenericValue<int> seekFrame, 
                           GenericValue<bool> play, 
                           Button doSeek, 
                           GenericValue<Asset> asset)
        {
            Resolution = resolution;
            SeekFrame = seekFrame;
            Play = play;
            DoSeek = doSeek;
            Asset = asset;
            PrefabService = prefabService;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Button DoSeek { get; set; }
        public GenericValue<int> SeekFrame { get; set; }
        public GenericValue<bool> Play { get; set; }
        public GenericValue<Asset> Asset { get; set; }
        public Integer2 Resolution { get; set; }
    }
}
