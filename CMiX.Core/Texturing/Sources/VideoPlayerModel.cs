// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record VideoPlayerModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public ButtonModel DoSeek { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<int> SeekFrame { get; set; } = new(0);
        public GenericValueModel<bool> Play { get; set; } = new(true);
        public AssetSelectorModel AssetSelector { get; set; } = new();
        public Integer2Model Resolution { get; set; } = new(0, 0);
        public PrefabManagerModel FilterManager { get; set; } = new();
    }
}
