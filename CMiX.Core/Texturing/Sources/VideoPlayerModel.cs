// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoPlayerModel : IPrefabModel
    {
        public VideoPlayerModel()
        {
            PrefabService = new PrefabServiceModel();
            SeekFrame = new GenericValueModel<int>();
            DoSeek = new ButtonModel();
            Play = new GenericValueModel<bool>(true);
            Resolution = new Integer2Model(0, 0);
            AssetSelector = new AssetSelectorModel();
            FilterManager = new PrefabManagerModel();
        }
        public Guid ID { get; set; } = Guid.NewGuid();

        public ButtonModel DoSeek { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<int> SeekFrame { get; set; }
        public GenericValueModel<bool> Play { get; set; }
        public AssetSelectorModel AssetSelector { get; set; }
        public Integer2Model Resolution { get; internal set; }
        public PrefabManagerModel FilterManager { get; set; }
    }
}
